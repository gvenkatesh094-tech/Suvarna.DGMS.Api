using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Caching.Memory;
using Suvarna.DGMS.Api.Helpers;
using Suvarna.DGMS.Api.Models;
using System.Data;

namespace Suvarna.DGMS.Api.Services
{
    /// <summary>
    /// NEW (23-Oct): PayLink settings of the login location, from System Parameters (DGMS.DGM_SYS_PARAM,
    /// codes DGMS_PAYLINK_*) instead of appsettings.json. Read once per request, cached 1 minute per session,
    /// so a change on the System Parameters screen is used within a minute without restarting the API.
    /// POS is off for a location until its URLs, user name and password are filled in.
    /// </summary>
    public class PayLinkSettings
    {
        public const string Prefix = "DGMS_PAYLINK_";

        private readonly string _connectionString = "";
        private readonly SessionContext? _session;
        private readonly IMemoryCache? _cache;
        private readonly ILogger<PayLinkSettings>? _log;
        private PayLinkOptions? _current;

        /// <summary>Fixed settings (unit tests).</summary>
        public PayLinkSettings(PayLinkOptions fixedOptions) => _current = fixedOptions;

        public PayLinkSettings(IConfiguration configuration, SessionContext session, IMemoryCache cache, ILogger<PayLinkSettings> log)
        {
            _connectionString = configuration.GetConnectionString("SqlServer")
                ?? throw new InvalidOperationException("Connection string 'SqlServer' is missing.");
            _session = session;
            _cache = cache;
            _log = log;
        }

        public PayLinkOptions Current => _current ??= Load();

        private PayLinkOptions Load()
        {
            if (_session == null || _cache == null || !int.TryParse(_session.SessionId, out var sid) || sid <= 0)
                return new PayLinkOptions { Enabled = false };

            return _cache.GetOrCreate("PayLinkSettings:" + sid, entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(1);
                try
                {
                    using var con = new SqlConnection(_connectionString);
                    var rows = con.Query<(string PARAM_CD, string? PARAM_VALUE)>(
                        "DGMS.PR_DGMS_POS_GET_SETTINGS", new { IP_SESSION_ID = sid }, commandType: CommandType.StoredProcedure);
                    return Build(rows.ToDictionary(r => r.PARAM_CD.Trim().ToUpperInvariant(), r => (r.PARAM_VALUE ?? "").Trim()));
                }
                catch (Exception ex)
                {
                    _log?.LogError(ex, "PayLink settings could not be read from System Parameters; POS is off for this request");
                    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(5);
                    return new PayLinkOptions { Enabled = false };
                }
            })!;
        }

        /// <summary>System Parameter values -> settings (missing / wrong values fall back to the defaults).</summary>
        public static PayLinkOptions Build(IReadOnlyDictionary<string, string> p)
        {
            string S(string code, string def = "") => p.TryGetValue(Prefix + code, out var v) && v.Length > 0 ? v : def;
            int I(string code, int def) => int.TryParse(S(code), out var v) && v >= 0 ? v : def;
            bool B(string code, bool def)
            {
                var v = S(code).ToUpperInvariant();
                return v is "Y" or "YES" or "TRUE" or "1" ? true : v is "N" or "NO" or "FALSE" or "0" ? false : def;
            }

            var o = new PayLinkOptions
            {
                AuthUrl = S("AUTH_URL"),
                PushUrl = S("PUSH_URL"),
                StatusUrl = S("STATUS_URL"),
                CancelUrl = S("CANCEL_URL"),
                UserName = S("USERNAME"),
                Password = S("PASSWORD"),
                TimeoutSeconds = Math.Max(5, I("TIMEOUT_SECONDS", 30)),
                TokenCacheMinutes = I("TOKEN_CACHE_MINUTES", 10),
                Currency = S("CURRENCY", "INR"),
                DefaultAutoCancelMinutes = Math.Max(1, I("AUTO_CANCEL_MINUTES", 5)),
                AllowManualCardEntry = B("ALLOW_MANUAL_RRN", true)
            };
            o.TxnTypes = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                ["CARD"] = S("TXNTYPE_CARD", "00"),
                ["UPI"] = S("TXNTYPE_UPI", "01")
            };
            var pending = S("PENDING_MESSAGES");
            if (pending.Length > 0)
                o.PendingMessages = pending.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList();

            o.Enabled = B("ENABLED", true)
                        && o.AuthUrl.Length > 0 && o.PushUrl.Length > 0 && o.StatusUrl.Length > 0
                        && o.UserName.Length > 0 && o.Password.Length > 0;
            return o;
        }
    }
}
