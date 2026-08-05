using Suvarna.DGMS.Api.Models;
using Suvarna.DGMS.Api.Repositories;

namespace Suvarna.DGMS.Api.Services
{
    public class BedService
    {
        private readonly BedRepository _repo;

        public BedService(BedRepository repo)
        {
            _repo = repo;
        }
        public ApiResponse<string> Create(Bed bed)
        {
           
            if (string.IsNullOrWhiteSpace(bed.bedNo))
                return ApiResponse<string>.Fail("bedNo Number is required.");

            if (string.IsNullOrWhiteSpace(bed.bedType))
                return ApiResponse<string>.Fail("bedType is required.");

            // Use the mode sent from Angular
            string msg = _repo.Save(bed);

            return ApiResponse<string>.Success(msg, "OK");
        }

        public ApiResponse<string> Update(Bed bed)
        {

            if (string.IsNullOrWhiteSpace(bed.bedNo))
                return ApiResponse<string>.Fail("bedNo Number is required.");

            if (string.IsNullOrWhiteSpace(bed.bedType))
                return ApiResponse<string>.Fail("bedType is required.");

            // Use the mode sent from Angular
            string msg = _repo.Save(bed);

            return ApiResponse<string>.Success(msg, "OK");
        }
        public ApiResponse<List<Bed>> GetAll(int sessionId, int locationId)
        {
            var list = _repo.GetAllBeds( sessionId,  locationId);

            return ApiResponse<List<Bed>>.Success(
                "Bed List",
                list);
        }

        public ApiResponse<Bed> GetById(int id)
        {
            var bed = _repo.GetBedById(id);

            if (bed == null)
                return ApiResponse<Bed>.Fail("Bed not found.");

            return ApiResponse<Bed>.Success(
                "Bed Details",
                bed);
        }

        public ApiResponse<string> UpdateStatus(Bed bed)
        {
            if (bed.bedId <= 0)
                return ApiResponse<string>.Fail("Bed Id is required.");

            if (bed.sessionId<=0)
                return ApiResponse<string>.Fail("Session Id is required.");

            if (string.IsNullOrWhiteSpace(bed.bedStatus))
                return ApiResponse<string>.Fail("Bed Status is required.");

            _repo.UpdateStatus(bed);

            return ApiResponse<string>.Success(
                "Bed status updated successfully.",
                "OK");
        }
    }
}