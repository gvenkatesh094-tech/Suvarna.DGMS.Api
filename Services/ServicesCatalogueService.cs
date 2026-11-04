using Suvarna.DGMS.Api.Models;
using Suvarna.DGMS.Api.Repositories;

namespace Suvarna.DGMS.Api.Services
{
    public class ServicesCatalogueService
    {
        private readonly ServicesCatalogueRepository _repo;

        private readonly Suvarna.DGMS.Api.Helpers.PropertyScope _scope;   // 28-Oct: property switched in the header

        public ServicesCatalogueService(ServicesCatalogueRepository repo, Suvarna.DGMS.Api.Helpers.PropertyScope scope)
        {
            _repo = repo;
            _scope = scope;
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

            // 28-Oct: only the services of the current property (+ common services without a property)
            var propertyId = _scope.CurrentPropertyId;
            if (propertyId is not null)
                list = list.Where(s => s.propertyId == 0 || s.propertyId == propertyId.Value).ToList();

            return ApiResponse<List<ServiceCatalogue>>.Success(
                "Services Catalogue List",
                list);
        }
    }
}