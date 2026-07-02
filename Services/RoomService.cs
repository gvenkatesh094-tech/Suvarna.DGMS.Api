using Suvarna.DGMS.Api.Models;
using Suvarna.DGMS.Api.Repositories;

namespace Suvarna.DGMS.Api.Services
{
    public class RoomService
    {
        private readonly RoomRepository _repo;

        public RoomService(RoomRepository repo)
        {
            _repo = repo;
        }

        public ApiResponse<string> Create(Room room)
        {
            if (room.propertyId <= 0)
                return ApiResponse<string>.Fail("Property is required.");

            if (room.buildingId <= 0)
                return ApiResponse<string>.Fail("Building is required.");

            if (room.floorId <= 0)
                return ApiResponse<string>.Fail("Floor is required.");

            if (room.accmTypeId <= 0)
                return ApiResponse<string>.Fail("Accommodation Type is required.");

            if (string.IsNullOrWhiteSpace(room.roomNo))
                return ApiResponse<string>.Fail("Room Number is required.");

            if (room.totalBeds <= 0)
                return ApiResponse<string>.Fail("Total Beds should be greater than zero.");

            // Use the mode sent from Angular
            string msg = _repo.Save(room, room.mode);

            return ApiResponse<string>.Success(msg, "OK");
        }

        public ApiResponse<string> Update(Room room)
        {
            if (room.roomId <= 0)
                return ApiResponse<string>.Fail("Room Id is required.");


            if (room.propertyId <= 0)
                return ApiResponse<string>.Fail("Property is required.");

            if (room.buildingId <= 0)
                return ApiResponse<string>.Fail("Building is required.");

            if (room.floorId <= 0)
                return ApiResponse<string>.Fail("Floor is required.");

            if (room.accmTypeId <= 0)
                return ApiResponse<string>.Fail("Accommodation Type is required.");

            if (string.IsNullOrWhiteSpace(room.roomNo))
                return ApiResponse<string>.Fail("Room Number is required.");

            string msg = _repo.Save(room, "U");

            return ApiResponse<string>.Success(msg, "OK");
        }

        public ApiResponse<List<Room>> GetAll()
        {
            var list = _repo.GetAllRooms();

            return ApiResponse<List<Room>>.Success(
                "Room List",
                list);
        }
        public ApiResponse<string> GenroomCode(GenerateCodeRequest request)
        {
            string propertyCode = _repo.GenroomCode(request);

            return ApiResponse<string>.Success(
                "Room Code generated successfully.",
                propertyCode
            );
        }
        //public ApiResponse<Room> GetById(int id)
        //{
        //    var room = _repo.GetRoomById(id);

        //    if (room == null)
        //        return ApiResponse<Room>.Fail("Room not found.");

        //    return ApiResponse<Room>.Success(
        //        "Room Details",
        //        room);
        //}
    }
}