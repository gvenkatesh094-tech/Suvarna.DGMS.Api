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

        public ApiResponse<string> Create(Floor floor)
        {
            if (floor.grpId <= 0)
                return ApiResponse<string>.Fail("Group is required.");

            if (floor.orgId <= 0)
                return ApiResponse<string>.Fail("Organization is required.");

            if (floor.locId <= 0)
                return ApiResponse<string>.Fail("Location is required.");

            if (floor.propertyId <= 0)
                return ApiResponse<string>.Fail("Property is required.");

            if (string.IsNullOrWhiteSpace(floor.floorCode))
                return ApiResponse<string>.Fail("Floor Code is required.");

            if (string.IsNullOrWhiteSpace(floor.floorName))
                return ApiResponse<string>.Fail("Floor Name is required.");

            if (floor.floorSeq <= 0)
                return ApiResponse<string>.Fail("Total Floors should be greater than zero.");

            _repo.InsertFloor(floor);

            return ApiResponse<string>.Success("Floor created successfully.", "OK");
        }

        public ApiResponse<string> Update(Floor floor)
        {
            if (floor.buildingId <= 0)
                return ApiResponse<string>.Fail("Floor Id is required.");

            if (floor.propertyId <= 0)
                return ApiResponse<string>.Fail("Property is required.");

            if (string.IsNullOrWhiteSpace(floor.floorCode))
                return ApiResponse<string>.Fail("Floor Code is required.");

            if (string.IsNullOrWhiteSpace(floor.floorName))
                return ApiResponse<string>.Fail("Floor Name is required.");

            _repo.UpdateFloor(floor);

            return ApiResponse<string>.Success("Floor updated successfully.", "OK");
        }

        public ApiResponse<List<Floor>> GetAll()
        {
            var list = _repo.GetAllFloors();

            return ApiResponse<List<Floor>>.Success(
                "Floor List",
                list);
        }

        public ApiResponse<Floor> GetById(int id)
        {
            var data = _repo.GetFloorById(id);

            if (data == null)
                return ApiResponse<Floor>.Fail("Floor not found.");

            return ApiResponse<Floor>.Success(
                "Floor Details",
                data);
        }
    }
}
