using Microsoft.Data.SqlClient;
using Suvarna.DGMS.Api.Models;
using System.Data;
using System.Globalization;

namespace Suvarna.DGMS.Api.Repositories
{
    /// <summary>
    /// Unified user alerts (28-Oct). Procs: DGMS.PR_DGMS_GET_USER_ALERTS, DGMS.PR_DGMS_SET_USER_ALERT_READ
    /// (scripts/alerts/2026-10-28_dgms_user_alerts.sql).
    /// </summary>
    public class AlertsRepository
    {
        private readonly string _conn;

        public AlertsRepository(IConfiguration configuration)
        {
            _conn = configuration.GetConnectionString("SqlServer");
        }

        // ==========================================
        // LIST + SUMMARY
        // ==========================================

        public UserAlertList GetAlerts(AlertListRequest req)
        {
            var result = new UserAlertList { PageNo = req.PageNo, PageSize = req.PageSize };

            try
            {
                using SqlConnection conn = new SqlConnection(_conn);
                using SqlCommand cmd = new SqlCommand("DGMS.PR_DGMS_GET_USER_ALERTS", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 60;

                cmd.Parameters.AddWithValue("@IP_SESSION_ID", req.SessionId);
                cmd.Parameters.Add("@IP_ROUTES", SqlDbType.VarChar, 4000).Value = (object?)JoinRoutes(req.Routes) ?? DBNull.Value;
                cmd.Parameters.Add("@IP_STATUS", SqlDbType.VarChar, 10).Value = (object?)req.Status ?? "ALL";
                cmd.Parameters.Add("@IP_SHOW", SqlDbType.VarChar, 10).Value = (object?)req.Show ?? "ALL";
                cmd.Parameters.Add("@IP_CATEGORY", SqlDbType.VarChar, 20).Value = (object?)NullIfEmpty(req.Category) ?? DBNull.Value;
                cmd.Parameters.Add("@IP_SEVERITY", SqlDbType.VarChar, 10).Value = (object?)NullIfEmpty(req.Severity) ?? DBNull.Value;
                cmd.Parameters.Add("@IP_SEARCH", SqlDbType.NVarChar, 100).Value = (object?)NullIfEmpty(req.Search) ?? DBNull.Value;
                cmd.Parameters.Add("@IP_FROM_DT", SqlDbType.Date).Value = (object?)ParseDate(req.FromDate) ?? DBNull.Value;
                cmd.Parameters.Add("@IP_TO_DT", SqlDbType.Date).Value = (object?)ParseDate(req.ToDate) ?? DBNull.Value;
                cmd.Parameters.AddWithValue("@IP_PAGE_NO", req.PageNo);
                cmd.Parameters.AddWithValue("@IP_PAGE_SIZE", req.PageSize);

                conn.Open();

                using SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    result.TotalRows = GetInt(dr, "TOTAL_ROWS");

                    result.Items.Add(new UserAlertItem
                    {
                        AlertId = GetLong(dr, "ALERT_ID"),
                        AlertCode = GetString(dr, "ALERT_CODE") ?? "",
                        Category = GetString(dr, "CATEGORY") ?? "",
                        AlertName = GetString(dr, "ALERT_NM") ?? "",
                        Severity = GetString(dr, "SEVERITY") ?? "INFO",
                        Title = GetString(dr, "TITLE") ?? "",
                        Message = GetString(dr, "MESSAGE"),
                        RoomNo = GetString(dr, "ROOM_NO"),
                        GuestName = GetString(dr, "GUEST_NM"),
                        RefType = GetString(dr, "REF_TYPE"),
                        RefId = dr["REF_ID"] == DBNull.Value ? null : Convert.ToInt64(dr["REF_ID"]),
                        RefNo = GetString(dr, "REF_NO"),
                        BookingId = dr["BOOKING_ID"] == DBNull.Value ? null : Convert.ToInt32(dr["BOOKING_ID"]),
                        Route = GetString(dr, "OPEN_ROUTE"),
                        IsPersonal = GetString(dr, "IS_PERSONAL") == "Y",
                        IsRead = GetString(dr, "IS_READ") == "Y",
                        ReadDate = GetNullableDateTime(dr, "READ_DT"),
                        IsResolved = GetString(dr, "IS_RESOLVED") == "Y",
                        ResolvedDate = GetNullableDateTime(dr, "RESOLVED_DT"),
                        CreatedDate = GetNullableDateTime(dr, "CREATE_DT") ?? DateTime.UtcNow,
                        ActorName = GetString(dr, "ACTOR_NM")
                    });
                }

                if (dr.NextResult() && dr.Read())
                {
                    result.Summary = new UserAlertSummary
                    {
                        Total = GetInt(dr, "TOTAL_COUNT"),
                        Unread = GetInt(dr, "UNREAD_COUNT"),
                        Attention = GetInt(dr, "ATTENTION_COUNT"),
                        Critical = GetInt(dr, "CRITICAL_COUNT"),
                        Warning = GetInt(dr, "WARNING_COUNT"),
                        Info = GetInt(dr, "INFO_COUNT"),
                        LatestDate = GetNullableDateTime(dr, "LATEST_DT"),
                        ByCategory = new Dictionary<string, int>
                        {
                            ["FRONT_OFFICE"] = GetInt(dr, "FRONT_OFFICE_COUNT"),
                            ["HOUSEKEEPING"] = GetInt(dr, "HOUSEKEEPING_COUNT"),
                            ["BILLING"] = GetInt(dr, "BILLING_COUNT"),
                            ["MAINTENANCE"] = GetInt(dr, "MAINTENANCE_COUNT"),
                            ["SECURITY"] = GetInt(dr, "SECURITY_COUNT")
                        }
                    };
                }

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception("Error while fetching alerts.", ex);
            }
        }

        // ==========================================
        // READ / UNREAD / READ ALL
        // ==========================================

        public (bool ok, string message, int updated) SetRead(AlertReadRequest req)
        {
            try
            {
                using SqlConnection conn = new SqlConnection(_conn);
                using SqlCommand cmd = new SqlCommand("DGMS.PR_DGMS_SET_USER_ALERT_READ", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@IP_SESSION_ID", req.SessionId);
                cmd.Parameters.Add("@IP_ACTION", SqlDbType.VarChar, 10).Value = (req.Action ?? "").Trim().ToUpperInvariant();
                cmd.Parameters.Add("@IP_ALERT_ID", SqlDbType.BigInt).Value = (object?)req.AlertId ?? DBNull.Value;
                cmd.Parameters.Add("@IP_ROUTES", SqlDbType.VarChar, 4000).Value = (object?)JoinRoutes(req.Routes) ?? DBNull.Value;
                cmd.Parameters.Add("@IP_CATEGORY", SqlDbType.VarChar, 20).Value = (object?)NullIfEmpty(req.Category) ?? DBNull.Value;

                var status = cmd.Parameters.Add("@OUT_STATUS", SqlDbType.Char, 1);
                status.Direction = ParameterDirection.Output;
                var message = cmd.Parameters.Add("@OUT_MESSAGE", SqlDbType.VarChar, 500);
                message.Direction = ParameterDirection.Output;
                var count = cmd.Parameters.Add("@OUT_COUNT", SqlDbType.Int);
                count.Direction = ParameterDirection.Output;

                conn.Open();
                cmd.ExecuteNonQuery();

                return (
                    Convert.ToString(status.Value) == "S",
                    Convert.ToString(message.Value) ?? "",
                    count.Value == DBNull.Value ? 0 : Convert.ToInt32(count.Value)
                );
            }
            catch (Exception ex)
            {
                throw new Exception("Error while updating alert status.", ex);
            }
        }

        // ==========================================
        // HELPERS
        // ==========================================

        /// <summary>Only plain route names ("/checkout") reach the procedure.</summary>
        private static string? JoinRoutes(List<string>? routes)
        {
            if (routes == null || routes.Count == 0) return null;

            var clean = routes
                .Select(r => (r ?? "").Trim().Split('?')[0].Split('#')[0])
                .Where(r => r.Length > 0)
                .Select(r => r.StartsWith("/") ? r : "/" + r)
                .Where(r => r.Length <= 100 && r.All(c => char.IsLetterOrDigit(c) || c == '/' || c == '-'))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            if (clean.Count == 0) return null;

            var joined = string.Join(",", clean);
            return joined.Length > 4000 ? joined[..4000] : joined;
        }

        private static string? NullIfEmpty(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();

        private static DateTime? ParseDate(string? value)
        {
            if (string.IsNullOrWhiteSpace(value)) return null;
            return DateTime.TryParseExact(value.Trim()[..Math.Min(10, value.Trim().Length)], "yyyy-MM-dd",
                CultureInfo.InvariantCulture, DateTimeStyles.None, out var d) ? d.Date : null;
        }

        private static string? GetString(SqlDataReader dr, string column)
        {
            return dr[column] == DBNull.Value ? null : Convert.ToString(dr[column]);
        }

        private static int GetInt(SqlDataReader dr, string column)
        {
            return dr[column] == DBNull.Value ? 0 : Convert.ToInt32(dr[column]);
        }

        private static long GetLong(SqlDataReader dr, string column)
        {
            return dr[column] == DBNull.Value ? 0 : Convert.ToInt64(dr[column]);
        }

        private static DateTime? GetNullableDateTime(SqlDataReader dr, string column)
        {
            // DB stores UTC; the JSON converter writes it with 'Z'
            return dr[column] == DBNull.Value ? null : DateTime.SpecifyKind(Convert.ToDateTime(dr[column]), DateTimeKind.Utc);
        }
    }
}
