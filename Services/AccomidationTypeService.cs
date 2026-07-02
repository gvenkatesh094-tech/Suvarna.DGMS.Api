using Suvarna.DGMS.Api.Models;
using Suvarna.DGMS.Api.Repositories;

namespace Suvarna.DGMS.Api.Services
{
    public class AccomidationTypeService
    {
        private readonly AccomidationTypeRepository _repo;

        public AccomidationTypeService(AccomidationTypeRepository repo)
        {
            _repo = repo;
        }

        public ApiResponse<List<AccommodationType>> GetAccommodationTypes()
        {
            var list = _repo.GetAccommodationTypes();

            return ApiResponse<List<AccommodationType>>.Success(
                "Accommodation Type List",
                list);
        }
    }
}