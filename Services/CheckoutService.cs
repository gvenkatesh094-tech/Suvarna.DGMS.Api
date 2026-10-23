using Suvarna.DGMS.Api.Models;
using Suvarna.DGMS.Api.Repositories;

namespace Suvarna.DGMS.Api.Services
{
    public class CheckoutService
    {
        private readonly CheckoutRepository _repo;
        private readonly GuestFolioBillingRepository _billing;   // NEW (Round B): GST invoice right after check-out
        private readonly PosPaymentService _pos;                 // NEW: POS machine payments
        private readonly Suvarna.DGMS.Api.Helpers.SessionContext _session;

        private static readonly HashSet<string> AllowedPaymentModes =
            new(StringComparer.OrdinalIgnoreCase) { "CASH", "CARD", "UPI", "ONLINE", "CORP_BILL" };

        // Same list as DepositLedgerService.ValidRefundModes / PR_DGMS_REFUND_DEPOSIT
        private static readonly HashSet<string> AllowedRefundModes =
            new(StringComparer.OrdinalIgnoreCase) { "CASH", "UPI", "BANK_TRANSFER", "CARD_REVERSAL", "CHEQUE" };

        public CheckoutService(CheckoutRepository repo, GuestFolioBillingRepository billing,
            PosPaymentService pos, Suvarna.DGMS.Api.Helpers.SessionContext session)
        {
            _repo = repo;
            _billing = billing;
            _pos = pos;
            _session = session;
        }

        //====================================================
        // GET CHECKOUT BOOKINGS
        //====================================================

        public ApiResponse<List<CheckoutBooking>> GetCheckoutBookings(int sessionId, string? searchTerm = null)
        {
            var data = _repo.GetCheckoutBookings(sessionId, searchTerm);
            return ApiResponse<List<CheckoutBooking>>.Success("Checkout bookings fetched successfully.", data);
        }

        //====================================================
        // PREPARE CHECKOUT
        //====================================================

        public ApiResponse<CheckoutDetails> PrepareCheckout(int sessionId, int bookingId, DateTime checkoutDt)
        {
            var data = _repo.PrepareCheckout(sessionId, bookingId, checkoutDt)
                ?? throw new Suvarna.DGMS.Api.Helpers.UserMessageException("Checkout details not found.");

            // FIX: someone else may have checked this guest out already
            if (!string.IsNullOrEmpty(data.bookingStatus) && data.bookingStatus != "CHECKEDIN")
            {
                throw new Suvarna.DGMS.Api.Helpers.UserMessageException("This booking is no longer checked in — it may already have been checked out.");
            }

            return ApiResponse<CheckoutDetails>.Success("Checkout details fetched successfully.", data);
        }

        //====================================================
        // GET CHECKOUT DETAILS
        //====================================================

        public ApiResponse<CheckoutDetails> GetCheckoutDetails(int sessionId, int bookingId, DateTime? checkoutDt)
        {
            var data = _repo.GetCheckoutDetails(sessionId, bookingId, checkoutDt)
                ?? throw new Suvarna.DGMS.Api.Helpers.UserMessageException("Checkout details not found.");

            return ApiResponse<CheckoutDetails>.Success("Checkout details fetched successfully.", data);
        }

        //====================================================
        // CONFIRM CHECKOUT
        //====================================================

        public ApiResponse<CheckoutResult> ConfirmCheckout(CheckoutRequest request)
        {
            // NEW: card paid on the POS machine -> RRN and card last 4 come from the verified POS payment
            if (request == null) throw new Suvarna.DGMS.Api.Helpers.UserMessageException("Checkout request is required.");

            var sid = _session.Require(request.sessionId);
            var pos = _pos.ClaimForPayment(sid, request.posTxnId, "CHECKOUT", request.bookingId, request.paymentAmount, request.paymentMode);
            if (pos != null)
            {
                request.paymentReference = pos.Rrn;
                request.cardLast4 = pos.CardLast4;
                request.upiVpa = null;
            }

            CheckoutResult data;
            try
            {
                ValidateCheckout(request);
                data = _repo.ConfirmCheckout(request);
            }
            catch
            {
                _pos.SafeRelease(sid, pos);
                throw;
            }

            if (data.status != "S")
            {
                _pos.SafeRelease(sid, pos);
                throw new Suvarna.DGMS.Api.Helpers.UserMessageException(string.IsNullOrWhiteSpace(data.message) ? "Checkout failed." : data.message);
            }

            var posWarning = _pos.LinkAfterSave(sid, pos, request.bookingId, data.billId, data.paymentTxnId);
            if (posWarning != null) data.message = (data.message ?? "") + " | " + posWarning;

            // NEW (Round B): the bill is final now -> issue the GST tax invoice. Check-out is already saved,
            // so a problem here never undoes it; the invoice can be issued later from Guest Folio.
            try
            {
                var inv = _billing.GenerateInvoice(new GenerateInvoiceRequest { sessionId = request.sessionId, bookingId = request.bookingId });
                if (inv.status == "S" && !string.IsNullOrWhiteSpace(inv.invoiceNo))
                {
                    data.gstInvoiceNo = inv.invoiceNo;
                    data.gstInvoiceId = inv.invoiceId;
                    data.message = (data.message ?? "") + " | GST invoice: " + inv.invoiceNo;
                }
                else if (!string.IsNullOrWhiteSpace(inv.message))
                {
                    data.gstInvoiceMessage = inv.message;
                }
            }
            catch (Exception)
            {
                data.gstInvoiceMessage = "GST invoice was not issued - issue it from Guest Folio.";
            }

            return ApiResponse<CheckoutResult>.Success(data.message, data);
        }

        //====================================================
        // SERVER VALIDATION
        //====================================================

        private static void ValidateCheckout(CheckoutRequest request)
        {
            if (request == null) throw new Suvarna.DGMS.Api.Helpers.UserMessageException("Checkout request is required.");
            if (request.sessionId <= 0) throw new Suvarna.DGMS.Api.Helpers.UserMessageException("Session Id is required.");
            if (request.bookingId <= 0) throw new Suvarna.DGMS.Api.Helpers.UserMessageException("Booking is required.");
            if (request.checkoutDt == DateTime.MinValue) throw new Suvarna.DGMS.Api.Helpers.UserMessageException("Checkout date is required.");

            if (request.paymentAmount < 0) throw new Suvarna.DGMS.Api.Helpers.UserMessageException("Payment amount cannot be negative.");

            // FIX: payment mode is only mandatory when money is actually being
            // collected (or billed to corporate). When the deposit covers the
            // whole bill there is nothing to collect, and staff were forced to
            // pick a random mode. The SP re-checks this against the real balance.
            // FIX: "corp_bill" in lower case skipped the CORP_BILL checks below
            request.paymentMode = request.paymentMode?.Trim().ToUpperInvariant();
            bool hasMode = !string.IsNullOrWhiteSpace(request.paymentMode);

            if (request.paymentAmount > 0 && !hasMode)
                throw new Suvarna.DGMS.Api.Helpers.UserMessageException("Payment mode is required.");

            if (hasMode && !AllowedPaymentModes.Contains(request.paymentMode!))
                throw new Suvarna.DGMS.Api.Helpers.UserMessageException("Invalid payment mode.");

            if (request.paymentMode == "CORP_BILL" && request.paymentAmount > 0)
                throw new Suvarna.DGMS.Api.Helpers.UserMessageException("Payment amount must be 0 when billing to Corporate — the balance will be invoiced separately.");

            // FIX: RRN / UTR mandatory for card, UPI and online — was only
            // enforced on the screen.
            if (request.paymentAmount > 0 &&
                (request.paymentMode == "CARD" || request.paymentMode == "UPI" || request.paymentMode == "ONLINE") &&
                string.IsNullOrWhiteSpace(request.paymentReference))
            {
                throw new Suvarna.DGMS.Api.Helpers.UserMessageException(request.paymentMode == "ONLINE"
                    ? "UTR / reference number is required for online payment."
                    : "RRN number is required for card / UPI payment.");
            }

            // SP column is VARCHAR(50) — 100 used to pass here and then fail/truncate in SQL
            if (!string.IsNullOrWhiteSpace(request.paymentReference) && request.paymentReference.Trim().Length > 50)
                throw new Suvarna.DGMS.Api.Helpers.UserMessageException("Payment reference cannot exceed 50 characters.");

            if (!string.IsNullOrWhiteSpace(request.cardLast4) &&
                (request.cardLast4.Length != 4 || !request.cardLast4.All(char.IsDigit)))
                throw new Suvarna.DGMS.Api.Helpers.UserMessageException("Card last 4 digits must be exactly 4 numbers.");

            if (!string.IsNullOrWhiteSpace(request.refundMode) && !AllowedRefundModes.Contains(request.refundMode))
                throw new Suvarna.DGMS.Api.Helpers.UserMessageException("Invalid refund mode.");

            if (request.discountAmount < 0) throw new Suvarna.DGMS.Api.Helpers.UserMessageException("Discount amount cannot be negative.");
            if (request.discountAmount > 0 && string.IsNullOrWhiteSpace(request.discountReason))
                throw new Suvarna.DGMS.Api.Helpers.UserMessageException("Discount reason is required.");
            if (!string.IsNullOrWhiteSpace(request.discountReason) && request.discountReason.Length > 200)
                throw new Suvarna.DGMS.Api.Helpers.UserMessageException("Discount reason cannot exceed 200 characters.");

            if (request.penaltyAmount < 0) throw new Suvarna.DGMS.Api.Helpers.UserMessageException("Penalty amount cannot be negative.");
            if (request.penaltyAmount > 0 && string.IsNullOrWhiteSpace(request.penaltyReasonCode))
                throw new Suvarna.DGMS.Api.Helpers.UserMessageException("Penalty reason is required when a penalty amount is entered.");
            if (request.penaltyAmount > 0 && string.IsNullOrWhiteSpace(request.penaltyRemarks))
                throw new Suvarna.DGMS.Api.Helpers.UserMessageException("Penalty remarks are required when a penalty amount is entered.");
            if (!string.IsNullOrWhiteSpace(request.penaltyRemarks) && request.penaltyRemarks.Length > 400)
                throw new Suvarna.DGMS.Api.Helpers.UserMessageException("Penalty remarks cannot exceed 400 characters.");

            if (!request.roomInspected) throw new Suvarna.DGMS.Api.Helpers.UserMessageException("Room inspection must be completed.");
            if (!request.keyReturned) throw new Suvarna.DGMS.Api.Helpers.UserMessageException("Room key must be returned.");
            if (!request.signatureObtained) throw new Suvarna.DGMS.Api.Helpers.UserMessageException("Guest signature is required.");

            if (!string.IsNullOrWhiteSpace(request.remarks) && request.remarks.Length > 500)
                throw new Suvarna.DGMS.Api.Helpers.UserMessageException("Remarks cannot exceed 500 characters.");
        }
    }
}
