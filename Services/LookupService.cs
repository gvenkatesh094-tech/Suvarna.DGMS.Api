using Suvarna.DGMS.Api.Models;
using Suvarna.DGMS.Api.Repositories;

namespace Suvarna.DGMS.Api.Services
{
    public class LookupService
    {
        private readonly LookupRepository _repo;

        public LookupService(LookupRepository repo)
        {
            _repo = repo;
        }

        public ApiResponse<List<LookupModel>> GetLookup(string lookupType)
        {
            var data = _repo.GetLookup(lookupType);

            return ApiResponse<List<LookupModel>>.Success(
                "Lookup loaded successfully.",
                data
            );
        }
    }
}