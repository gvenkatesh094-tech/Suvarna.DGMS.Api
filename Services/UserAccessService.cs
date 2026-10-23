using Suvarna.DGMS.Api.Helpers;
using Suvarna.DGMS.Api.Models;
using Suvarna.DGMS.Api.Repositories;

namespace Suvarna.DGMS.Api.Services
{
    public class UserAccessService
    {
        private readonly UserAccessRepository _repository;
        private readonly PropertyAccessHelper _access;
        private readonly ILogger<UserAccessService> _logger;

        public UserAccessService(UserAccessRepository repository, PropertyAccessHelper access, ILogger<UserAccessService> logger)
        {
            _repository = repository;
            _access = access;
            _logger = logger;
        }

        public async Task<ApiResponseIntf<List<AccessUserModel>>> GetUsers(string? search)
        {
            try
            {
                var list = await _repository.GetUsers(search);
                return ApiResponseIntf<List<AccessUserModel>>.Success("Users fetched successfully", list);
            }
            catch (UnauthorizedAccessException ex)
            {
                return ApiResponseIntf<List<AccessUserModel>>.Fail(ex.Message, "401");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetUsers failed");
                return ApiResponseIntf<List<AccessUserModel>>.Fail("Unable to load users", "500");
            }
        }

        public async Task<ApiResponseIntf<List<AccessPropertyModel>>> GetProperties()
        {
            try
            {
                var list = await _repository.GetProperties();
                return ApiResponseIntf<List<AccessPropertyModel>>.Success("Properties fetched successfully", list);
            }
            catch (UnauthorizedAccessException ex)
            {
                return ApiResponseIntf<List<AccessPropertyModel>>.Fail(ex.Message, "401");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetProperties failed");
                return ApiResponseIntf<List<AccessPropertyModel>>.Fail("Unable to load properties", "500");
            }
        }

        public async Task<ApiResponseIntf<UserPropertyAccessModel>> GetUserAccess(int userId)
        {
            try
            {
                if (userId <= 0)
                {
                    return ApiResponseIntf<UserPropertyAccessModel>.Fail("Invalid user id");
                }

                var model = await _repository.GetUserAccess(userId);
                return ApiResponseIntf<UserPropertyAccessModel>.Success("Permissions fetched successfully", model);
            }
            catch (UnauthorizedAccessException ex)
            {
                return ApiResponseIntf<UserPropertyAccessModel>.Fail(ex.Message, "401");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetUserAccess failed for user {UserId}", userId);
                return ApiResponseIntf<UserPropertyAccessModel>.Fail("Unable to load permissions", "500");
            }
        }

        public async Task<ApiResponseIntf<string>> Save(SaveUserPropertyAccessRequest request)
        {
            try
            {
                if (request == null || request.UserId <= 0)
                {
                    return ApiResponseIntf<string>.Fail("Please select a user.");
                }

                if (!request.IsAdmin && (request.PropertyIds == null || !request.PropertyIds.Any(x => x > 0)))
                {
                    return ApiResponseIntf<string>.Fail("Select at least one property, or mark the user as administrator.");
                }

                var (success, message) = await _repository.Save(request, _access.AdminRoleFlag());

                return success
                    ? ApiResponseIntf<string>.Success(message, "SUCCESS")
                    : ApiResponseIntf<string>.Fail(string.IsNullOrWhiteSpace(message) ? "Unable to save permissions" : message, "403");
            }
            catch (UnauthorizedAccessException ex)
            {
                return ApiResponseIntf<string>.Fail(ex.Message, "401");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Save permissions failed for user {UserId}", request?.UserId);
                return ApiResponseIntf<string>.Fail("Unable to save permissions", "500");
            }
        }
    }
}
