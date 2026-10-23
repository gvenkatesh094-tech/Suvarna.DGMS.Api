using Suvarna.DGMS.Api.Helpers;
using Suvarna.DGMS.Api.Models;
using Suvarna.DGMS.Api.Repositories;

namespace Suvarna.DGMS.Api.Services
{
    public class AuthService
    {
        private readonly AuthRepository _repository;
        private readonly JwtHelper _jwtHelper;
        private readonly PropertyAccessHelper _access;
        private readonly ILogger<AuthService> _logger;

        public AuthService(AuthRepository repository, JwtHelper jwtHelper, PropertyAccessHelper access, ILogger<AuthService> logger)   //DEPENDENCY INJECTION
        {
            _repository = repository;
            _jwtHelper = jwtHelper;
            _access = access;
            _logger = logger;
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

                // FIX: real role from the database (same source the refresh-token
                // flow already uses) instead of a hard-coded "admin".
                var jwtUser = await _repository.GetUserById(result.UserId ?? 0);

                var token = _jwtHelper.GenerateJwtToken(

                    result.UserId ?? 0,

                    request.UserName,

                    string.IsNullOrWhiteSpace(jwtUser?.ROLE_NAME) ? "user" : jwtUser!.ROLE_NAME
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
                return ApiResponseIntf<LoginResponse>.Fail(SafeMessage(ex, "Login"), "500");
            }
        }

        /// <summary>
        /// FIX (security): ex.Message of a real error (SQL server / object names, connection details) was sent to the browser.
        /// A message raised on purpose by a procedure (RAISERROR / THROW 50000+) is still shown; anything else is logged
        /// and the client gets a reference id, the same as GlobalExceptionMiddleware.
        /// </summary>
        private string SafeMessage(Exception ex, string action)
        {
            for (Exception? e = ex; e != null; e = e.InnerException)
            {
                if (e is Microsoft.Data.SqlClient.SqlException sql)
                {
                    foreach (Microsoft.Data.SqlClient.SqlError err in sql.Errors)
                    {
                        if (err.Number >= 50000 && !string.IsNullOrWhiteSpace(err.Message)) return err.Message;
                    }
                    break;
                }
            }

            var errorId = Guid.NewGuid().ToString("N");
            _logger.LogError(ex, "{Action} failed. Reference Id: {ErrorId}", action, errorId);
            return $"Unexpected error. Reference Id: {errorId}";
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
                // PROPERTY LEVEL ACCESS
                // Normal users with property permissions see only the locations
                // of those properties. Admins / users without permissions: unchanged.
                // ============================================

                facilities = await ApplyLocationAccess(userId, facilities);

                if (facilities.Count == 0)
                {
                    return ApiResponseIntf<List<UserFacilityModel>>.Fail("No location or property access has been given to this user. Please contact the administrator.");
                }

                // ============================================
                // SUCCESS
                // ============================================

                return ApiResponseIntf<List<UserFacilityModel>>.Success("Facilities Fetched Successfully", facilities);
            }
            catch (Exception ex)
            {
                return ApiResponseIntf<List<UserFacilityModel>>.Fail(SafeMessage(ex, "GetUserFacilities"), "500");
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
                // PROPERTY (only when the new login screen sent one;
                // old clients send nothing and keep the old behaviour)
                // =============================

                if (request.PropertyId.HasValue && request.PropertyId.Value >= 0)
                {
                    long userId = _access.JwtUserId > 0 ? _access.JwtUserId : request.UserId;

                    SessionPropertyResult property;

                    try
                    {
                        property = await _repository.SetSessionProperty(
                            result.SessionId.Value,
                            userId,
                            request.PropertyId.Value,
                            _access.StrictModeFlag,
                            _access.AdminRoleFlag());
                    }
                    catch (Exception ex) when (IsMissingAccessObject(ex))
                    {
                        // Database script not installed yet -> continue exactly like before
                        _logger.LogWarning("Session property not saved (objects missing): {Message}", ex.Message);
                        return ApiResponseIntf<UserSessionResult>.Success("Session created successfully", result);
                    }

                    if (!property.Success)
                    {
                        return ApiResponseIntf<UserSessionResult>.Fail(property.Message ?? "Unable to select the property", "403");
                    }

                    result.PropertyId = property.PropertyId;
                    result.PropertyName = property.PropertyName;
                    result.PropertyCode = property.PropertyCode;
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

        // =====================================================
        // PROPERTY LEVEL ACCESS
        // =====================================================

        /// <summary>
        /// Properties of one location the signed-in user may choose on the login screen.
        /// If the database script is not installed yet, returns AccessMode = "LEGACY" with no
        /// properties so the login screen continues with the old (location only) flow.
        /// </summary>
        public async Task<ApiResponseIntf<LoginPropertiesResponse>> GetLoginProperties(long userId, long locId)
        {
            try
            {
                if (userId <= 0)
                {
                    return ApiResponseIntf<LoginPropertiesResponse>.Fail("Invalid user id");
                }

                if (locId <= 0)
                {
                    return ApiResponseIntf<LoginPropertiesResponse>.Fail("Invalid location");
                }

                var (properties, isAdmin, accessMode) = await _repository.GetLoginProperties(
                    userId, locId, _access.StrictModeFlag, _access.AdminRoleFlag());

                var response = new LoginPropertiesResponse
                {
                    LocId = (int)locId,
                    IsAdmin = isAdmin,
                    AccessMode = accessMode,
                    AllowAllProperties = isAdmin && properties.Count > 1,
                    Properties = properties
                };

                if (accessMode == "NONE" || (accessMode == "MAP" && properties.Count == 0))
                {
                    return ApiResponseIntf<LoginPropertiesResponse>.Fail("No property access has been given to you for this location. Please contact the administrator.", "403");
                }

                return ApiResponseIntf<LoginPropertiesResponse>.Success("Properties fetched successfully", response);
            }
            catch (Exception ex) when (IsMissingAccessObject(ex))
            {
                _logger.LogWarning("Property access objects are not installed yet - login continues without property: {Message}", ex.Message);

                return ApiResponseIntf<LoginPropertiesResponse>.Success("Property access is not configured", new LoginPropertiesResponse
                {
                    LocId = (int)locId,
                    AccessMode = "LEGACY"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetLoginProperties failed for user {UserId} location {LocId}", userId, locId);
                return ApiResponseIntf<LoginPropertiesResponse>.Fail("Unable to load properties", "500");
            }
        }

        /// <summary>Changes the property of the current session without logging out.</summary>
        public async Task<ApiResponseIntf<SessionPropertyResult>> SwitchProperty(string sessionId, int propertyId)
        {
            try
            {
                if (!long.TryParse(sessionId, out long sid) || sid <= 0)
                {
                    return ApiResponseIntf<SessionPropertyResult>.Fail("Session id is missing. Please login again.", "401");
                }

                long userId = _access.JwtUserId;

                if (userId <= 0)
                {
                    return ApiResponseIntf<SessionPropertyResult>.Fail("Invalid user", "401");
                }

                if (propertyId < 0)
                {
                    return ApiResponseIntf<SessionPropertyResult>.Fail("Invalid property");
                }

                SessionPropertyResult result = await _repository.SetSessionProperty(
                    sid, userId, propertyId, _access.StrictModeFlag, _access.AdminRoleFlag());

                if (!result.Success)
                {
                    return ApiResponseIntf<SessionPropertyResult>.Fail(result.Message ?? "Unable to change property", "403");
                }

                return ApiResponseIntf<SessionPropertyResult>.Success(result.Message ?? "Property changed successfully", result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SwitchProperty failed for session {SessionId}", sessionId);
                return ApiResponseIntf<SessionPropertyResult>.Fail("Unable to change property", "500");
            }
        }

        /// <summary>
        /// Keeps only the locations a normal user has property permissions for.
        /// Never blocks login because of a technical problem: on any error the list is returned unchanged.
        /// </summary>
        private async Task<List<UserFacilityModel>> ApplyLocationAccess(long userId, List<UserFacilityModel> facilities)
        {
            try
            {
                if (_access.IsAdminRole())
                {
                    return facilities;
                }

                UserAccessInfo info = await _repository.GetUserAccessInfo(userId);

                if (info.IsAdmin)
                {
                    return facilities;
                }

                if (info.HasMapping)
                {
                    var allowed = new HashSet<long>(info.LocIds.Select(x => (long)x));
                    return facilities.Where(f => allowed.Contains(f.FacilityId)).ToList();
                }

                // No permissions given yet
                return _access.StrictModeFlag == "Y" ? new List<UserFacilityModel>() : facilities;
            }
            catch (Exception ex)
            {
                _logger.LogWarning("Location access filter skipped: {Message}", ex.Message);
                return facilities;
            }
        }

        /// <summary>SQL error 2812 = "Could not find stored procedure" (DB script not run yet).</summary>
        private static bool IsMissingAccessObject(Exception ex)
        {
            for (Exception? e = ex; e != null; e = e.InnerException)
            {
                if (e is Microsoft.Data.SqlClient.SqlException sql && (sql.Number == 2812 || sql.Number == 208))
                {
                    return true;
                }
            }

            return false;
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
                return ApiResponseIntf<List<UserLocationResponse>>.Fail(SafeMessage(ex, "GetUsersByLocation"), "500");
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

                    SafeMessage(ex, "RefreshToken"),

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
                return ApiResponseIntf<string>.Fail(SafeMessage(ex, "Logout"), "500");
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
