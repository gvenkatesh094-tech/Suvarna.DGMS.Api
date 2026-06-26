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

        public ApiResponse<string> Create(Building building)
        {
            if (building.grpId <= 0)
                return ApiResponse<string>.Fail("Group is required.");

            if (building.orgId <= 0)
                return ApiResponse<string>.Fail("Organization is required.");

            if (building.locId <= 0)
                return ApiResponse<string>.Fail("Location is required.");

            if (building.propertyId <= 0)
                return ApiResponse<string>.Fail("Property is required.");

            if (string.IsNullOrWhiteSpace(building.buildingCode))
                return ApiResponse<string>.Fail("Building Code is required.");

            if (string.IsNullOrWhiteSpace(building.buildingName))
                return ApiResponse<string>.Fail("Building Name is required.");

            if (building.totalFloors <= 0)
                return ApiResponse<string>.Fail("Total Floors should be greater than zero.");

            _repo.InsertBuilding(building);

            return ApiResponse<string>.Success("Building created successfully.", "OK");
        }

        public ApiResponse<string> Update(Building building)
        {
            if (building.buildingId <= 0)
                return ApiResponse<string>.Fail("Building Id is required.");

            if (building.propertyId <= 0)
                return ApiResponse<string>.Fail("Property is required.");

            if (string.IsNullOrWhiteSpace(building.buildingCode))
                return ApiResponse<string>.Fail("Building Code is required.");

            if (string.IsNullOrWhiteSpace(building.buildingName))
                return ApiResponse<string>.Fail("Building Name is required.");

            _repo.UpdateBuilding(building);

            return ApiResponse<string>.Success("Building updated successfully.", "OK");
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