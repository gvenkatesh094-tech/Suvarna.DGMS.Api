using Suvarna.DGMS.Api.Models;
using Suvarna.DGMS.Api.Repositories;

namespace Suvarna.DGMS.Api.Services
{
    public class TariffService
    {
        private readonly TariffRepository _repo;

        public TariffService(
            TariffRepository repo)
        {
            _repo = repo;
        }

        

        public ApiResponse<string> SaveTariff(TariffSaveRequest tariff)
        {
            _repo.SaveTariff(tariff);

            string message = tariff.mode switch
            {
                "I" => "Tariff created successfully.",
                "U" => "Tariff updated successfully.",
                "D" => "Tariff deleted successfully.",
                _ => "Operation completed successfully."
            };

            return ApiResponse<string>.Success(
                message,
                "OK"
            );
        }

        public ApiResponse<List<Tariff>> GetAll(int sessionId, int locationId)
        {
            var data =
                _repo.GetAllTariffs(sessionId, locationId);

            return ApiResponse<List<Tariff>>.Success(
                "Tariff list fetched successfully.",
                data
            );
        }
        public ApiResponse<List<BillingModelMaster>> GetBillingmodelAll(int sessionId, int locationId)
        {
            var data =
                _repo.GetAllBillingModels(sessionId, locationId);

            return ApiResponse<List<BillingModelMaster>>.Success(
                "Tariff list fetched successfully.",
                data
            );
        }
    }
}