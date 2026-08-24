using Suvarna.DGMS.Api.Models;
using Suvarna.DGMS.Api.Repositories;

namespace Suvarna.DGMS.Api.Services
{
    public class ReservationsService
    {
        private readonly ReservationsRepository _repo;

        public ReservationsService(
            ReservationsRepository repo)
        {
            _repo = repo;
        }


        // ==========================================
        // CREATE / UPDATE RESERVATION
        // ==========================================

        public ApiResponse<ReservationResult>
            SaveReservation(
                Reservation request)
        {
            var data =
                _repo.SaveReservation(
                    request
                );

            if (data.status != "S")
            {
                throw new Exception(
                    string.IsNullOrWhiteSpace(
                        data.message
                    )
                    ? "Reservation save failed."
                    : data.message
                );
            }


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
        public ApiResponse<MarkNoShowResult> MarkNoShowReservations(int sessionId, int? graceHours)
        {
            var data = _repo.MarkNoShowReservations(sessionId, graceHours);

            if (data.status != "S")
            {
                throw new Exception(string.IsNullOrWhiteSpace(data.message) ? "Failed to mark no-show reservations." : data.message);
            }

            return ApiResponse<MarkNoShowResult>.Success(data.message, data);
        }
    }
}