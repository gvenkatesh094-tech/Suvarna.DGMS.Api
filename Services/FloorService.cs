using Suvarna.DGMS.Api.Models;
using Suvarna.DGMS.Api.Repositories;

namespace Suvarna.DGMS.Api.Services
{
    public class FloorService
    {

        private readonly FloorRepository _repo;

        public FloorService(FloorRepository repo)
        {
            _repo = repo;
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
            var list = _repo.GetAllFloors();

            return ApiResponse<List<Floor>>.Success(
                "Floor List",
                list);
        }

       
    }
}
