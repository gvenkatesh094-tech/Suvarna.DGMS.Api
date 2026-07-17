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

        public ApiResponse<string> GenerateCode(
            GenerateCodeRequest request)
        {
            string tariffCode =
                _repo.GenerateCodeTariff(request);

            return ApiResponse<string>.Success(
                "Tariff Code generated successfully.",
                tariffCode
            );
        }

        public ApiResponse<string> SaveTariff(
            Tariff tariff)
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

        public ApiResponse<List<Tariff>> GetAll()
        {
            var data =
                _repo.GetAllTariffs();

            return ApiResponse<List<Tariff>>.Success(
                "Tariff list fetched successfully.",
                data
            );
        }
    }
}