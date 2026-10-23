using Suvarna.DGMS.Api.Models;
using Suvarna.DGMS.Api.Repositories;

namespace Suvarna.DGMS.Api.Services
{
    public class PropertyService
    {
        private readonly PropertyRepository _repo;

        private readonly Suvarna.DGMS.Api.Helpers.PropertyScope _scope;   // NEW: login property

        public PropertyService(PropertyRepository repo, Suvarna.DGMS.Api.Helpers.PropertyScope scope)
        {
            _repo = repo;
            _scope = scope;
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
       
        public ApiResponse<List<Property>> GetAll(int sessionId, int locationId)
        {
            // NEW: only the property chosen at login (all when "All Properties" / no property chosen)
            var data = _scope.Filter(_repo.GetAllProperties( sessionId,  locationId), p => p.propertyId);

            return ApiResponse<List<Property>>.Success(
                "Property list fetched successfully.",
                data
            );
        }
    }
}
