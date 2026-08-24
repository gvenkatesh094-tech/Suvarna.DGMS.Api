using Suvarna.DGMS.Api.Models;
using Suvarna.DGMS.Api.Repositories;

namespace Suvarna.DGMS.Api.Services
{
    public class RoomTransferService
    {
        private readonly RoomTransferRepository _repo;

        public RoomTransferService(RoomTransferRepository repo)
        {
            _repo = repo;
        }

        public ApiResponse<List<TransferBookingLookup>> GetBookings(int sessionId, int? propertyId)
        {
            var data = _repo.GetBookings(sessionId, propertyId);

            return ApiResponse<List<TransferBookingLookup>>.Success(
                "Bookings fetched successfully.",
                data
            );
        }

        public ApiResponse<List<TransferAvailableRoom>> GetAvailableRooms(int sessionId, int? propertyId, int? excludeRoomId)
        {
            var data = _repo.GetAvailableRooms(sessionId, propertyId, excludeRoomId);

            return ApiResponse<List<TransferAvailableRoom>>.Success(
                "Available rooms fetched successfully.",
                data
            );
        }

        public ApiResponse<TransferResult> ConfirmTransfer(ConfirmTransferRequest request)
        {
            var data = _repo.ConfirmTransfer(request);

            if (data.status != "S")
            {
                throw new Exception(string.IsNullOrWhiteSpace(data.message) ? "Failed to confirm room transfer." : data.message);
            }

            return ApiResponse<TransferResult>.Success(data.message, data);
        }
    }
}
