using Suvarna.DGMS.Api.Models;
using Suvarna.DGMS.Api.Repositories;

namespace Suvarna.DGMS.Api.Services
{
    public class ReceptionDashboardService
    {
        private readonly ReceptionDashboardRepository _repo;

        public ReceptionDashboardService(ReceptionDashboardRepository repo)
        {
            _repo = repo;
        }

        public ApiResponse<ReceptionDashboardSummary> GetSummary(int sessionId)
        {
            var data = _repo.GetSummary(sessionId);
            return ApiResponse<ReceptionDashboardSummary>.Success("Dashboard summary fetched successfully.", data);
        }

        public ApiResponse<List<ReceptionActiveBooking>> GetActiveBookings(int sessionId)
        {
            var data = _repo.GetActiveBookings(sessionId);
            return ApiResponse<List<ReceptionActiveBooking>>.Success(
                data.Count == 0 ? "No active bookings right now." : "Active bookings fetched successfully.",
                data
            );
        }

        public ApiResponse<List<ReceptionRoomStatus>> GetRoomStatus(int sessionId)
        {
            var data = _repo.GetRoomStatus(sessionId);
            return ApiResponse<List<ReceptionRoomStatus>>.Success(
                data.Count == 0 ? "No rooms configured." : "Room status fetched successfully.",
                data
            );
        }

        public ApiResponse<List<ReceptionAlert>> GetAlerts(int sessionId)
        {
            var data = _repo.GetAlerts(sessionId);
            return ApiResponse<List<ReceptionAlert>>.Success(
                data.Count == 0 ? "No active alerts." : "Alerts fetched successfully.",
                data
            );
        }

        public ApiResponse<List<ReceptionRevenueItem>> GetRevenue(int sessionId)
        {
            var data = _repo.GetRevenue(sessionId);
            return ApiResponse<List<ReceptionRevenueItem>>.Success("Revenue fetched successfully.", data);
        }
    }
}
