using System.Globalization;
using System.Security.Claims;
using Microsoft.Extensions.Options;
using Suvarna.DGMS.Api.Helpers;
using Suvarna.DGMS.Api.Models;
using Suvarna.DGMS.Api.Repositories;

namespace Suvarna.DGMS.Api.Services
{
    /// <summary>
    /// NEW: card / UPI payments on a POS machine (Pine Labs, PhonePe, ... - whatever PayLink supports) through PayLink.
    ///
    ///  1. initiate : DGM_POS_TXN row (one open payment per machine and per bill) -> authtoken -> pushtrans
    ///  2. status   : transstatus; SUCCESS only when transactionId, amount and RRN check out
    ///  3. the screen saves the payment as before (check-in / checkout / folio / laundry / reservation)
    ///     with posTxnId; that service claims the SUCCESS row, puts the verified RRN / card last 4 on the
    ///     payment, and links it after the save -> one POS payment can be saved only once
    ///  4. close    : staff closes a payment that never completed (final status check first)
    /// </summary>
    public class PosPaymentService
    {
        public static readonly string[] Purposes = { "CHECKIN", "CHECKOUT", "FOLIO", "LAUNDRY", "RESERVATION" };

        private readonly PosPaymentRepository _repo;
        private readonly PayLinkClient _client;
        private readonly PayLinkSettings _settings;
        private PayLinkOptions _options => _settings.Current;
        private readonly SessionContext _session;
        private readonly IHttpContextAccessor _http;
        private readonly ILogger<PosPaymentService> _log;

        public PosPaymentService(
            PosPaymentRepository repo,
            PayLinkClient client,
            PayLinkSettings settings,
            SessionContext session,
            IHttpContextAccessor http,
            ILogger<PosPaymentService> log)
        {
            _repo = repo;
            _client = client;
            _settings = settings;
            _session = session;
            _http = http;
            _log = log;
        }

        // =====================================================
        // SETTINGS / DEVICES
        // =====================================================

        public ApiResponse<PosSettingsDto> GetSettings()
        {
            return ApiResponse<PosSettingsDto>.Success("POS settings.", new PosSettingsDto
            {
                enabled = _options.Enabled,
                modes = _options.Enabled ? PosModes().ToList() : new List<string>(),
                allowManualCardEntry = !_options.Enabled || _options.AllowManualCardEntry
            });
        }

        public async Task<ApiResponse<List<PosDeviceDto>>> GetDevicesAsync(int bodySessionId)
        {
            var sid = _session.Require(bodySessionId);
            EnsureEnabled();

            var devices = await _repo.GetPineLabMissionDataAsync(sid);

            var list = devices
                .Where(d => !string.IsNullOrWhiteSpace(d.SECURITY_TOKEN) && !string.IsNullOrWhiteSpace(d.MERCHANTID))
                .Select(d => new PosDeviceDto
                {
                    configId = d.CONFIG_ID,
                    terminalName = string.IsNullOrWhiteSpace(d.MAP_TERMINAL_NAME) ? $"POS {d.CONFIG_ID}" : d.MAP_TERMINAL_NAME!.Trim(),
                    edcNo = d.SECURITY_TOKEN?.Trim(),
                    deviceNo = d.DEVICE_NO,
                    autoCancelMinutes = AutoCancel(d)
                })
                .ToList();

            return ApiResponse<List<PosDeviceDto>>.Success(
                list.Count == 0 ? "No POS machine is configured for this location." : "POS machines fetched.", list);
        }

        // =====================================================
        // 1. INITIATE
        // =====================================================

        public async Task<ApiResponse<PosTxnDto>> InitiateAsync(PosInitiateRequest request)
        {
            var sid = _session.Require(request.sessionId);
            EnsureEnabled();

            request.purpose = (request.purpose ?? "").Trim().ToUpperInvariant();
            request.paymentMode = (request.paymentMode ?? "").Trim().ToUpperInvariant();
            request.amount = decimal.Round(request.amount, 2);

            if (!Purposes.Contains(request.purpose))
                throw new UserMessageException("Invalid payment purpose.");
            if (request.amount <= 0)
                throw new UserMessageException("Invalid payment amount.");
            if (request.refId < 0)
                throw new UserMessageException("Invalid bill reference.");
            if (request.refId == 0 && string.IsNullOrWhiteSpace(request.refNo))
                throw new UserMessageException("Guest mobile number is required before taking the card payment.");

            var txnType = TxnType(request.paymentMode)
                ?? throw new UserMessageException(request.paymentMode == "UPI"
                    ? "UPI is not taken on the POS machine. Enter the UPI reference instead."
                    : "This payment mode cannot be sent to the POS machine.");

            var device = await FindDeviceAsync(sid, request.deviceConfigId)
                ?? throw new UserMessageException("Please select a POS machine of this location.");

            var created = await _repo.CreateAsync(sid, request, txnType, device);

            // 22-Oct: an old payment still open on this machine (nobody pressed Check Status / Close).
            // Once the machine's auto-cancel time has passed it is settled here (status check, then closed)
            // and the new payment goes ahead; otherwise the screen gets that payment to check or close.
            if (created.status != "S" && created.blockingPosTxnId > 0)
            {
                var blocking = await _repo.GetAsync(sid, created.blockingPosTxnId);
                if (blocking != null && await SettleExpiredAsync(sid, blocking))
                    created = await _repo.CreateAsync(sid, request, txnType, device);
            }

            if (created.status != "S" || created.posTxnId <= 0)
            {
                if (created.blockingPosTxnId > 0 && await _repo.GetAsync(sid, created.blockingPosTxnId) is { } open)
                {
                    var sameBill = open.REF_ID == request.refId
                                   && (request.refId > 0 || string.Equals(open.REF_NO ?? "", (request.refNo ?? "").Trim(), StringComparison.OrdinalIgnoreCase))
                                   && (open.PURPOSE == request.purpose || (IsBillFamily(open.PURPOSE) && IsBillFamily(request.purpose) && request.refId > 0));
                    var age = (int)Math.Max(0, (DateTime.UtcNow - DateTime.SpecifyKind(open.CREATE_DT, DateTimeKind.Utc)).TotalMinutes);
                    var msg = sameBill
                        ? created.message
                        : $"This POS machine is busy with payment {open.TXN_NO} ({open.CUSTOMER_NM}, Rs.{open.AMOUNT:0.00}, {open.PURPOSE}, {age} min ago). Check its status or close it.";
                    return new ApiResponse<PosTxnDto>
                    {
                        Status = false,
                        Code = sameBill ? "OPEN_FOR_BILL" : "DEVICE_BUSY",
                        Message = msg,
                        Data = ToDto(open, AutoCancel(device))
                    };
                }
                throw new UserMessageException(string.IsNullOrWhiteSpace(created.message) ? "POS payment could not be started." : created.message);
            }

            var ctx = new PayLinkClient.LogContext { PosTxnId = created.posTxnId, TxnNo = created.txnNo, SessionId = sid };

            var payload = new
            {
                provider = device.MERCHANTID,
                edcsNo = device.SECURITY_TOKEN,
                customerName = Clean(request.customerName, 100) ?? "",
                mobileNumber = Clean(request.mobileNo, 15) ?? "",
                patientId = Clean(request.patientId, 50) ?? Clean(request.refNo, 50) ?? "",
                transactionNo = created.txnNo,
                refernceId = "",
                amount = request.amount.ToString("0.00", CultureInfo.InvariantCulture),
                currency = string.IsNullOrWhiteSpace(_options.Currency) ? "INR" : _options.Currency,
                txntype = txnType,
                createby = UserName()
            };

            PayLinkClient.CallResult push;
            try
            {
                push = await _client.PushAsync(payload, ctx);
            }
            catch (Exception ex)
            {
                // never leave the row INITIATED: that would keep the machine busy for everyone
                _log.LogError(ex, "PayLink push crashed pos={PosTxnId}", created.posTxnId);
                push = new PayLinkClient.CallResult { HttpStatus = null, Error = "error while sending: " + ex.Message };
            }

            string newStatus;
            string message;
            string? plTxnId = push.Get("transactionId");
            // 23-Oct: transstatus needs the reference of THIS push ("Reference Id is required" otherwise)
            string? plRefId = PayLinkClient.FindReference(push.Body);

            if (push.PayLinkOk && string.IsNullOrWhiteSpace(plTxnId))
                plTxnId = created.txnNo;

            // No reference id in the push response: send the push transactionId as the reference id
            if (push.PayLinkOk && plRefId == null)
                plRefId = plTxnId;

            if (push.PayLinkOk)
            {
                newStatus = "PUSHED";
                message = "Amount sent to the POS machine. Ask the guest to tap / insert the card, then press Check Status.";
            }
            else if (push.Answered && push.HttpStatus is >= 400 and < 500)
            {
                newStatus = "PUSH_FAILED";
                message = push.Message ?? "PayLink refused the payment.";
            }
            else if (push.Answered && push.HttpOk)
            {
                // PayLink answered "status false": nothing was sent to the machine
                newStatus = "PUSH_FAILED";
                message = push.Message ?? "Payment initiation failed.";
            }
            else if (!push.Answered && push.Error != null && push.Error.StartsWith("PayLink authentication failed"))
            {
                newStatus = "PUSH_FAILED";
                message = push.Error;
            }
            else
            {
                // timeout / 5xx: the machine may or may not show the amount; Check Status asks PayLink by our number
                newStatus = "UNKNOWN";
                plTxnId = created.txnNo;
                message = "No clear answer from PayLink (" + (push.Error ?? "server error") + "). Check the POS machine before charging again.";
            }

            await _repo.UpdateAsync(sid, created.posTxnId, newStatus, plTxnId, plRefId, message: message);

            var row = await _repo.GetAsync(sid, created.posTxnId);
            var dto = ToDto(row!, AutoCancel(device));

            if (newStatus == "PUSH_FAILED")
                return new ApiResponse<PosTxnDto> { Status = false, Code = "400", Message = message, Data = dto };

            return new ApiResponse<PosTxnDto> { Status = true, Code = "200", Message = message, Data = dto };
        }

        // =====================================================
        // 2. STATUS  (idempotent: a SUCCESS row is returned as it is)
        // =====================================================

        public async Task<ApiResponse<PosTxnDto>> CheckStatusAsync(PosStatusRequest request)
        {
            var sid = _session.Require(request.sessionId);
            var row = await _repo.GetAsync(sid, request.posTxnId)
                ?? throw new UserMessageException("POS payment not found.");

            if (row.STATUS == "SUCCESS")
                return ApiResponse<PosTxnDto>.Success("Payment successful.", ToDto(row, 0));

            EnsureEnabled();

            if (string.IsNullOrWhiteSpace(row.PL_TRANSACTION_ID) || RefOf(row) == null)
            {
                return new ApiResponse<PosTxnDto>
                {
                    Status = false, Code = "400",
                    Message = row.STATUS == "PUSH_FAILED"
                        ? "This payment was not sent to the machine. Start a new one."
                        : "PayLink did not return the transaction details, so the status cannot be checked here. Check the POS machine, then close this payment.",
                    Data = ToDto(row, 0)
                };
            }

            var device = await FindDeviceAsync(sid, row.DEVICE_CONFIG_ID)
                ?? throw new UserMessageException("The POS machine of this payment is not available for your login. Check the payment from the location it was taken.");

            var ctx = new PayLinkClient.LogContext { PosTxnId = row.POS_TXN_ID, TxnNo = row.TXN_NO, SessionId = sid };

            var payload = new
            {
                provider = device.MERCHANTID,
                edcsNo = device.SECURITY_TOKEN,
                transactionNo = row.PL_TRANSACTION_ID,
                refernceId = RefOf(row),
                amount = row.AMOUNT.ToString("0.00", CultureInfo.InvariantCulture),
                txntype = row.TXN_TYPE
            };

            var res = await _client.StatusAsync(payload, ctx);

            var respTxnId = res.Get("transactionId");
            var rrn = res.Get("rrn");
            var paidAmount = res.Body == null ? null : PayLinkClient.ReadDecimal(res.Body, "amount");

            string message;
            var failed = false;
            if (!res.Answered || !res.HttpOk)
            {
                // no clear answer: nothing is decided, staff checks again
                message = "Unable to check payment status: " + (res.Error ?? "no answer") + ". Try again.";
            }
            else if (!res.PayLinkOk)
            {
                // PayLink answered "status false": failed, unless the text says the machine is still waiting
                var text = $"{res.Message} {res.Get("orginalMessage")}";
                var pending = _options.PendingMessages.Any(k => !string.IsNullOrWhiteSpace(k) && text.Contains(k, StringComparison.OrdinalIgnoreCase));
                // 23-Oct: PayLink refusing OUR request ("Reference Id is required", "Invalid request") says nothing
                // about the guest's payment: never FAILED / cancelled for that
                var requestError = text.Contains("is required", StringComparison.OrdinalIgnoreCase)
                                   || text.Contains("invalid request", StringComparison.OrdinalIgnoreCase);
                message = res.Message ?? "Payment is not successful.";
                failed = !pending && !requestError;
                if (pending) message = "Still waiting on the POS machine: " + message;
                else if (requestError)
                {
                    message = "PayLink did not accept the status request: " + message + ". Check the machine.";
                    _log.LogWarning("PayLink status request refused pos={PosTxnId}: {Msg}", row.POS_TXN_ID, text);
                }
            }
            else if (!string.Equals(respTxnId, row.PL_TRANSACTION_ID, StringComparison.OrdinalIgnoreCase))
            {
                message = "PayLink returned a different transaction. Check the POS machine.";
                _log.LogWarning("PayLink status txn mismatch pos={PosTxnId} expected={Expected} got={Got}", row.POS_TXN_ID, row.PL_TRANSACTION_ID, respTxnId);
            }
            else if (paidAmount != null && decimal.Round(paidAmount.Value, 2) != row.AMOUNT)
            {
                message = $"PayLink reports Rs.{paidAmount:0.00} but Rs.{row.AMOUNT:0.00} was sent. Not accepted; contact the supervisor.";
                _log.LogWarning("PayLink amount mismatch pos={PosTxnId} sent={Sent} paid={Paid}", row.POS_TXN_ID, row.AMOUNT, paidAmount);
            }
            else if (string.IsNullOrWhiteSpace(rrn))
            {
                message = "Payment approved but RRN / UTR not received yet. Check the status again.";
            }
            else
            {
                var cardNo = res.Get("cardNo");
                var last4 = PayLinkMask.CardLast4(cardNo);
                var masked = string.IsNullOrWhiteSpace(cardNo) ? null : PayLinkMask.Last4(cardNo);

                var upd = await _repo.UpdateAsync(sid, row.POS_TXN_ID, "SUCCESS",
                    respTxnId, string.IsNullOrWhiteSpace(res.Get("refernceId")) ? row.PL_REFERENCE_ID : res.Get("refernceId"),
                    rrn, masked, last4, res.Get("upi"), paidAmount ?? row.AMOUNT,
                    message: $"Payment successful. RRN {rrn}", isStatusCheck: true);

                if (upd.status != "S")
                    _log.LogError("PayLink SUCCESS could not be stored pos={PosTxnId}: {Msg}", row.POS_TXN_ID, upd.message);

                var paid = await _repo.GetAsync(sid, row.POS_TXN_ID);
                return ApiResponse<PosTxnDto>.Success($"Payment successful. RRN: {rrn}", ToDto(paid!, AutoCancel(device)));
            }

            // FAILED: take it off the machine (so the guest cannot pay it later), free the machine;
            // the screen shows the reason and "Initiate Payment" again
            if (failed && (row.STATUS == "PUSHED" || row.STATUS == "UNKNOWN"))
            {
                var cancelled = await CancelOnMachineAsync(sid, row, device);
                await _repo.UpdateAsync(sid, row.POS_TXN_ID, "FAILED",
                    message: "Payment failed: " + message + (cancelled ? " (cancelled on machine)" : ""),
                    isStatusCheck: true, closeReason: "Status: " + message);

                var failedRow = await _repo.GetAsync(sid, row.POS_TXN_ID);
                return new ApiResponse<PosTxnDto> { Status = false, Code = "FAILED", Message = "Payment failed: " + message, Data = ToDto(failedRow!, AutoCancel(device)) };
            }

            // not decided (no answer / still waiting / recheck of a closed one): keep the state
            await _repo.UpdateAsync(sid, row.POS_TXN_ID, row.STATUS, message: message, isStatusCheck: true);

            var current = await _repo.GetAsync(sid, row.POS_TXN_ID);
            return new ApiResponse<PosTxnDto> { Status = false, Code = "PENDING", Message = message, Data = ToDto(current!, AutoCancel(device)) };
        }

        // =====================================================
        // 3. CLOSE a payment that never completed (frees the machine)
        // =====================================================

        public async Task<ApiResponse<PosTxnDto>> CloseAsync(PosCloseRequest request)
        {
            var sid = _session.Require(request.sessionId);
            var reason = (request.reason ?? "").Trim();
            if (reason.Length < 5)
                throw new UserMessageException("Please enter the reason for closing this POS payment.");

            var row = await _repo.GetAsync(sid, request.posTxnId)
                ?? throw new UserMessageException("POS payment not found.");

            if (row.STATUS == "SUCCESS")
                throw new UserMessageException("This POS payment is successful. Save it; it cannot be closed.");
            if (row.STATUS is "CANCELLED" or "PUSH_FAILED" or "FAILED")
                return ApiResponse<PosTxnDto>.Success("Already closed.", ToDto(row, 0));

            var device = await FindDeviceAsync(sid, row.DEVICE_CONFIG_ID);
            var autoCancel = device == null ? _options.DefaultAutoCancelMinutes : AutoCancel(device);

            // last status check: never close a payment the guest has actually made
            if (_options.Enabled && !string.IsNullOrWhiteSpace(row.PL_TRANSACTION_ID) && RefOf(row) != null && device != null)
            {
                var check = await CheckStatusAsync(new PosStatusRequest { sessionId = request.sessionId, posTxnId = row.POS_TXN_ID });
                if (check.Data?.isPaid == true)
                    return check;   // paid after all -> the screen saves it

                if (check.Data?.status is "FAILED")
                    return ApiResponse<PosTxnDto>.Success("POS payment failed and is closed. " + check.Message, check.Data);

                // 23-Oct: canceltrans is best effort (it does not always work): the payment is closed in DGMS
                // either way; when the cancel works the machine is cleared too
                reason += await CancelOnMachineAsync(sid, row, device)
                    ? " (cancelled on machine via PayLink)"
                    : " (PayLink cancel not confirmed - cancel on the machine if it still shows the amount)";
            }

            // INITIATED never reached the machine: PUSH_FAILED is its closed state
            var upd = await _repo.UpdateAsync(sid, row.POS_TXN_ID, row.STATUS == "INITIATED" ? "PUSH_FAILED" : "CANCELLED",
                message: "Closed by staff: " + reason, closeReason: reason);
            if (upd.status != "S")
                throw new UserMessageException(upd.message);

            _log.LogWarning("POS payment closed pos={PosTxnId} txn={TxnNo} by user={User}: {Reason}", row.POS_TXN_ID, row.TXN_NO, UserName(), reason);

            var closed = await _repo.GetAsync(sid, row.POS_TXN_ID);
            return ApiResponse<PosTxnDto>.Success("POS payment closed. If the guest is charged later it appears under POS reconciliation.", ToDto(closed!, autoCancel));
        }

        /// <summary>
        /// An open payment whose machine auto-cancel time has passed: status check first (a late payment becomes
        /// SUCCESS and shows under POS Reconciliation), otherwise cancel on the machine and close it.
        /// True when the payment is no longer open.
        /// </summary>
        private async Task<bool> SettleExpiredAsync(string sid, PosTxnRow row)
        {
            if (row.STATUS is not ("INITIATED" or "PUSHED" or "UNKNOWN"))
                return true;

            var device = await FindDeviceAsync(sid, row.DEVICE_CONFIG_ID);
            var autoCancel = device == null ? _options.DefaultAutoCancelMinutes : AutoCancel(device);
            var age = DateTime.UtcNow - DateTime.SpecifyKind(row.CREATE_DT, DateTimeKind.Utc);
            if (age < TimeSpan.FromMinutes(autoCancel + 1))
                return false;

            try
            {
                if (device != null && !string.IsNullOrWhiteSpace(row.PL_TRANSACTION_ID) && RefOf(row) != null)
                {
                    var check = await CheckStatusAsync(new PosStatusRequest { posTxnId = row.POS_TXN_ID });
                    if (check.Data != null && !check.Data.isOpen)
                        return true;

                    await CancelOnMachineAsync(sid, row, device);
                }

                var reason = $"Expired: machine auto-cancel time ({autoCancel} min) passed without payment; closed when the machine was used again";
                var current = await _repo.GetAsync(sid, row.POS_TXN_ID) ?? row;
                if (current.STATUS is not ("INITIATED" or "PUSHED" or "UNKNOWN"))
                    return true;
                // INITIATED never reached the machine -> PUSH_FAILED (the allowed move from INITIATED)
                var upd = await _repo.UpdateAsync(sid, row.POS_TXN_ID, current.STATUS == "INITIATED" ? "PUSH_FAILED" : "CANCELLED",
                    message: reason, closeReason: reason);
                _log.LogWarning("POS payment expired and closed pos={PosTxnId} txn={TxnNo}: {Msg}", row.POS_TXN_ID, row.TXN_NO, upd.message);
                return upd.status == "S";
            }
            catch (Exception ex)
            {
                _log.LogError(ex, "Expired POS payment could not be settled pos={PosTxnId}", row.POS_TXN_ID);
                return false;
            }
        }

        /// <summary>canceltrans; true when PayLink confirms the cancel</summary>
        private async Task<bool> CancelOnMachineAsync(string sid, PosTxnRow row, PineLabMissionData device)
        {
            if (string.IsNullOrWhiteSpace(_options.CancelUrl) || string.IsNullOrWhiteSpace(row.PL_TRANSACTION_ID) || RefOf(row) == null)
                return false;

            var cancel = await _client.CancelAsync(new
            {
                provider = device.MERCHANTID,
                edcsNo = device.SECURITY_TOKEN,
                transactionNo = row.PL_TRANSACTION_ID,
                refernceId = RefOf(row),
                amount = row.AMOUNT.ToString("0.00", CultureInfo.InvariantCulture),
                txntype = row.TXN_TYPE
            }, new PayLinkClient.LogContext { PosTxnId = row.POS_TXN_ID, TxnNo = row.TXN_NO, SessionId = sid });

            if (!cancel.PayLinkOk)
                _log.LogWarning("PayLink cancel not confirmed pos={PosTxnId}: {Msg}", row.POS_TXN_ID, cancel.Message);

            return cancel.PayLinkOk;
        }

        // =====================================================
        // OPEN PAYMENT OF A SCREEN (resume after refresh)
        // =====================================================

        public async Task<ApiResponse<PosTxnDto?>> GetOpenAsync(int bodySessionId, string purpose, int refId, string? refNo)
        {
            var sid = _session.Require(bodySessionId);
            var row = await _repo.GetOpenAsync(sid, (purpose ?? "").Trim().ToUpperInvariant(), refId, refNo);
            return ApiResponse<PosTxnDto?>.Success(row == null ? "No open POS payment." : "Open POS payment found.",
                row == null ? null : ToDto(row, _options.DefaultAutoCancelMinutes));
        }

        public async Task<ApiResponse<PosTxnDto>> GetAsync(int bodySessionId, long posTxnId)
        {
            var sid = _session.Require(bodySessionId);
            var row = await _repo.GetAsync(sid, posTxnId) ?? throw new UserMessageException("POS payment not found.");
            return ApiResponse<PosTxnDto>.Success("POS payment.", ToDto(row, _options.DefaultAutoCancelMinutes));
        }

        // =====================================================
        // RECONCILIATION
        // =====================================================

        public async Task<ApiResponse<List<PosTxnDto>>> ListAsync(int bodySessionId, DateTime fromDt, DateTime toDt, string? filter)
        {
            var sid = _session.Require(bodySessionId);
            if (toDt < fromDt) throw new UserMessageException("To date must be after From date.");
            if ((toDt - fromDt).TotalDays > 92) throw new UserMessageException("Please choose up to 3 months.");

            var f = (filter ?? "ALL").Trim().ToUpperInvariant();
            if (f is not ("ALL" or "OPEN" or "UNSAVED" or "MISMATCH" or "FAILED")) f = "ALL";

            var rows = await _repo.ListAsync(sid, fromDt, toDt, f);
            return ApiResponse<List<PosTxnDto>>.Success("POS payments fetched.",
                rows.Select(r => ToDto(r, _options.DefaultAutoCancelMinutes)).ToList());
        }

        public async Task<ApiResponse<List<PosApiLogDto>>> GetLogsAsync(int bodySessionId, long posTxnId)
        {
            var sid = _session.Require(bodySessionId);
            var rows = await _repo.GetLogsAsync(sid, posTxnId);
            return ApiResponse<List<PosApiLogDto>>.Success("PayLink log.", rows.Select(l => new PosApiLogDto
            {
                logId = l.LOG_ID,
                apiName = l.API_NAME,
                requestUrl = l.REQUEST_URL,
                requestBody = l.REQUEST_BODY,
                responseBody = l.RESPONSE_BODY,
                httpStatus = l.HTTP_STATUS,
                isSuccess = l.IS_SUCCESS == "Y",
                error = l.ERROR_MSG,
                durationMs = l.DURATION_MS,
                correlationId = l.CORRELATION_ID,
                createdAt = DateTime.SpecifyKind(l.CREATE_DT, DateTimeKind.Utc)
            }).ToList());
        }

        // =====================================================
        // USED BY THE PAYMENT SERVICES (check-in, checkout, folio, laundry, reservation)
        // =====================================================

        /// <summary>
        /// Before a payment is saved. posTxnId given -> reserve that SUCCESS POS payment (same purpose, bill and
        /// exact amount) and return it; the caller puts its RRN / card last 4 on the payment.
        /// No posTxnId -> null, unless card payments must go through the POS machine.
        /// </summary>
        public PosClaim? ClaimForPayment(string sessionId, long? posTxnId, string purpose, int refId, decimal amount, string? paymentMode)
        {
            var mode = (paymentMode ?? "").Trim().ToUpperInvariant();

            if (posTxnId is null or <= 0)
            {
                if (_options.Enabled && !_options.AllowManualCardEntry && amount > 0 && PosModes().Contains(mode))
                    throw new UserMessageException("Card payments must be taken on the POS machine. Use 'Send to POS'.");
                return null;
            }

            var (claim, message) = _repo.Claim(sessionId, posTxnId.Value, purpose, refId, decimal.Round(amount, 2));
            if (claim == null)
                throw new UserMessageException(message);

            if (!string.Equals(claim.PaymentMode, mode, StringComparison.OrdinalIgnoreCase))
            {
                SafeRelease(sessionId, claim);
                throw new UserMessageException($"The POS payment was taken as {claim.PaymentMode}. Select {claim.PaymentMode} as the payment mode.");
            }

            return claim;
        }

        /// <summary>After a successful save. Never throws: the payment is already saved.</summary>
        public string? LinkAfterSave(string sessionId, PosClaim? claim, int? bookingId, int? billId, int? paymentId)
        {
            if (claim == null) return null;

            for (var attempt = 1; attempt <= 2; attempt++)
            {
                try
                {
                    var (status, message) = _repo.Link(sessionId, claim, bookingId, billId, paymentId);
                    if (status == "S")
                    {
                        if (message.StartsWith("Saved, but", StringComparison.OrdinalIgnoreCase))
                        {
                            _log.LogWarning("POS amount mismatch after save pos={PosTxnId}: {Msg}", claim.PosTxnId, message);
                            return message;
                        }
                        return null;
                    }
                    _log.LogError("POS link refused pos={PosTxnId}: {Msg}", claim.PosTxnId, message);
                    return null;
                }
                catch (Exception ex)
                {
                    _log.LogError(ex, "POS link failed pos={PosTxnId} attempt {Attempt}", claim.PosTxnId, attempt);
                }
            }

            return null;
        }

        /// <summary>The save failed: the POS payment can be used again.</summary>
        public void SafeRelease(string sessionId, PosClaim? claim)
        {
            if (claim == null) return;
            try
            {
                _repo.Release(sessionId, claim);
            }
            catch (Exception ex)
            {
                _log.LogError(ex, "POS claim release failed pos={PosTxnId}; it frees itself after 5 minutes", claim.PosTxnId);
            }
        }

        // =====================================================
        // HELPERS
        // =====================================================

        private IEnumerable<string> PosModes() =>
            _options.TxnTypes.Where(kv => !string.IsNullOrWhiteSpace(kv.Value)).Select(kv => kv.Key.ToUpperInvariant());

        /// <summary>Reference for transstatus / canceltrans: the push reference id, else the push transactionId.</summary>
        private static string? RefOf(PosTxnRow r) =>
            !string.IsNullOrWhiteSpace(r.PL_REFERENCE_ID) ? r.PL_REFERENCE_ID
            : !string.IsNullOrWhiteSpace(r.PL_TRANSACTION_ID) ? r.PL_TRANSACTION_ID : null;

        private static bool IsBillFamily(string? purpose) => purpose is "CHECKOUT" or "FOLIO";

        private string? TxnType(string mode) =>
            _options.TxnTypes.TryGetValue(mode, out var t) && !string.IsNullOrWhiteSpace(t) ? t.Trim() : null;

        private void EnsureEnabled()
        {
            if (!_options.Enabled)
                throw new UserMessageException("POS machine payments are switched off.");
        }

        private async Task<PineLabMissionData?> FindDeviceAsync(string sid, int configId)
        {
            var devices = await _repo.GetPineLabMissionDataAsync(sid);
            return devices.FirstOrDefault(d => d.CONFIG_ID == configId
                                               && !string.IsNullOrWhiteSpace(d.SECURITY_TOKEN)
                                               && !string.IsNullOrWhiteSpace(d.MERCHANTID));
        }

        private int AutoCancel(PineLabMissionData d) =>
            d.AUTO_CANCEL_DURATION_MINUTES is > 0 ? d.AUTO_CANCEL_DURATION_MINUTES.Value : _options.DefaultAutoCancelMinutes;

        private string UserName()
        {
            var user = _http.HttpContext?.User;
            return (user?.FindFirst(ClaimTypes.Name)?.Value ?? user?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "").Trim();
        }

        private static string? Clean(string? value, int max)
        {
            if (string.IsNullOrWhiteSpace(value)) return null;
            var v = value.Trim();
            return v.Length > max ? v[..max] : v;
        }

        private static PosTxnDto ToDto(PosTxnRow r, int autoCancelMinutes) => new()
        {
            posTxnId = r.POS_TXN_ID,
            txnNo = r.TXN_NO ?? "",
            purpose = r.PURPOSE,
            refId = r.REF_ID,
            refNo = r.REF_NO,
            paymentMode = r.PAYMENT_MODE,
            amount = r.AMOUNT,
            deviceConfigId = r.DEVICE_CONFIG_ID,
            terminalName = r.TERMINAL_NM,
            status = r.STATUS,
            isPaid = r.STATUS == "SUCCESS",
            isOpen = r.STATUS is "INITIATED" or "PUSHED" or "UNKNOWN",
            canCheckStatus = !string.IsNullOrWhiteSpace(r.PL_TRANSACTION_ID) && RefOf(r) != null && r.STATUS != "SUCCESS",
            isSaved = r.CLAIM_STATUS == "LINKED",
            rrn = r.RRN,
            cardMasked = r.CARD_MASKED,
            cardLast4 = r.CARD_LAST4,
            cardHolderName = r.CARD_HOLDER_NM,
            message = r.LAST_MESSAGE,
            statusCheckCount = r.STATUS_CHECK_COUNT,
            createdAt = DateTime.SpecifyKind(r.CREATE_DT, DateTimeKind.Utc),
            paidAt = r.SUCCESS_DT == null ? null : DateTime.SpecifyKind(r.SUCCESS_DT.Value, DateTimeKind.Utc),
            autoCancelMinutes = autoCancelMinutes,
            customerName = r.CUSTOMER_NM,
            bookingNo = r.BOOKING_NO,
            recordedAmount = r.RECORDED_AMOUNT,
            amountMismatch = r.AMOUNT_MISMATCH == "Y",
            closeReason = r.CLOSE_REASON,
            plTransactionId = r.PL_TRANSACTION_ID
        };
    }
}
