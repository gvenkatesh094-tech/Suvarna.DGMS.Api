namespace Suvarna.DGMS.Api.Models
{
    public class Model_authModels
    {
    }
    public class LoginRequest
    {
        public string UserName { get; set; }

        public string Password { get; set; }

        public string ClientName { get; set; }

        public int GroupId { get; set; }

        public string TransactionPassword { get; set; }

        public DateTime SystemTime { get; set; }

        public string DomainValue { get; set; }
    }
    public class LoginResponse
    {
        public int? UserId { get; set; }
        public string? ReturnMessage { get; set; }
        public string? Token { get; set; }
        public string? RefreshToken { get; set; }
    }
    public class WebConfigModel
    {
        public string Key { get; set; }

        public string Value { get; set; }
    }
    public class UserDetailsModel
    {
        public long UserId { get; set; }

        public string UserCd { get; set; }

        public string UserName { get; set; }

        public string EmailId { get; set; }

        public string MobileNo { get; set; }

        public string ReferenceName { get; set; }

        public long RoleId { get; set; }

        public string RoleCd { get; set; }

        public string RoleName { get; set; }
    }
    public class UserFacilityModel
    {
        public long FacilityId { get; set; }

        public string FacilityName { get; set; }

        public string FacilityCd { get; set; }

        public int FacilityRevNo { get; set; }

        public int TotalRecordCount { get; set; }
    }
    public class UserSessionRequest
    {
        public string Machine { get; set; } = string.Empty;

        public string Version { get; set; } = string.Empty;

        public string Terminal { get; set; } = string.Empty;

        public string OsUser { get; set; } = string.Empty;

        public int UserId { get; set; }

        public int OrgId { get; set; }

        public int RoleId { get; set; }

        public int LocId { get; set; }

        public int TimeZoneId { get; set; }

        public string Browser { get; set; } = string.Empty;

        public string BrowserVersion { get; set; } = string.Empty;

        public string ClientName { get; set; } = string.Empty;

        // Property chosen at login.
        // null = old client (no property step) -> behaves exactly as before.
        // 0    = "All Properties" (administrators only).
        public int? PropertyId { get; set; }
    }
    public class UserSessionResult
    {
        public int? SessionId { get; set; }

        public int? RoleId { get; set; }

        public string RoleName { get; set; }

        public int? OrgId { get; set; }
        public string? OrgName { get; set; }

        public string? DisplayName { get; set; }

        // Filled only when a property was sent in the request
        public int? PropertyId { get; set; }
        public string? PropertyName { get; set; }
        public string? PropertyCode { get; set; }
    }

    // =========================================
    // PROPERTY LEVEL ACCESS (login / switch)
    // =========================================

    public class LoginPropertyModel
    {
        public int PropertyId { get; set; }
        public string PropertyCd { get; set; } = string.Empty;
        public string PropertyNm { get; set; } = string.Empty;
        public string? PropertyTypeCd { get; set; }
        public string? City { get; set; }
        public int LocId { get; set; }
        public bool IsDefault { get; set; }
    }

    public class LoginPropertiesResponse
    {
        public int LocId { get; set; }

        public bool IsAdmin { get; set; }

        // ALL  = every property of the location
        // MAP  = only the properties given on the permission screen
        // NONE = strict mode and nothing given
        // LEGACY = property access objects not installed in the database yet (old flow)
        public string AccessMode { get; set; } = "ALL";

        // Administrators may choose "All Properties" when the location has more than one
        public bool AllowAllProperties { get; set; }

        public List<LoginPropertyModel> Properties { get; set; } = new();
    }

    public class SwitchPropertyRequest
    {
        public int PropertyId { get; set; }
    }

    public class SessionPropertyResult
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public int PropertyId { get; set; }
        public string? PropertyName { get; set; }
        public string? PropertyCode { get; set; }
    }

    public class UserAccessInfo
    {
        public bool IsAdmin { get; set; }
        public bool HasMapping { get; set; }
        public List<int> LocIds { get; set; } = new();
    }

    /// <summary>appsettings section "PropertyAccess"</summary>
    public class PropertyAccessSettings
    {
        // false (default) : a normal user with no permissions yet sees all properties of the location (old behaviour)
        // true            : a normal user must be given properties on the permission screen
        public bool StrictMode { get; set; } = false;

        // Role names treated as administrators (all locations + all properties), e.g. [ "ADMIN" ]
        public string[] AdminRoles { get; set; } = Array.Empty<string>();
    }
    public class UserLocationResponse
    {
        public long UserId { get; set; }

        public string UserName { get; set; } = string.Empty;

        public string UserCd { get; set; } = string.Empty;

        public long ReferenceId { get; set; }

        public long ReferenceTypeId { get; set; }

        public string ReferenceTypeName { get; set; } = string.Empty;

        public long LocationId { get; set; }

        public string LocationName { get; set; } = string.Empty;

        public long EmployeeId { get; set; }

        public string EmployeeName { get; set; } = string.Empty;

        public long DepartmentId { get; set; }

        public string DepartmentName { get; set; } = string.Empty;
    }
    public class ApiResponseIntf<T>
    {
        public bool SuccessStatus { get; set; }

        public string? Message { get; set; }

        public string? StatusCode { get; set; }

        public T? Data { get; set; }

        // =========================================
        // SUCCESS RESPONSE
        // =========================================

        public static ApiResponseIntf<T> Success(string message, T data)
        {
            return new ApiResponseIntf<T>
            {
                SuccessStatus = true,
                Message = message,
                StatusCode = "200",
                Data = data
            };
        }

        // =========================================
        // FAIL RESPONSE
        // =========================================

        public static ApiResponseIntf<T> Fail(string message, string statusCode = "400")
        {
            return new ApiResponseIntf<T>
            {
                SuccessStatus = false,
                Message = message,
                StatusCode = statusCode,
                Data = default
            };
        }
    }
}
