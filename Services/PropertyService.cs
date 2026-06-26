using Suvarna.DGMS.Api.Models;
using Suvarna.DGMS.Api.Repositories;

namespace Suvarna.DGMS.Api.Services
{
    public class PropertyService
    {
        private readonly PropertyRepository _repo;

        public PropertyService(PropertyRepository repo)
        {
            _repo = repo;
        }
        public ApiResponse<string> Create(Property _pr)
        {
            if (string.IsNullOrWhiteSpace(_pr.propertyCode))
                return ApiResponse<string>.Fail("Property Code is required.");

            if (string.IsNullOrWhiteSpace(_pr.propertyName))
                return ApiResponse<string>.Fail("Property Name is required.");

            if (string.IsNullOrWhiteSpace(_pr.propertyType))
                return ApiResponse<string>.Fail("Property Type is required.");

            if (_pr.grpId <= 0)
                return ApiResponse<string>.Fail("Group is required.");

            if (_pr.orgId <= 0)
                return ApiResponse<string>.Fail("Organization is required.");

            if (_pr.locId <= 0)
                return ApiResponse<string>.Fail("Location is required.");

            // Optional validation
            if (!string.IsNullOrWhiteSpace(_pr.email))
            {
                try
                {
                    var addr = new System.Net.Mail.MailAddress(_pr.email);
                }
                catch
                {
                    return ApiResponse<string>.Fail("Invalid Email Address.");
                }
            }

            if (!string.IsNullOrWhiteSpace(_pr.gstin) && _pr.gstin.Length != 15)
                return ApiResponse<string>.Fail("GSTIN should be 15 characters.");

            _repo.InsertProperty(_pr);

            return ApiResponse<string>.Success("Property created successfully.", "OK");
        }
        public ApiResponse<string> Update(Property property)
        {
            if (property.propertyId <= 0)
                return ApiResponse<string>.Fail("Property Id is required.");

            if (string.IsNullOrWhiteSpace(property.propertyCode))
                return ApiResponse<string>.Fail("Property Code is required.");

            if (string.IsNullOrWhiteSpace(property.propertyName))
                return ApiResponse<string>.Fail("Property Name is required.");

            if (string.IsNullOrWhiteSpace(property.propertyType))
                return ApiResponse<string>.Fail("Property Type is required.");

            _repo.UpdateProperty(property);

            return ApiResponse<string>.Success("Property updated successfully.", "OK");
        }
        public ApiResponse<List<Property>> GetAll()
        {
            var data = _repo.GetAllProperties();

            return ApiResponse<List<Property>>.Success(
                "Property list fetched successfully.",
                data
            );
        }
    }
}
