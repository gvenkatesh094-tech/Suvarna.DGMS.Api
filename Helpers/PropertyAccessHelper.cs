using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Suvarna.DGMS.Api.Models;

namespace Suvarna.DGMS.Api.Helpers
{
    /// <summary>
    /// Settings + signed-in user details needed by the property level access checks.
    /// appsettings:  "PropertyAccess": { "StrictMode": false, "AdminRoles": [ "ADMIN" ] }
    /// </summary>
    public class PropertyAccessHelper
    {
        private readonly PropertyAccessSettings _settings;
        private readonly IHttpContextAccessor _http;

        public PropertyAccessHelper(IOptions<PropertyAccessSettings> options, IHttpContextAccessor http)
        {
            _settings = options.Value ?? new PropertyAccessSettings();
            _http = http;
        }

        /// <summary>"Y" / "N" - passed to the stored procedures as @IP_STRICT_MODE.</summary>
        public string StrictModeFlag => _settings.StrictMode ? "Y" : "N";

        /// <summary>User id from the JWT, or 0 when the call is not authenticated.</summary>
        public long JwtUserId
        {
            get
            {
                var value = _http.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
                return long.TryParse(value, out var id) ? id : 0;
            }
        }

        /// <summary>True when the role in the JWT is listed in PropertyAccess:AdminRoles.</summary>
        public bool IsAdminRole(string? roleName = null)
        {
            var role = roleName ?? _http.HttpContext?.User?.FindFirstValue(ClaimTypes.Role);

            if (string.IsNullOrWhiteSpace(role) || _settings.AdminRoles == null || _settings.AdminRoles.Length == 0)
            {
                return false;
            }

            return _settings.AdminRoles.Any(r => string.Equals(r?.Trim(), role.Trim(), StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>"Y" / "N" - passed to the stored procedures as @IP_FORCE_ADMIN.</summary>
        public string AdminRoleFlag(string? roleName = null) => IsAdminRole(roleName) ? "Y" : "N";
    }
}
