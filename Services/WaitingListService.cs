using Suvarna.DGMS.Api.Models;
using Suvarna.DGMS.Api.Repositories;

namespace Suvarna.DGMS.Api.Services
{
    public class WaitingListService
    {
        private readonly WaitingListRepository _repo;

        public WaitingListService(WaitingListRepository repo)
        {
            _repo = repo;
        }

        public ApiResponse<List<WaitlistEntryRow>> GetWaitlist(int sessionId)
        {
            var data = _repo.GetWaitlist(sessionId);
            return ApiResponse<List<WaitlistEntryRow>>.Success("Waiting list fetched successfully.", data);
        }

        public ApiResponse<WaitlistActionResult> AddToWaitlist(AddWaitlistRequest request)
        {
            var data = _repo.AddToWaitlist(request);

            if (data.status != "S")
            {
                throw new Exception(string.IsNullOrWhiteSpace(data.message) ? "Failed to add to waiting list." : data.message);
            }

            return ApiResponse<WaitlistActionResult>.Success(data.message, data);
        }

        public ApiResponse<WaitlistActionResult> Notify(int sessionId, int waitlistId)
        {
            var data = _repo.Notify(sessionId, waitlistId);

            if (data.status != "S")
            {
                throw new Exception(string.IsNullOrWhiteSpace(data.message) ? "Failed to notify guest." : data.message);
            }

            return ApiResponse<WaitlistActionResult>.Success(data.message, data);
        }

        public ApiResponse<WaitlistActionResult> Remove(int sessionId, int waitlistId)
        {
            var data = _repo.Remove(sessionId, waitlistId);

            if (data.status != "S")
            {
                throw new Exception(string.IsNullOrWhiteSpace(data.message) ? "Failed to remove entry." : data.message);
            }

            return ApiResponse<WaitlistActionResult>.Success(data.message, data);
        }
    }
}
