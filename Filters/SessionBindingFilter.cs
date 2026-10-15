using System.Data;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Caching.Memory;

namespace Suvarna.DGMS.Api.Filters
{
    /// <summary>
    /// Confirms the DB session id sent by the screen (header X-Session-Id) belongs to the user in the JWT.
    /// Stops a signed-in user from calling APIs/SPs with someone else's session (= someone else's org/location).
    /// appsettings:  "Security": { "SessionBinding": "Off" | "Warn" | "Enforce" }   (default Warn = log only)
    /// </summary>
    public class SessionBindingFilter : IAsyncActionFilter
    {
        private readonly IConfiguration _cfg;
        private readonly IMemoryCache _cache;
        private readonly ILogger<SessionBindingFilter> _log;

        public SessionBindingFilter(IConfiguration cfg, IMemoryCache cache, ILogger<SessionBindingFilter> log)
        { _cfg = cfg; _cache = cache; _log = log; }

        public async Task OnActionExecutionAsync(ActionExecutingContext ctx, ActionExecutionDelegate next)
        {
            var mode = _cfg["Security:SessionBinding"] ?? "Warn";
            var http = ctx.HttpContext;

            if (mode.Equals("Off", StringComparison.OrdinalIgnoreCase)
                || http.User?.Identity?.IsAuthenticated != true
                || !http.Request.Headers.TryGetValue("X-Session-Id", out var sid)
                || string.IsNullOrWhiteSpace(sid))
            {
                await next();
                return;
            }

            var jwtUser = http.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var sessionUser = await GetSessionUserAsync(sid.ToString().Trim());
            var ok = sessionUser != null && sessionUser == jwtUser;

            if (!ok)
            {
                _log.LogWarning("Session {Sid} does not belong to JWT user {User} (session user: {SUser})", sid.ToString(), jwtUser, sessionUser);
                if (mode.Equals("Enforce", StringComparison.OrdinalIgnoreCase))
                {
                    ctx.Result = new ObjectResult(new { status = false, code = 403, message = "Session does not belong to the signed-in user." }) { StatusCode = 403 };
                    return;
                }
            }
            await next();
        }

        private async Task<string?> GetSessionUserAsync(string sessionId)
        {
            var key = "sess-user:" + sessionId;
            if (_cache.TryGetValue(key, out string? cached)) return cached;

            string? user = null;
            try
            {
                await using var con = new SqlConnection(_cfg.GetConnectionString("SqlServer"));
                await using var cmd = new SqlCommand("PR_GET_SESSION_DET", con) { CommandType = CommandType.StoredProcedure };
                cmd.Parameters.AddWithValue("@IP_SESSION_ID", sessionId);
                cmd.Parameters.Add(new SqlParameter("@LV_GRP_ID", SqlDbType.SmallInt) { Direction = ParameterDirection.Output });
                cmd.Parameters.Add(new SqlParameter("@LV_ORG_ID", SqlDbType.SmallInt) { Direction = ParameterDirection.Output });
                cmd.Parameters.Add(new SqlParameter("@LV_LOC_ID", SqlDbType.SmallInt) { Direction = ParameterDirection.Output });
                var u = new SqlParameter("@LV_USER_ID", SqlDbType.Int) { Direction = ParameterDirection.Output };
                cmd.Parameters.Add(u);
                cmd.Parameters.Add(new SqlParameter("@LV_UTC_MIN", SqlDbType.Int) { Direction = ParameterDirection.Output });
                await con.OpenAsync();
                await cmd.ExecuteNonQueryAsync();
                user = u.Value == DBNull.Value ? null : Convert.ToString(u.Value);
            }
            catch (Exception ex) { _log.LogError(ex, "Session lookup failed"); return null; }

            _cache.Set(key, user, TimeSpan.FromMinutes(2));
            return user;
        }
    }
}
