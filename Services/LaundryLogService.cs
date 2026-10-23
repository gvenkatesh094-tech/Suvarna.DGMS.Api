using Suvarna.DGMS.Api.Models;
using Suvarna.DGMS.Api.Repositories;

namespace Suvarna.DGMS.Api.Services
{
    public class LaundryLogService
    {
        private readonly LaundryLogRepository _repo;
        private readonly PosPaymentService _pos;          // NEW: POS machine payments
        private readonly Suvarna.DGMS.Api.Helpers.SessionContext _session;

        public LaundryLogService(LaundryLogRepository repo, PosPaymentService pos, Suvarna.DGMS.Api.Helpers.SessionContext session)
        {
            _repo = repo;
            _pos = pos;
            _session = session;
        }

        public ApiResponse<List<LaundryLogRow>> GetEntries(int sessionId, string? status, string? type, string? scope)
        {
            var data = _repo.GetEntries(sessionId, status, type, scope);
            return ApiResponse<List<LaundryLogRow>>.Success("Laundry log fetched successfully.", data);
        }


        public ApiResponse<List<LaundryBookingLookup>> GetActiveBookings(int sessionId)
        {
            var data = _repo.GetActiveBookings(sessionId);
            return ApiResponse<List<LaundryBookingLookup>>.Success("Active bookings fetched successfully.", data);
        }

        public ApiResponse<LaundryActionResult> CreateLog(CreateLaundryLogRequest request)
        {
            if (request.items == null || request.items.Count == 0 || !request.items.Exists(i => !string.IsNullOrWhiteSpace(i.name)))
            {
                throw new Suvarna.DGMS.Api.Helpers.UserMessageException("At least one laundry item is required.");
            }

            // FIX: a zero / negative quantity was summed as sent and later posted as a credit to the guest
            if (request.items.Exists(i => !string.IsNullOrWhiteSpace(i.name) && i.qty <= 0))
            {
                throw new Suvarna.DGMS.Api.Helpers.UserMessageException("Laundry item quantity must be at least 1.");
            }

            var data = _repo.CreateLog(request);

            if (data.status != "S")
            {
                throw new Suvarna.DGMS.Api.Helpers.UserMessageException(string.IsNullOrWhiteSpace(data.message) ? "Failed to log laundry pickup." : data.message);
            }

            return ApiResponse<LaundryActionResult>.Success(data.message, data);
        }

        public ApiResponse<LaundryActionResult> MarkPickedUp(int sessionId, int laundryLogId)
        {
            var data = _repo.MarkPickedUp(sessionId, laundryLogId);

            if (data.status != "S")
            {
                throw new Suvarna.DGMS.Api.Helpers.UserMessageException(string.IsNullOrWhiteSpace(data.message) ? "Failed to mark picked up." : data.message);
            }

            return ApiResponse<LaundryActionResult>.Success(data.message, data);
        }

        public ApiResponse<LaundryActionResult> MarkDelivered(int sessionId, int laundryLogId)
        {
            var data = _repo.MarkDelivered(sessionId, laundryLogId);

            if (data.status != "S")
            {
                throw new Suvarna.DGMS.Api.Helpers.UserMessageException(string.IsNullOrWhiteSpace(data.message) ? "Failed to mark delivered." : data.message);
            }

            return ApiResponse<LaundryActionResult>.Success(data.message, data);
        }

        public ApiResponse<LaundryActionResult> Cancel(CancelLaundryLogRequest request)
        {
            var data = _repo.Cancel(request.sessionId, request.laundryLogId, request.reason);

            if (data.status != "S")
            {
                throw new Suvarna.DGMS.Api.Helpers.UserMessageException(string.IsNullOrWhiteSpace(data.message) ? "Failed to cancel entry." : data.message);
            }

            return ApiResponse<LaundryActionResult>.Success(data.message, data);
        }

        // ==========================================
        // REVERSE CHARGE — NEW
        // ==========================================

        public ApiResponse<LaundryActionResult> ReverseCharge(int sessionId, int laundryLogId, string? reason)
        {
            var data = _repo.ReverseCharge(sessionId, laundryLogId, reason);

            if (data.status != "S")
            {
                throw new Suvarna.DGMS.Api.Helpers.UserMessageException(string.IsNullOrWhiteSpace(data.message) ? "Failed to reverse charge." : data.message);
            }

            return ApiResponse<LaundryActionResult>.Success(data.message, data);
        }


       

        public ApiResponse<LaundryReturnPreview> GetReturnPreview(int sessionId, int laundryLogId)
        {
            var data = _repo.GetReturnPreview(sessionId, laundryLogId)
                ?? throw new Suvarna.DGMS.Api.Helpers.UserMessageException("Laundry entry not found.");

            return ApiResponse<LaundryReturnPreview>.Success("Return preview fetched successfully.", data);
        }

        public ApiResponse<LaundryReturnResult> ReturnWithPayment(LaundryReturnRequest request)
        {
            if (request.laundryLogId <= 0) throw new Suvarna.DGMS.Api.Helpers.UserMessageException("Laundry entry is required.");
            if (string.IsNullOrWhiteSpace(request.paymentMode)) throw new Suvarna.DGMS.Api.Helpers.UserMessageException("Payment mode is required.");
            if (request.paymentAmount <= 0) throw new Suvarna.DGMS.Api.Helpers.UserMessageException("Payment amount is required.");
            if (!string.IsNullOrWhiteSpace(request.paymentReference) && request.paymentReference.Trim().Length > 50)
                throw new Suvarna.DGMS.Api.Helpers.UserMessageException("Reference number cannot exceed 50 characters.");
            if (!string.IsNullOrWhiteSpace(request.cardLast4) &&
                (request.cardLast4.Length != 4 || !request.cardLast4.All(char.IsDigit)))
                throw new Suvarna.DGMS.Api.Helpers.UserMessageException("Card last 4 digits must be exactly 4 numbers.");

            // NEW: card paid on the POS machine -> RRN and card last 4 come from the verified POS payment
            var sid = _session.Require(request.sessionId);
            var pos = _pos.ClaimForPayment(sid, request.posTxnId, "LAUNDRY", request.laundryLogId, request.paymentAmount, request.paymentMode);
            if (pos != null)
            {
                request.paymentReference = pos.Rrn;
                request.cardLast4 = pos.CardLast4;
                request.upiVpa = null;
            }

            LaundryReturnResult data;
            try
            {
                data = _repo.ReturnWithPayment(request);
            }
            catch
            {
                _pos.SafeRelease(sid, pos);
                throw;
            }

            if (data.status != "S")
            {
                _pos.SafeRelease(sid, pos);
                throw new Suvarna.DGMS.Api.Helpers.UserMessageException(string.IsNullOrWhiteSpace(data.message) ? "Failed to return laundry." : data.message);
            }

            var posWarning = _pos.LinkAfterSave(sid, pos, null, null, data.paymentId);
            if (posWarning != null) data.message = (data.message ?? "") + " | " + posWarning;

            return ApiResponse<LaundryReturnResult>.Success(data.message, data);
        }
    }
}
