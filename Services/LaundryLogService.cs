using Suvarna.DGMS.Api.Models;
using Suvarna.DGMS.Api.Repositories;

namespace Suvarna.DGMS.Api.Services
{
    public class LaundryLogService
    {
        private readonly LaundryLogRepository _repo;

        public LaundryLogService(LaundryLogRepository repo)
        {
            _repo = repo;
        }

        public ApiResponse<List<LaundryLogRow>> GetEntries(int sessionId, string? status, string? type)
        {
            var data = _repo.GetEntries(sessionId, status, type);
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
                throw new Exception("At least one laundry item is required.");
            }

            var data = _repo.CreateLog(request);

            if (data.status != "S")
            {
                throw new Exception(string.IsNullOrWhiteSpace(data.message) ? "Failed to log laundry pickup." : data.message);
            }

            return ApiResponse<LaundryActionResult>.Success(data.message, data);
        }

        public ApiResponse<LaundryActionResult> MarkPickedUp(int sessionId, int laundryLogId)
        {
            var data = _repo.MarkPickedUp(sessionId, laundryLogId);

            if (data.status != "S")
            {
                throw new Exception(string.IsNullOrWhiteSpace(data.message) ? "Failed to mark picked up." : data.message);
            }

            return ApiResponse<LaundryActionResult>.Success(data.message, data);
        }

        public ApiResponse<LaundryActionResult> MarkDelivered(int sessionId, int laundryLogId)
        {
            var data = _repo.MarkDelivered(sessionId, laundryLogId);

            if (data.status != "S")
            {
                throw new Exception(string.IsNullOrWhiteSpace(data.message) ? "Failed to mark delivered." : data.message);
            }

            return ApiResponse<LaundryActionResult>.Success(data.message, data);
        }

        public ApiResponse<LaundryActionResult> Cancel(CancelLaundryLogRequest request)
        {
            var data = _repo.Cancel(request.sessionId, request.laundryLogId, request.reason);

            if (data.status != "S")
            {
                throw new Exception(string.IsNullOrWhiteSpace(data.message) ? "Failed to cancel entry." : data.message);
            }

            return ApiResponse<LaundryActionResult>.Success(data.message, data);
        }
    }
}
