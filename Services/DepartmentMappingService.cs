using Suvarna.DGMS.Api.Models;
using Suvarna.DGMS.Api.Repositories;

namespace Suvarna.DGMS.Api.Services
{
    public class DepartmentMappingService
    {
        private readonly DepartmentMappingRepository _repo;

        public DepartmentMappingService(DepartmentMappingRepository repo)
        {
            _repo = repo;
        }

        public ApiResponse<string> Create(DepartmentCostMapping model)
        {
            if (string.IsNullOrWhiteSpace(model.deptCd))
                return ApiResponse<string>.Fail("Department Code is required.");
            if (string.IsNullOrWhiteSpace(model.costCenterCd))
                return ApiResponse<string>.Fail("Cost Center Code is required.");

            var (success, message, newId) = _repo.Save(model);

            if (!success)
                return ApiResponse<string>.Fail(message);

            return ApiResponse<string>.Success(message, newId.ToString());
        }

        public ApiResponse<string> Update(DepartmentCostMapping model)
        {
            if (model.deptMappingId <= 0)
                return ApiResponse<string>.Fail("Department Mapping Id is required.");
            if (string.IsNullOrWhiteSpace(model.deptCd))
                return ApiResponse<string>.Fail("Department Code is required.");
            if (string.IsNullOrWhiteSpace(model.costCenterCd))
                return ApiResponse<string>.Fail("Cost Center Code is required.");

            var (success, message) = _repo.Update(model);

            if (!success)
                return ApiResponse<string>.Fail(message);

            return ApiResponse<string>.Success(message, "OK");
        }

        public ApiResponse<List<DepartmentCostMapping>> GetAll()
        {
            var list = _repo.GetAll();
            return ApiResponse<List<DepartmentCostMapping>>.Success("Department Mapping List", list);
        }

        public ApiResponse<string> Delete(int id, int userId)
        {
            if (id <= 0)
                return ApiResponse<string>.Fail("Department Mapping Id is required.");

            var (success, message) = _repo.Delete(id, userId);

            if (!success)
                return ApiResponse<string>.Fail(message);

            return ApiResponse<string>.Success(message, "OK");
        }
    }
}