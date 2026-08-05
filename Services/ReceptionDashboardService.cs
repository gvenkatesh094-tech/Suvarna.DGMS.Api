using Suvarna.DGMS.Api.Models;
using Suvarna.DGMS.Api.Repositories;

namespace Suvarna.DGMS.Api.Services
{
    public class ReceptionDashboardService
    {
        private readonly ReceptionDashboardRepository _repo;

        public ReceptionDashboardService(
            ReceptionDashboardRepository repo)
        {
            _repo = repo;
        }

        //=========================================================
        // Dashboard Summary
        //=========================================================

        public ApiResponse<ReceptionDashboardSummary> GetDashboardSummary()
        {
            var data = _repo.GetDashboardSummary();

            return ApiResponse<ReceptionDashboardSummary>.Success(
                "Reception dashboard summary fetched successfully.",
                data
            );
        }

        //=========================================================
        // Active Bookings
        //=========================================================

        public ApiResponse<List<ReceptionBooking>> GetActiveBookings()
        {
            var data = _repo.GetActiveBookings();

            return ApiResponse<List<ReceptionBooking>>.Success(
                "Active bookings fetched successfully.",
                data
            );
        }

        //=========================================================
        // Room Status
        //=========================================================

        public ApiResponse<List<ReceptionRoomStatus>> GetRoomStatus()
        {
            var data = _repo.GetRoomStatus();

            return ApiResponse<List<ReceptionRoomStatus>>.Success(
                "Room status fetched successfully.",
                data
            );
        }

        //=========================================================
        // Alerts
        //=========================================================

        public ApiResponse<List<ReceptionAlert>> GetAlerts()
        {
            var data = _repo.GetAlerts();

            return ApiResponse<List<ReceptionAlert>>.Success(
                "Alerts fetched successfully.",
                data
            );
        }

        //=========================================================
        // Revenue
        //=========================================================

        public ApiResponse<List<ReceptionRevenue>> GetRevenue()
        {
            var data = _repo.GetRevenue();

            return ApiResponse<List<ReceptionRevenue>>.Success(
                "Revenue details fetched successfully.",
                data
            );
        }
    }
}