using Suvarna.DGMS.Api.Helpers;
using Suvarna.DGMS.Api.Models;
using Suvarna.DGMS.Api.Repositories;

namespace Suvarna.DGMS.Api.Services
{
    public class AuthService
    {
        private readonly AuthRepository _repository;
        private readonly JwtHelper _jwtHelper;

        public AuthService(AuthRepository repository, JwtHelper jwtHelper)   //DEPENDENCY INJECTION
        {
            _repository = repository;
            _jwtHelper = jwtHelper;
        }

        public async Task<ApiResponseIntf<LoginResponse>> Login(LoginRequest request)
        {
            try
            {
                // =====================================
                // BASIC VALIDATION
                // =====================================

                if (request == null)
                {
                    return ApiResponseIntf<LoginResponse>.Fail("Invalid Request", "400");
                }

                if (string.IsNullOrWhiteSpace(request.UserName))
                {
                    return ApiResponseIntf<LoginResponse>.Fail("Username Required", "400");
                }

                if (string.IsNullOrWhiteSpace(request.Password))
                {
                    return ApiResponseIntf<LoginResponse>.Fail("Password Required", "400");
                }

                // =====================================
                // DB CALL
                // =====================================

                var result = await _repository.ValidateUserLogin(request);

                // =====================================
                // NULL RESPONSE
                // =====================================

                if (result == null)
                {
                    return ApiResponseIntf<LoginResponse>.Fail("Login Failed", "500");
                }

                // =====================================
                // INVALID LOGIN
                // =====================================

                if (!result.UserId.HasValue)
                {
                    return ApiResponseIntf<LoginResponse>.Fail(result.ReturnMessage ?? "Invalid Login", "401");
                }
                // =====================================
                // JWT TOKEN GENERATION
                // =====================================

                var token = _jwtHelper.GenerateJwtToken(

                    result.UserId ?? 0,

                    request.UserName,

                    "admin"    //THIS ROLE NEED TO COME FROM LOGIN PAGE DYNAMIC LIKE USER NAME AND PASSWORD
                );
                var refreshToken = _jwtHelper.GenerateRefreshToken();
                result.Token = token;
                result.RefreshToken = refreshToken;
                await _repository.SaveRefreshToken(result.UserId ?? 0, refreshToken);
                // =====================================
                // SUCCESS
                // =====================================

                return ApiResponseIntf<LoginResponse>.Success("Login Successful", result);

            }
            catch (Exception ex)
            {
                return ApiResponseIntf<LoginResponse>.Fail(ex.Message, "500");
            }
        }
        public async Task<ApiResponseIntf<string>> GetClientName()
        {
            try
            {
                string clientName = await _repository.GetClientName();

                if (string.IsNullOrWhiteSpace(clientName))
                {
                    return ApiResponseIntf<string>.Fail("Client Name Not Found", "404");
                }

                return ApiResponseIntf<string>.Success("Client Name Fetched Successfully", clientName);
            }
            catch (Exception)
            {
                return ApiResponseIntf<string>.Fail("Something went wrong", "500");
            }
        }

        public async Task<ApiResponseIntf<UserDetailsModel>> GetUserDetails(long userId)
        {
            try
            {
                // =========================================
                // VALIDATION
                // =========================================

                if (userId <= 0)
                {
                    return ApiResponseIntf<UserDetailsModel>.Fail("Invalid User Id", "400");
                }

                // =========================================
                // REPOSITORY CALL
                // =========================================

                UserDetailsModel response = await _repository.GetUserDetails(userId);

                // =========================================
                // NO DATA
                // =========================================

                if (response == null || response.UserId <= 0)
                {
                    return ApiResponseIntf<UserDetailsModel>.Fail("User Details Not Found", "404");
                }

                // =========================================
                // SUCCESS
                // =========================================

                return ApiResponseIntf<UserDetailsModel>.Success("User Details Fetched Successfully", response);
            }
            catch (Exception)
            {
                return ApiResponseIntf<UserDetailsModel>.Fail("Something went wrong", "500");
            }
        }
        public async Task<ApiResponseIntf<List<UserFacilityModel>>> GetUserFacilities(long userId)
        {
            try
            {
                // ============================================
                // VALIDATION
                // ============================================

                if (userId <= 0)
                {
                    return ApiResponseIntf<List<UserFacilityModel>>.Fail("INVALID USER ID");
                }

                // ============================================
                // GET DATA
                // ============================================

                var facilities = await _repository.GetUserFacilities(userId);

                // ============================================
                // NO DATA
                // ============================================

                if (facilities == null ||
                    facilities.Count == 0)
                {
                    return ApiResponseIntf<List<UserFacilityModel>>.Fail("NO FACILITIES FOUND");
                }

                // ============================================
                // SUCCESS
                // ============================================

                return ApiResponseIntf<List<UserFacilityModel>>.Success("Facilities Fetched Successfully", facilities);
            }
            catch (Exception ex)
            {
                return ApiResponseIntf<List<UserFacilityModel>>.Fail(ex.Message, "500");
            }
        }
        public async Task<ApiResponseIntf<UserSessionResult>> CreateSession(UserSessionRequest request)
        {
            try
            {
                // =============================
                // BASIC VALIDATION
                // =============================

                if (request == null)
                {
                    return ApiResponseIntf<UserSessionResult>.Fail("Invalid request");
                }

                if (request.UserId <= 0)
                {
                    return ApiResponseIntf<UserSessionResult>.Fail("Invalid user id");
                }

                if (request.LocId <= 0)
                {
                    return ApiResponseIntf<UserSessionResult>.Fail("Invalid location");
                }

                // =============================
                // DB CALL
                // =============================

                UserSessionResult result = await _repository.CreateUserSession(request);

                // =============================
                // DB RESULT VALIDATION
                // =============================

                if (result == null
                    || result.SessionId == null
                    || result.SessionId <= 0)
                {
                    return ApiResponseIntf<UserSessionResult>.Fail("Session creation failed");
                }

                // =============================
                // SUCCESS
                // =============================

                return ApiResponseIntf<UserSessionResult>.Success("Session created successfully", result);
            }
            catch
            {
                return ApiResponseIntf<UserSessionResult>.Fail("Unexpected error occurred");
            }
        }

        public async Task<ApiResponseIntf<List<UserLocationResponse>>> GetUsersByLocation(long locId)
        {
            try
            {
                // ============================================
                // VALIDATION
                // ============================================

                if (locId <= 0)
                {
                    return ApiResponseIntf<List<UserLocationResponse>>.Fail("INVALID LOCATION ID");
                }

                // ============================================
                // GET DATA
                // ============================================

                var users = await _repository.GetUsersByLocation(locId);

                // ============================================
                // NO DATA
                // ============================================

                if (users == null ||
                    users.Count == 0)
                {
                    return ApiResponseIntf<List<UserLocationResponse>>.Fail("NO USERS FOUND");
                }

                // ============================================
                // SUCCESS
                // ============================================

                return ApiResponseIntf<List<UserLocationResponse>>.Success("Users Fetched Successfully", users);
            }
            catch (Exception ex)
            {
                return ApiResponseIntf<List<UserLocationResponse>>.Fail(ex.Message, "500");
            }
        }

        public async Task<ApiResponseIntf<LoginResponse>> RefreshToken(RefreshTokenRequest request)
        {
            try
            {
                // =====================================
                // GET REFRESH TOKEN FROM DB
                // =====================================

                var dbToken = await _repository.GetRefreshToken(request.RefreshToken);

                // =====================================
                // TOKEN NOT FOUND
                // =====================================

                if (dbToken == null)
                {
                    return ApiResponseIntf<LoginResponse>.Fail("Invalid Refresh Token", "401");
                }

                // =====================================
                // TOKEN EXPIRED
                // =====================================

                if (dbToken.EXPIRY_DATE < DateTime.Now)
                {
                    return ApiResponseIntf<LoginResponse>.Fail("Refresh Token Expired", "401");
                }

                // =====================================
                // TOKEN REVOKED
                // =====================================

                if (dbToken.IS_REVOKED == "Y")
                {
                    return ApiResponseIntf<LoginResponse>.Fail("Refresh Token Revoked", "401");
                }

                // =====================================
                // GET ACTUAL USER DETAILS
                // =====================================

                var user =
                    await _repository
                        .GetUserById(
                            dbToken.USER_ID);

                // =====================================
                // USER NOT FOUND
                // =====================================

                if (user == null)
                {
                    return ApiResponseIntf<
                        LoginResponse>.Fail(

                        "User Not Found",

                        "401");
                }

                // =====================================
                // GENERATE NEW JWT TOKEN
                // =====================================

                var newJwtToken =
                    _jwtHelper.GenerateJwtToken(

                        dbToken.USER_ID,

                        user.USER_NAME,

                        user.ROLE_NAME
                    );

                // =====================================
                // RESPONSE
                // =====================================

                var response =
                    new LoginResponse
                    {
                        UserId =
                            (int)dbToken.USER_ID,

                        Token =
                            newJwtToken,

                        RefreshToken =
                            request.RefreshToken,

                        ReturnMessage =
                            "Token Refreshed"
                    };

                return ApiResponseIntf<
                    LoginResponse>.Success(

                    "Token Refreshed Successfully",

                    response);
            }
            catch (Exception ex)
            {
                return ApiResponseIntf<
                    LoginResponse>.Fail(

                    ex.Message,

                    "500");
            }
        }

        public async Task<ApiResponseIntf<string>> Logout(LogoutRequest request)
        {
            try
            {
                // =====================================
                // VALIDATE
                // =====================================

                if (string.IsNullOrEmpty(
                    request.RefreshToken))
                {
                    return ApiResponseIntf<string>.Fail("Refresh Token Required", "400");
                }

                // =====================================
                // REVOKE TOKEN
                // =====================================

                await _repository.RevokeToken(request.RefreshToken);

                // =====================================
                // SUCCESS
                // =====================================

                return ApiResponseIntf<string>.Success("Logout Successful", "SUCCESS");
            }
            catch (Exception ex)
            {
                return ApiResponseIntf<string>.Fail(ex.Message, "500");
            }
        }

        public async Task<ApiResponseIntf<List<ModuleModel>>> GetModules(ModuleRequest request)
        {
            try
            {
                // =====================================
                // BASIC VALIDATION
                // =====================================

                if (request == null)
                {
                    return ApiResponseIntf<List<ModuleModel>>
                        .Fail("Invalid request");
                }

                if (request.UserId <= 0)
                {
                    return ApiResponseIntf<List<ModuleModel>>
                        .Fail("Invalid user id");
                }

                if (request.SessionId <= 0)
                {
                    return ApiResponseIntf<List<ModuleModel>>
                        .Fail("Invalid session id");
                }

                // =====================================
                // DB CALL
                // =====================================

                List<ModuleModel> result =
                    await _repository.GetModules(

                        request.UserId,

                        request.SessionId,

                        request.ParentModuleId
                    );

                // =====================================
                // RESULT VALIDATION
                // =====================================

                if (result == null
                    || result.Count == 0)
                {
                    return ApiResponseIntf<List<ModuleModel>>
                        .Fail("No modules found");
                }

                // =====================================
                // SUCCESS
                // =====================================

                return ApiResponseIntf<List<ModuleModel>>
                    .Success(
                        "Modules loaded successfully",
                        result);
            }
            catch
            {
                return ApiResponseIntf<List<ModuleModel>>
                    .Fail("Unexpected error occurred");
            }
        }

        public async Task<ApiResponseIntf<List<ModuleDocResponse>>> GetUserModuleDocs(
    ModuleDocRequest request)
        {
            try
            {
                // =====================================
                // VALIDATION
                // =====================================

                if (request == null)
                {
                    return ApiResponseIntf<List<ModuleDocResponse>>
                        .Fail("Invalid request");
                }

                if (request.UserId <= 0)
                {
                    return ApiResponseIntf<List<ModuleDocResponse>>
                        .Fail("Invalid user id");
                }

                if (request.SessionId <= 0)
                {
                    return ApiResponseIntf<List<ModuleDocResponse>>
                        .Fail("Invalid session id");
                }

                // =====================================
                // DB CALL
                // =====================================

                List<ModuleDocResponse> result =
                    await _repository.GetUserModuleDocs(
                        request);

                // =====================================
                // RESULT VALIDATION
                // =====================================

                if (result == null || result.Count == 0)
                {
                    return ApiResponseIntf<List<ModuleDocResponse>>
                        .Fail("No records found");
                }

                // =====================================
                // SUCCESS
                // =====================================

                return ApiResponseIntf<List<ModuleDocResponse>>
                    .Success(
                        "Records fetched successfully",
                        result);
            }
            catch
            {
                return ApiResponseIntf<List<ModuleDocResponse>>
                    .Fail("Unexpected error occurred");
            }
        }
    }
}
