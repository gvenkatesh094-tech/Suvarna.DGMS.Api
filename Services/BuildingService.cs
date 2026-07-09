using Suvarna.DGMS.Api.Models;
using Suvarna.DGMS.Api.Repositories;

namespace Suvarna.DGMS.Api.Services
{
    public class BuildingService
    {
        private readonly BuildingRepository _repo;

        public BuildingService(BuildingRepository repo)
        {
            _repo = repo;
        }

        public ApiResponse<string> inUpdService(Building building)
        {


            _repo.SaveBuilding(building);
            string message = building.mode switch
            {
                "I" => "Floor created successfully.",
                "U" => "Floor updated successfully.",
                "D" => "Floor deleted successfully.",
                _ => "Operation completed successfully."
            };

            return ApiResponse<string>.Success(message, "OK");
            
        }
        public ApiResponse<string> Create(Building building)
        {
            

            _repo.InsertBuilding(building);

            return ApiResponse<string>.Success("Building created successfully.", "OK");
        }

        public ApiResponse<string> Update(Building building)
        {
           

            _repo.UpdateBuilding(building);

            return ApiResponse<string>.Success("Building updated successfully.", "OK");
        }
        public ApiResponse<string> Delete(DeleteBuildingRequest building)
        {
            _repo.DeleteBuilding(building);

            return ApiResponse<string>.Success(
                "Building deleted successfully.",
                "OK");
        }

        public ApiResponse<List<Building>> GetAll()
        {
            var list = _repo.GetAllBuildings();

            return ApiResponse<List<Building>>.Success(
                "Building List",
                list);
        }

        public ApiResponse<Building> GetById(int id)
        {
            var data = _repo.GetBuildingById(id);

            if (data == null)
                return ApiResponse<Building>.Fail("Building not found.");

            return ApiResponse<Building>.Success(
                "Building Details",
                data);
        }
    }
}