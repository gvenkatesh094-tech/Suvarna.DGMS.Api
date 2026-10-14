using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Suvarna.DGMS.Api.Helpers
{
    public class JwtHelper
    {
        // Used when appsettings has no valid expiry value
        private const double DefaultExpiryMinutes = 60;

        private readonly IConfiguration _configuration;

        public JwtHelper(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // =====================================================
        // ACCESS TOKEN
        // =====================================================

        public string GenerateJwtToken(long userId, string userName, string roleName)
        {
            var keyText = _configuration["Jwt:Key"];

            if (string.IsNullOrWhiteSpace(keyText))
            {
                throw new InvalidOperationException("JWT key is not configured (Jwt:Key).");
            }

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Name, userName ?? string.Empty),
                new Claim(ClaimTypes.Role, roleName ?? string.Empty),

                // Unique id for every token (useful for logging / revoking)
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyText));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Always UTC, so the expiry is correct on any server time zone
            var now = DateTime.UtcNow;

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                notBefore: now,
                expires: now.AddMinutes(GetExpiryMinutes()),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        // =====================================================
        // REFRESH TOKEN
        // =====================================================

        public string GenerateRefreshToken()
        {
            // Guid.NewGuid() uses a cryptographically secure random source in .NET.
            // Kept as Guid so it still fits the existing database column.
            return Guid.NewGuid().ToString();
        }

        // =====================================================
        // HELPERS
        // =====================================================

        /// <summary>
        /// Reads "Jwt:ExpiryMinutes" (current appsettings name).
        /// Falls back to the old name "Jwt:DurationInMinutes", then to 60 minutes.
        /// </summary>
        private double GetExpiryMinutes()
        {
            var value = _configuration["Jwt:ExpiryMinutes"]
                        ?? _configuration["Jwt:DurationInMinutes"];

            return double.TryParse(value, out var minutes) && minutes > 0
                ? minutes
                : DefaultExpiryMinutes;
        }
    }
}
