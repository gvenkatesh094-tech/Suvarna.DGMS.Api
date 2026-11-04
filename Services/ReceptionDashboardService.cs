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
    
        // ===== 03-Nov =====

        public ApiResponse<ReceptionRoomDetail> GetRoomDetail(int sessionId, int roomId)
            => ApiResponse<ReceptionRoomDetail>.Success("Room details fetched successfully.", _repo.GetRoomDetail(sessionId, roomId));

        public ApiResponse<List<ReceptionTodayItem>> GetToday(int sessionId)
            => ApiResponse<List<ReceptionTodayItem>>.Success("Today's arrivals and departures fetched successfully.", _repo.GetToday(sessionId));

        public ApiResponse<List<ReceptionSearchItem>> Search(int sessionId, string? q)
        {
            var text = (q ?? "").Trim();
            if (text.Length < 2)
                return ApiResponse<List<ReceptionSearchItem>>.Success("Type at least 2 letters.", new List<ReceptionSearchItem>());
            var data = _repo.Search(sessionId, text.Length > 100 ? text[..100] : text);
            return ApiResponse<List<ReceptionSearchItem>>.Success(data.Count == 0 ? "No guest found." : "Guests found.", data);
        }

        public ApiResponse<List<ReceptionRoomSuggestion>> SuggestRooms(int sessionId, int bookingId)
        {
            var data = _repo.SuggestRooms(sessionId, bookingId);
            return ApiResponse<List<ReceptionRoomSuggestion>>.Success(
                data.Count == 0 ? "Reservation not found or no room of its type." : "Rooms fetched successfully.", data);
        }

        public ApiResponse<object> SetHkPriority(ReceptionRoomActionRequest req)
        {
            var (status, message) = _repo.SetHkPriority(req.sessionId, req.roomId);
            return status == "S" ? ApiResponse<object>.Success(message, new { }) : ApiResponse<object>.Fail(message);
        }

        public ApiResponse<ShiftHandover> GetShiftHandover(int sessionId)
            => ApiResponse<ShiftHandover>.Success("Shift handover fetched successfully.", _repo.GetShiftHandover(sessionId));

        public ApiResponse<object> SaveShiftHandover(ShiftHandoverSaveRequest req)
        {
            if (req.notes != null && req.notes.Length > 1000)
                return ApiResponse<object>.Fail("Note is too long (max 1000 characters).");
            var (status, message) = _repo.SaveShiftHandover(req);
            return status == "S" ? ApiResponse<object>.Success(message, new { }) : ApiResponse<object>.Fail(message);
        }
    }
}
