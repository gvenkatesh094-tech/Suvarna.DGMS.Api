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

        public ApiResponse<List<CheckinRoom>> GetRooms(int sessionId, int? locationId, int? occCatId,DateTime checkinDt  , DateTime checkOutDt, int? bookinId, int? accmTypeId = null)
        {
            var data = _repo.GetRooms(
                sessionId,
                locationId,
                occCatId,
                checkinDt,  checkOutDt, bookinId, accmTypeId
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
        int sessionId,
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

        public ApiResponse<CheckinResult> ConfirmCheckin(Checkin request, IFormFile? idFile, IFormFile? photoFile)
        {
            //ValidateCheckin(request);

            var data = _repo.ConfirmCheckin(request, idFile, photoFile);

            if (data.status != "S")
            {
                throw new Exception(
                    string.IsNullOrWhiteSpace(data.message)
                        ? "Check-in failed."
                        : data.message
                );
            }

            return ApiResponse<CheckinResult>.Success(data.message, data);
        }
        public ApiResponse<PatientInfo?> GetPatientInfo(string umrNo)
        {
            var data =
                _repo.GetPatientInfo(umrNo);

            if (data == null)
            {
                return ApiResponse<PatientInfo?>.Success(
                    "Patient not found.",
                    null
                );
            }

            return ApiResponse<PatientInfo?>.Success(
                "Patient information fetched successfully.",
                data
            );
        }


    }
}