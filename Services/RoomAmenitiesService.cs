using Suvarna.DGMS.Api.Models;
using Suvarna.DGMS.Api.Repositories;

namespace Suvarna.DGMS.Api.Services
{
    public class RoomAmenitiesService
    {
        private readonly RoomAmenitiesRepository _repo;

        public RoomAmenitiesService(
            RoomAmenitiesRepository repo)
        {
            _repo = repo;
        }

        public ApiResponse<string> InUpdRoomAmenity(
            RoomAmenity amenity)
        {
            _repo.SaveRoomAmenity(amenity);

            return ApiResponse<string>.Success(
                amenity.mode == "I"
                    ? "Room Amenity created successfully."
                    : amenity.mode == "U"
                        ? "Room Amenity updated successfully."
                        : "Room Amenity deleted successfully.",
                "OK");
        }

        public ApiResponse<List<RoomAmenity>>
            GetAll()
        {
            var list = _repo.GetRoomAmenities();

            return ApiResponse<List<RoomAmenity>>
                .Success(
                    "Room Amenities List",
                    list);
        }
        public ApiResponse<List<string>> GetCategories()
        {
            var list = _repo.GetAmenityCategories();

            return ApiResponse<List<string>>.Success(
                "Amenity Categories",
                list);
        }
    }
}