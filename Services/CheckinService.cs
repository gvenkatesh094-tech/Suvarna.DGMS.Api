using Suvarna.DGMS.Api.Models;
using Suvarna.DGMS.Api.Repositories;

namespace Suvarna.DGMS.Api.Services
{
    public class CheckinService
    {
        private readonly CheckinRepository _repo;

        public CheckinService(
            CheckinRepository repo)
        {
            _repo = repo;
        }

        // ==========================================
        // GET ROOMS
        // ==========================================

        public ApiResponse<List<CheckinRoom>> GetRooms(
            string sessionId,
            int? propertyId,
            int? buildingId,
            int? floorId,
            int? accmTypeId,
            string? roomStatus,
            string? isAc,
            string? isAccessible)
        {
            var data = _repo.GetRooms(
                sessionId,
                propertyId,
                buildingId,
                floorId,
                accmTypeId,
                roomStatus,
                isAc,
                isAccessible
            );

            return ApiResponse<List<CheckinRoom>>.Success(
                "Available rooms fetched successfully.",
                data
            );
        }

        // ==========================================
        // GET TARIFF
        // ==========================================

        public ApiResponse<List<CheckinTariff>> GetTariff(
        string sessionId,
        int propertyId,
        int accmTypeId,
        int occCatId,
        DateTime checkinDt,
        string? billingModel)
        {
            var data = _repo.GetTariff(
                sessionId,
                propertyId,
                accmTypeId,
                occCatId,
                checkinDt,
                billingModel
            );

            return ApiResponse<List<CheckinTariff>>.Success(
                data.Count == 0
                    ? "No applicable tariff found."
                    : "Tariff fetched successfully.",
                data
            );
        }

        // ==========================================
        // CONFIRM CHECK-IN
        // ==========================================

        public ApiResponse<CheckinResult> ConfirmCheckin(Checkin request)
        {
            //ValidateCheckin(request);

            var data = _repo.ConfirmCheckin(request);

            if (data.status != "S")
            {
                throw new Exception(
                    string.IsNullOrWhiteSpace(data.message)
                        ? "Check-in failed."
                        : data.message
                );
            }

            return ApiResponse<CheckinResult>.Success(
                data.message,
                data
            );
        }

       
    }
}