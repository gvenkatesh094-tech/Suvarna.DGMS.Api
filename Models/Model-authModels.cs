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
    }
    public class UserSessionResult
    {
        public int? SessionId { get; set; }

        public int? RoleId { get; set; }

        public string RoleName { get; set; }

        public int? OrgId { get; set; }
        public string? OrgName { get; set; }

        public string? DisplayName { get; set; }
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
