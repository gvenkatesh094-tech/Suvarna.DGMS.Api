using Suvarna.DGMS.Api.Models;
using Suvarna.DGMS.Api.Repositories;

namespace Suvarna.DGMS.Api.Services
{
    public class ServicesCatalogueService
    {
        private readonly ServicesCatalogueRepository _repo;

        public ServicesCatalogueService(ServicesCatalogueRepository repo)
        {
            _repo = repo;
        }

        public ApiResponse<string> inUpdService(ServiceCatalogue serviceCatalogue)
        {
            _repo.SaveServiceCatalogue(serviceCatalogue);

            return ApiResponse<string>.Success(
                serviceCatalogue.mode == "I"
                    ? "Service created successfully."
                    : "Service updated successfully.",
                "OK");
        }

        public ApiResponse<List<ServiceCatalogue>> GetAll()
        {
            var list = _repo.GetServiceCatalogue();

            return ApiResponse<List<ServiceCatalogue>>.Success(
                "Services Catalogue List",
                list);
        }
    }
}