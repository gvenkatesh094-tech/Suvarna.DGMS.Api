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

        public ApiResponse<CheckinResult> ConfirmCheckin(           Checkin request)
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

        // ==========================================
        // SERVER VALIDATION
        // ==========================================

        private static void ValidateCheckin(
            Checkin request)
        {
            if (request == null)
                throw new Exception(
                    "Check-in request is required."
                );

            if (string.IsNullOrWhiteSpace(
                request.sessionId))
            {
                throw new Exception(
                    "Session Id is required."
                );
            }

            if (request.propertyId <= 0)
                throw new Exception(
                    "Property is required."
                );

            if (request.roomId <= 0)
                throw new Exception(
                    "Room is required."
                );

            if (request.tariffId <= 0)
                throw new Exception(
                    "Tariff is required."
                );

            if (request.occCatId <= 0)
                throw new Exception(
                    "Occupant category is required."
                );

            if (string.IsNullOrWhiteSpace(
                request.guestName))
            {
                throw new Exception(
                    "Guest name is required."
                );
            }

            if (request.guestName.Trim().Length < 2)
            {
                throw new Exception(
                    "Guest name must contain at least 2 characters."
                );
            }

            if (string.IsNullOrWhiteSpace(
                request.mobileNo))
            {
                throw new Exception(
                    "Mobile number is required."
                );
            }

            if (!System.Text.RegularExpressions.Regex.IsMatch(
                request.mobileNo,
                @"^[6-9][0-9]{9}$"))
            {
                throw new Exception(
                    "Enter valid 10 digit mobile number."
                );
            }

            if (!string.IsNullOrWhiteSpace(
                    request.emailId)
                &&
                !System.Text.RegularExpressions.Regex.IsMatch(
                    request.emailId,
                    @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                throw new Exception(
                    "Enter valid email address."
                );
            }

            if (string.IsNullOrWhiteSpace(
                request.idTypeCd))
            {
                throw new Exception(
                    "ID type is required."
                );
            }

            if (string.IsNullOrWhiteSpace(
                request.idNumberMasked))
            {
                throw new Exception(
                    "ID number is required."
                );
            }

            if (request.expectedCheckoutDt
                <= DateTime.UtcNow)
            {
                throw new Exception(
                    "Expected checkout must be greater than check-in date."
                );
            }

            if (request.depositAmount < 0)
            {
                throw new Exception(
                    "Deposit amount cannot be negative."
                );
            }

            if (request.depositAmount > 0
                &&
                string.IsNullOrWhiteSpace(
                    request.depositPaymentMode))
            {
                throw new Exception(
                    "Deposit payment mode is required."
                );
            }
        }
    }
}