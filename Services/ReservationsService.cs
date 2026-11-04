using Suvarna.DGMS.Api.Models;
using Suvarna.DGMS.Api.Repositories;

namespace Suvarna.DGMS.Api.Services
{
    public class ReservationsService
    {
        private readonly ReservationsRepository _repo;
        private readonly PosPaymentService _pos;          // NEW: POS machine payments
        private readonly Suvarna.DGMS.Api.Helpers.SessionContext _session;

        public ReservationsService(
            ReservationsRepository repo,
            PosPaymentService pos,
            Suvarna.DGMS.Api.Helpers.SessionContext session)
        {
            _repo = repo;
            _pos = pos;
            _session = session;
        }


        // ==========================================
        // CREATE / UPDATE RESERVATION
        // ==========================================

        public ApiResponse<ReservationResult>
            SaveReservation(
                Reservation request)
        {
            // NEW: advance paid by card on the POS machine -> the RRN comes from the verified POS payment
            var sid = _session.Require(request.sessionId);
            var pos = request.advanceAmount > 0
                ? _pos.ClaimForPayment(sid, request.posTxnId, "RESERVATION", request.bookingId ?? 0, request.advanceAmount, request.advanceMode)
                : null;
            if (pos != null) request.advanceRef = pos.Rrn;

            ReservationResult data;
            try
            {
                data = _repo.SaveReservation(request);
            }
            catch
            {
                _pos.SafeRelease(sid, pos);
                throw;
            }

            if (data.status != "S")
            {
                _pos.SafeRelease(sid, pos);
                throw new Suvarna.DGMS.Api.Helpers.UserMessageException(
                    string.IsNullOrWhiteSpace(
                        data.message
                    )
                    ? "Reservation save failed."
                    : data.message
                );
            }


            var posWarning = _pos.LinkAfterSave(sid, pos, data.bookingId, null, null);
            if (posWarning != null) data.message = (data.message ?? "") + " | " + posWarning;

            return ApiResponse<
                ReservationResult
            >.Success(
                data.message,
                data
            );
        }
        // ==========================================
        // GET RESERVATIONS FOR GRID
        // ==========================================

        public ApiResponse<List<ReservationListItem>> GetReservations(int sessionId)
        {
            var data = _repo.GetReservations(sessionId);

            return ApiResponse<List<ReservationListItem>>.Success("Reservations fetched successfully.", data);
        }
        public ApiResponse<ReservationAvailability> CheckAvailability(int sessionId, int propertyId, int accmTypeId, DateTime checkinDt, DateTime checkoutDt)
        {
            var data = _repo.CheckAvailability(sessionId, propertyId, accmTypeId, checkinDt, checkoutDt);
            return ApiResponse<ReservationAvailability>
                .Success(
                    data.isAvailable
                        ? "Rooms are available."
                        : "No rooms available for the selected dates.",
                    data
                );
        }
        // NEW: cancel a reservation
        public ApiResponse<ReservationCancelResult> CancelReservation(ReservationCancelRequest request)
        {
            if (request.bookingId <= 0)
                throw new Suvarna.DGMS.Api.Helpers.UserMessageException("Please select a reservation.");

            if (string.IsNullOrWhiteSpace(request.cancelRemarks))
                throw new Suvarna.DGMS.Api.Helpers.UserMessageException("Cancellation reason is required.");

            if (request.cancelChargeAmount < 0)
                throw new Suvarna.DGMS.Api.Helpers.UserMessageException("Cancellation charge cannot be negative.");

            var data = _repo.CancelReservation(request);

            if (data.status != "S")
                throw new Suvarna.DGMS.Api.Helpers.UserMessageException(string.IsNullOrWhiteSpace(data.message) ? "Reservation cancel failed." : data.message);

            return ApiResponse<ReservationCancelResult>.Success(data.message, data);
        }

        // 03-Nov: returning guest + duplicate reservation check for the New Reservation form
        public ApiResponse<ReservationGuestCheck> GetGuestCheck(int sessionId, string mobileNo, DateTime? checkinDt, DateTime? checkoutDt, int bookingId)
        {
            var mobile = (mobileNo ?? "").Trim();
            if (!System.Text.RegularExpressions.Regex.IsMatch(mobile, "^[0-9]{10}$"))
                throw new Suvarna.DGMS.Api.Helpers.UserMessageException("Enter a valid 10-digit mobile number.");

            var data = _repo.GetGuestCheck(sessionId, mobile, checkinDt, checkoutDt, bookingId);
            return ApiResponse<ReservationGuestCheck>.Success("Guest check done.", data);
        }

        public ApiResponse<MarkNoShowResult> MarkNoShowReservations(int sessionId, int? graceHours)
        {
            var data = _repo.MarkNoShowReservations(sessionId, graceHours);

            if (data.status != "S")
            {
                throw new Suvarna.DGMS.Api.Helpers.UserMessageException(string.IsNullOrWhiteSpace(data.message) ? "Failed to mark no-show reservations." : data.message);
            }

            return ApiResponse<MarkNoShowResult>.Success(data.message, data);
        }
    }
}