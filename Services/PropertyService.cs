using Suvarna.DGMS.Api.Models;
using Suvarna.DGMS.Api.Repositories;

namespace Suvarna.DGMS.Api.Services
{
    public class PropertyService
    {
        private readonly PropertyRepository _repo;

        public PropertyService(PropertyRepository repo)
        {
            _repo = repo;
        }
        public ApiResponse<string> GenerateCode(GenerateCodeRequest request)
        {
            string propertyCode = _repo.GenerateCodeProperty(request);

            return ApiResponse<string>.Success(
                "Property Code generated successfully.",
                propertyCode
            );
        }

        public ApiResponse<string> inUpdService(Property _pr)
        {
           
            _repo.SaveProperty(_pr);
           
            string message = _pr.mode switch
            {
                "I" => "Floor created successfully.",
                "U" => "Floor updated successfully.",
                "D" => "Floor deleted successfully.",
                _ => "Operation completed successfully."
            };

            return ApiResponse<string>.Success(message, "OK");
            //return ApiResponse<string>.Success("Property created successfully.", "OK");
        }
       
        public ApiResponse<List<Property>> GetAll()
        {
            var data = _repo.GetAllProperties();

            return ApiResponse<List<Property>>.Success(
                "Property list fetched successfully.",
                data
            );
        }
    }
}
