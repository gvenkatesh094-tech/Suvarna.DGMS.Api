using Suvarna.DGMS.Api.Models;
using Suvarna.DGMS.Api.Repositories;

namespace Suvarna.DGMS.Api.Services
{
    public class OccupantGuestService
    {
        private readonly OccupantGuestRepository _repo;

        public OccupantGuestService(OccupantGuestRepository repo)
        {
            _repo = repo;
        }
        public ApiResponse<string> inUpdService(OccupantGuest Occupant)
        {


            _repo.SaveOccupantGuest(Occupant);

            return ApiResponse<string>.Success(
     Occupant.mode == "I"
     ? "Accommodation created successfully."
     : "Accommodation updated successfully.",
     "OK");
        }
        
        public ApiResponse<List<OccupantGuest>> GetAll()
        {
            var list = _repo.GetOccupantGuest();

            return ApiResponse<List<OccupantGuest>>.Success(
                "Accommodation Type List",
                list);
        }
    }
}
