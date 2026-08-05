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
        public ApiResponse<string> inUpdService(AccomidationType accomidation)
        {



            _repo.SaveAccommodation(accomidation);

            string message = accomidation.mode switch
            {
                "I" => "Accomidation created successfully.",
                "U" => "Accomidation updated successfully.",
                "D" => "Accomidation deleted successfully.",
                _ => "Operation completed successfully."
            };

            return ApiResponse<string>.Success(message, "OK");
        }
        public ApiResponse<List<AccomidationType>> Getall(int sessionId, int locationId)
        {
            var list = _repo.GetAccommodationTypes(sessionId, locationId);

            return ApiResponse<List<AccomidationType>>.Success(
                "Accommodation Type List",
                list);
        }
        public ApiResponse<List<AccommodationCategory>> Getcaterigyall(int sessionId, int locationId)
        {
            var list = _repo.GetAccommodationCategories(sessionId, locationId);

            return ApiResponse<List<AccommodationCategory>>.Success(
                "Accommodation Type List",
                list);
        }
    }
}