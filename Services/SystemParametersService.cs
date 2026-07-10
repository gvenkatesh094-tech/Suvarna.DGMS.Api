using Suvarna.DGMS.Api.Models;
using Suvarna.DGMS.Api.Repositories;

namespace Suvarna.DGMS.Api.Services
{
    public class SystemParametersService
    {
        private readonly SystemParametersRepository _repo;

        public SystemParametersService(SystemParametersRepository repo)
        {
            _repo = repo;
        }

        public ApiResponse<string> InUpdSystemParameter(SystemParameter parameter)
        {
            _repo.SaveSystemParameter(parameter);

            return ApiResponse<string>.Success(
                parameter.mode == "I"
                    ? "System Parameter created successfully."
                    : parameter.mode == "U"
                        ? "System Parameter updated successfully."
                        : "System Parameter deleted successfully.",
                "OK");
        }

        public ApiResponse<List<SystemParameter>> GetAll()
        {
            var list = _repo.GetSystemParameters();

            return ApiResponse<List<SystemParameter>>.Success(
                "System Parameters List",
                list);
        }

        public ApiResponse<List<string>> GetCategories()
        {
            var list = _repo.GetCategories();

            return ApiResponse<List<string>>.Success(
                "System Parameter Categories",
                list);
        }
    }
}