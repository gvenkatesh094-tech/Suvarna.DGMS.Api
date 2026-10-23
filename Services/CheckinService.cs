using Suvarna.DGMS.Api.Models;
using Suvarna.DGMS.Api.Repositories;

namespace Suvarna.DGMS.Api.Services
{
    public class CheckinService
    {
        private readonly CheckinRepository _repo;
        private readonly PosPaymentService _pos;                       // NEW: POS machine payments
        private readonly Suvarna.DGMS.Api.Helpers.SessionContext _session;

        public CheckinService(
            CheckinRepository repo,
            PosPaymentService pos,
            Suvarna.DGMS.Api.Helpers.SessionContext session)
        {
            _repo = repo;
            _pos = pos;
            _session = session;
        }

        // ==========================================
        // GET ROOMS
        // ==========================================

        public ApiResponse<List<CheckinRoom>> GetRooms(int sessionId, int? locationId, int? occCatId,DateTime checkinDt  , DateTime checkOutDt, int? bookinId, int? accmTypeId = null)
        {
            var data = _repo.GetRooms(
                sessionId,
                locationId,
                occCatId,
                checkinDt,  checkOutDt, bookinId, accmTypeId
            );

            return ApiResponse<List<CheckinRoom>>.Success(
                "Available rooms fetched successfully.",
                data
            );
        }

        // ==========================================
        // GET TARIFF
        // ==========================================

        public ApiResponse<List<CheckinTariff>> GetTariff(
        int sessionId,
        int propertyId,
        int accmTypeId,
        int occCatId,
        DateTime checkinDt,
        string? billingModel)
        {
            var data = _repo.GetTariff(
                sessionId,
                propertyId,
                accmTypeId,
                occCatId,
                checkinDt,
                billingModel
            );

            return ApiResponse<List<CheckinTariff>>.Success(
                data.Count == 0
                    ? "No applicable tariff found."
                    : "Tariff fetched successfully.",
                data
            );
        }

        // ==========================================
        // NEW: STAY QUOTE
        // ==========================================

        public ApiResponse<StayQuote?> GetStayQuote(int sessionId, int tariffId, DateTime checkinDt, DateTime checkoutDt, int? occCatId = null, int? corpAgreementId = null)
        {
            if (tariffId <= 0)
                throw new Suvarna.DGMS.Api.Helpers.UserMessageException("Please select a tariff.");

            if (checkoutDt <= checkinDt)
                throw new Suvarna.DGMS.Api.Helpers.UserMessageException("Expected check-out must be after the check-in time.");

            var data = _repo.GetStayQuote(sessionId, tariffId, checkinDt, checkoutDt, occCatId, corpAgreementId);

            return ApiResponse<StayQuote?>.Success(
                data == null ? "Tariff not found." : "Stay amount calculated.",
                data);
        }

        // ==========================================
        // CONFIRM CHECK-IN
        // ==========================================

        public ApiResponse<CheckinResult> ConfirmCheckin(Checkin request, IFormFile? idFile, IFormFile? photoFile)
        {
            //ValidateCheckin(request);

            // NEW: the request is form data, so SessionBindingFilter cannot replace its sessionId:
            // use the checked X-Session-Id header (same rule as every other API)
            var sid = _session.Require(request.sessionId);
            if (int.TryParse(sid, out var headerSessionId)) request.sessionId = headerSessionId;

            // NEW: card paid on the POS machine -> one swipe covers room + GST + deposit; RRN and card
            // last 4 come from the verified POS payment, not from the screen
            var pos = _pos.ClaimForPayment(sid, request.posTxnId, "CHECKIN", request.existingBookingId ?? 0,
                request.totalPaidAmount + request.depositAmount, request.paymentMode);
            if (pos != null)
            {
                request.paymentRef = pos.Rrn;
                request.depositRef = pos.Rrn;
                request.depositPaymentMode = request.paymentMode;
                request.cardLast4 = pos.CardLast4;
                request.upiVpa = null;
            }

            CheckinResult data;
            try
            {
                data = _repo.ConfirmCheckin(request, idFile, photoFile);
            }
            catch
            {
                _pos.SafeRelease(sid, pos);
                throw;
            }

            // NEW: company credit limit crossed -> the screen asks the user and sends creditOverride = 'Y'
            if (data.status == "W")
            {
                _pos.SafeRelease(sid, pos);
                return ApiResponse<CheckinResult>.Fail(data.message, "CREDIT_LIMIT");
            }

            if (data.status != "S")
            {
                _pos.SafeRelease(sid, pos);
                throw new Suvarna.DGMS.Api.Helpers.UserMessageException(
                    string.IsNullOrWhiteSpace(data.message)
                        ? "Check-in failed."
                        : data.message
                );
            }

            var posWarning = _pos.LinkAfterSave(sid, pos, data.bookingId, data.billId, null);
            if (posWarning != null) data.message = (data.message ?? "") + " | " + posWarning;

            return ApiResponse<CheckinResult>.Success(data.message, data);
        }
        public ApiResponse<List<PatientLookup>> SearchPatients(string? searchBy, string? searchText)
        {
            string text = (searchText ?? "").Trim();

            // Same rule as the UI: at least 3 characters
            if (text.Length < 3)
            {
                return ApiResponse<List<PatientLookup>>.Success(
                    "Enter at least 3 characters to search.",
                    new List<PatientLookup>()
                );
            }

            string mode = string.Equals(searchBy, "NAME", StringComparison.OrdinalIgnoreCase)
                ? "NAME"
                : "ALL";

            var data = _repo.SearchPatients(mode, text);

            return ApiResponse<List<PatientLookup>>.Success(
                data.Count == 0 ? "No matching patient found." : "Patients fetched successfully.",
                data
            );
        }


    }
}