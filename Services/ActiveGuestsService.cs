using Suvarna.DGMS.Api.Models;
using Suvarna.DGMS.Api.Repositories;

namespace Suvarna.DGMS.Api.Services
{
    public class ActiveGuestsService
    {
        private readonly ActiveGuestsRepository _repo;

        public ActiveGuestsService(ActiveGuestsRepository repo)
        {
            _repo = repo;
        }

        public ApiResponse<List<ActiveGuestRow>> GetActiveGuests(int sessionId, int? propertyId)
        {
            var data = _repo.GetActiveGuests(sessionId, propertyId);

            return ApiResponse<List<ActiveGuestRow>>.Success(
                "Active guests fetched successfully.",
                data
            );
        }
    }
}
