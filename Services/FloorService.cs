using Suvarna.DGMS.Api.Models;
using Suvarna.DGMS.Api.Repositories;

namespace Suvarna.DGMS.Api.Services
{
    public class FloorService
    {

        private readonly FloorRepository _repo;

        private readonly Suvarna.DGMS.Api.Helpers.PropertyScope _scope;   // NEW: login property

        public FloorService(FloorRepository repo, Suvarna.DGMS.Api.Helpers.PropertyScope scope)
        {
            _repo = repo;
            _scope = scope;
        }
        public ApiResponse<string> inUpdService(Floor floor)
        {


            _repo.SaveFloor(floor);

            
            string message = floor.mode switch
            {
                "I" => "Floor created successfully.",
                "U" => "Floor updated successfully.",
                "D" => "Floor deleted successfully.",
                _ => "Operation completed successfully."
            };

            return ApiResponse<string>.Success(message, "OK");
        }

       

        public ApiResponse<List<Floor>> GetAll()
        {
            // NEW: only floors of the property chosen at login
            var list = _scope.Filter(_repo.GetAllFloors(), f => f.propertyId);

            return ApiResponse<List<Floor>>.Success(
                "Floor List",
                list);
        }

       
    }
}
