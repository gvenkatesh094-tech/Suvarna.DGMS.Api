using System.Data;
using System.Reflection;
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
    ///
    /// FIX (security): most screens also send "sessionId" in the query string / request body, and the repositories
    /// passed THAT value to the procedures (@IP_SESSION_ID). Only the header was checked, so changing the number in
    /// the URL gave the data of another session (= another org / location / user). Unless the mode is "Off", every
    /// "sessionId" action argument (and the "sessionId" property of a request body) is now replaced with the checked
    /// header value, so the repositories always use the signed-in user's own session. No repository change needed.
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
            var enforce = mode.Equals("Enforce", StringComparison.OrdinalIgnoreCase);

            if (mode.Equals("Off", StringComparison.OrdinalIgnoreCase)
                || http.User?.Identity?.IsAuthenticated != true)
            {
                await next();
                return;
            }

            if (!http.Request.Headers.TryGetValue("X-Session-Id", out var sid)
                || string.IsNullOrWhiteSpace(sid))
            {
                // A session id in the URL / body without the header cannot be checked
                if (enforce && HasSessionArgument(ctx))
                {
                    _log.LogWarning("Request {Path} has a sessionId but no X-Session-Id header", http.Request.Path);
                    ctx.Result = new ObjectResult(new { status = false, code = 403, message = "Session is missing. Please login again." }) { StatusCode = 403 };
                    return;
                }
                await next();
                return;
            }

            var jwtUser = http.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var sessionUser = await GetSessionUserAsync(sid.ToString().Trim());
            var ok = sessionUser != null && sessionUser == jwtUser;

            if (!ok)
            {
                _log.LogWarning("Session {Sid} does not belong to JWT user {User} (session user: {SUser})", sid.ToString(), jwtUser, sessionUser);
                if (enforce)
                {
                    ctx.Result = new ObjectResult(new { status = false, code = 403, message = "Session does not belong to the signed-in user." }) { StatusCode = 403 };
                    return;
                }
            }

            if (int.TryParse(sid.ToString().Trim(), out var headerSessionId) && headerSessionId > 0)
            {
                BindSessionArguments(ctx, headerSessionId);
            }

            await next();
        }

        // =====================================================
        // sessionId from the URL / body -> header session id
        // =====================================================

        private static bool IsSessionName(string? name) =>
            string.Equals(name, "sessionId", StringComparison.OrdinalIgnoreCase);

        private static bool HasSessionArgument(ActionExecutingContext ctx)
        {
            foreach (var arg in ctx.ActionArguments)
            {
                if (IsSessionName(arg.Key) && arg.Value != null && Convert.ToString(arg.Value) is string v && v != "0" && v != "") return true;
                var prop = FindSessionProperty(arg.Value);
                if (prop != null && Convert.ToString(prop.GetValue(arg.Value)) is string pv && pv != "0" && pv != "") return true;
            }
            return false;
        }

        private static void BindSessionArguments(ActionExecutingContext ctx, int headerSessionId)
        {
            foreach (var key in ctx.ActionArguments.Keys.ToList())
            {
                var value = ctx.ActionArguments[key];

                if (IsSessionName(key))
                {
                    var converted = ConvertTo(headerSessionId, value?.GetType()
                        ?? ctx.ActionDescriptor.Parameters.FirstOrDefault(p => p.Name == key)?.ParameterType);
                    if (converted != null) ctx.ActionArguments[key] = converted;
                    continue;
                }

                var prop = FindSessionProperty(value);
                if (prop != null)
                {
                    var converted = ConvertTo(headerSessionId, prop.PropertyType);
                    if (converted != null) prop.SetValue(value, converted);
                }
            }
        }

        private static PropertyInfo? FindSessionProperty(object? value)
        {
            if (value == null || value is string || value.GetType().IsPrimitive) return null;
            return value.GetType()
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .FirstOrDefault(p => IsSessionName(p.Name) && p.CanWrite && ConvertTo(1, p.PropertyType) != null);
        }

        private static object? ConvertTo(int sessionId, Type? type)
        {
            var t = type == null ? null : Nullable.GetUnderlyingType(type) ?? type;
            if (t == typeof(int)) return sessionId;
            if (t == typeof(long)) return (long)sessionId;
            if (t == typeof(short)) return sessionId <= short.MaxValue ? (short)sessionId : null;
            if (t == typeof(string)) return sessionId.ToString();
            return null;
        }

        private async Task<string?> GetSessionUserAsync(string sessionId)
        {
            var key = "sess-user:" + sessionId;
            if (_cache.TryGetValue(key, out string? cached)) return cached;

            string? user = null;
            try
            {
                // FIX: the procedure's output parameters are not named @LV_* (every DGMS procedure passes them by
                // position), so the named call failed, the user came back null and Enforce returned 403 for every
                // request. Call it exactly like the procedures do: by position.
                const string sql = @"
DECLARE @G SMALLINT, @O SMALLINT, @L SMALLINT, @U INT, @M INT;
IF OBJECT_ID('DGMS.PR_GET_SESSION_DET', 'P') IS NOT NULL
    EXEC DGMS.PR_GET_SESSION_DET @SID, @G OUTPUT, @O OUTPUT, @L OUTPUT, @U OUTPUT, @M OUTPUT;
ELSE
    EXEC PR_GET_SESSION_DET @SID, @G OUTPUT, @O OUTPUT, @L OUTPUT, @U OUTPUT, @M OUTPUT;
SELECT @U;";
                await using var con = new SqlConnection(_cfg.GetConnectionString("SqlServer"));
                await using var cmd = new SqlCommand(sql, con) { CommandType = CommandType.Text };
                if (int.TryParse(sessionId, out var sidNum))
                    cmd.Parameters.Add(new SqlParameter("@SID", SqlDbType.Int) { Value = sidNum });
                else
                    cmd.Parameters.Add(new SqlParameter("@SID", SqlDbType.VarChar, 100) { Value = sessionId });
                await con.OpenAsync();
                var result = await cmd.ExecuteScalarAsync();
                user = result == null || result == DBNull.Value ? null : Convert.ToString(result);
            }
            catch (Exception ex) { _log.LogError(ex, "Session lookup failed"); return null; }

            // only a found session is cached (a session that was not found yet is looked up again next time)
            if (user != null) _cache.Set(key, user, TimeSpan.FromMinutes(2));
            return user;
        }
    }
}
