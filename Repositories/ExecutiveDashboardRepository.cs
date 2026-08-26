using Microsoft.Data.SqlClient;
using Suvarna.DGMS.Api.Models;
using System.Data;

namespace Suvarna.DGMS.Api.Repositories
{
    public class ExecutiveDashboardRepository
    {
        private readonly string _conn;

        public ExecutiveDashboardRepository(IConfiguration configuration)
        {
            _conn = configuration.GetConnectionString("SqlServer");
        }

        // ==========================================
        // KPI SUMMARY
        // ==========================================

        public ExecKpiSummary GetKpiSummary(int sessionId, int? propertyId)
        {
            try
            {
                using SqlConnection conn = new SqlConnection(_conn);
                using SqlCommand cmd = new SqlCommand("DGMS.PR_DGMS_GET_EXEC_KPI_SUMMARY", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@IP_SESSION_ID", sessionId);
                AddParameter(cmd, "@IP_PROPERTY_ID", propertyId);

                conn.Open();

                using SqlDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    return new ExecKpiSummary
                    {
                        totalRooms = GetInt(dr, "TOTAL_ROOMS"),
                        occupiedRooms = GetInt(dr, "OCCUPIED_ROOMS"),
                        mtdRevenueExclGst = GetDecimal(dr, "MTD_REVENUE_EXCL_GST"),
                        gstMtd = GetDecimal(dr, "GST_MTD"),
                        activeGuests = GetInt(dr, "ACTIVE_GUESTS"),
                        hkSlaBreaches = GetInt(dr, "HK_SLA_BREACHES"),
                        criticalMaintCount = GetInt(dr, "CRITICAL_MAINT_COUNT")
                    };
                }

                return new ExecKpiSummary();
            }
            catch (Exception ex)
            {
                throw new Exception("Error while fetching executive KPI summary.", ex);
            }
        }

        // ==========================================
        // PROPERTY PERFORMANCE
        // ==========================================

        public List<ExecPropertyPerformance> GetPropertyPerformance(int sessionId)
        {
            List<ExecPropertyPerformance> list = new();

            try
            {
                using SqlConnection conn = new SqlConnection(_conn);
                using SqlCommand cmd = new SqlCommand("DGMS.PR_DGMS_GET_EXEC_PROPERTY_PERFORMANCE", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@IP_SESSION_ID", sessionId);

                conn.Open();

                using SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    list.Add(new ExecPropertyPerformance
                    {
                        propertyId = GetInt(dr, "PROPERTY_ID"),
                        propertyName = GetString(dr, "PROPERTY_NM") ?? "",
                        totalRooms = GetInt(dr, "TOTAL_ROOMS"),
                        occupiedRooms = GetInt(dr, "OCCUPIED_ROOMS"),
                        occPct = GetDecimal(dr, "OCC_PCT"),
                        adr = GetDecimal(dr, "ADR"),
                        revPar = GetDecimal(dr, "REVPAR"),
                        mtdRevenue = GetDecimal(dr, "MTD_REVENUE"),
                        gstMtd = GetDecimal(dr, "GST_MTD")
                    });
                }

                return list;
            }
            catch (Exception ex)
            {
                throw new Exception("Error while fetching property performance.", ex);
            }
        }

        // ==========================================
        // REVENUE TREND
        // ==========================================

        public List<ExecRevenueTrendItem> GetRevenueTrend(int sessionId, int days)
        {
            List<ExecRevenueTrendItem> list = new();

            try
            {
                using SqlConnection conn = new SqlConnection(_conn);
                using SqlCommand cmd = new SqlCommand("DGMS.PR_DGMS_GET_EXEC_REVENUE_TREND", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@IP_SESSION_ID", sessionId);
                cmd.Parameters.AddWithValue("@IP_DAYS", days);

                conn.Open();

                using SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    list.Add(new ExecRevenueTrendItem
                    {
                        revenueDate = GetNullableDateTime(dr, "REVENUE_DATE") ?? DateTime.MinValue,
                        dayLabel = GetString(dr, "DAY_LABEL") ?? "",
                        subtotalAmount = GetDecimal(dr, "SUBTOTAL_AMT")
                    });
                }

                return list;
            }
            catch (Exception ex)
            {
                throw new Exception("Error while fetching revenue trend.", ex);
            }
        }

        // ==========================================
        // GST SLAB SUMMARY
        // ==========================================

        public List<ExecGstSlabItem> GetGstSlabSummary(int sessionId)
        {
            List<ExecGstSlabItem> list = new();

            try
            {
                using SqlConnection conn = new SqlConnection(_conn);
                using SqlCommand cmd = new SqlCommand("DGMS.PR_DGMS_GET_EXEC_GST_SLAB_SUMMARY", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@IP_SESSION_ID", sessionId);

                conn.Open();

                using SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    list.Add(new ExecGstSlabItem
                    {
                        slabLabel = GetString(dr, "SLAB_LABEL") ?? "",
                        slabPct = GetDecimal(dr, "SLAB_PCT"),
                        taxableValue = GetDecimal(dr, "TAXABLE_VALUE"),
                        cgst = GetDecimal(dr, "CGST"),
                        sgst = GetDecimal(dr, "SGST"),
                        totalGst = GetDecimal(dr, "TOTAL_GST"),
                        isTotal = GetInt(dr, "IS_TOTAL") == 1
                    });
                }

                return list;
            }
            catch (Exception ex)
            {
                throw new Exception("Error while fetching GST slab summary.", ex);
            }
        }

        // ==========================================
        // ALERTS
        // ==========================================

        public List<ExecAlertItem> GetAlerts(int sessionId, int topN)
        {
            List<ExecAlertItem> list = new();

            try
            {
                using SqlConnection conn = new SqlConnection(_conn);
                using SqlCommand cmd = new SqlCommand("DGMS.PR_DGMS_GET_EXEC_ALERTS", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@IP_SESSION_ID", sessionId);
                cmd.Parameters.AddWithValue("@IP_TOP_N", topN);

                conn.Open();

                using SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    list.Add(new ExecAlertItem
                    {
                        dotColor = GetString(dr, "DOT_COLOR") ?? "",
                        title = GetString(dr, "TITLE") ?? "",
                        subtitle = GetString(dr, "SUBTITLE") ?? "",
                        route = GetString(dr, "ROUTE")
                    });
                }

                return list;
            }
            catch (Exception ex)
            {
                throw new Exception("Error while fetching executive alerts.", ex);
            }
        }

        // ==========================================
        // OPERATIONAL SNAPSHOT
        // ==========================================

        public ExecOperationalSnapshot GetOperationalSnapshot(int sessionId)
        {
            try
            {
                using SqlConnection conn = new SqlConnection(_conn);
                using SqlCommand cmd = new SqlCommand("DGMS.PR_DGMS_GET_EXEC_OPERATIONAL_SNAPSHOT", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@IP_SESSION_ID", sessionId);

                conn.Open();

                using SqlDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    return new ExecOperationalSnapshot
                    {
                        hkPending = GetInt(dr, "HK_PENDING"),
                        hkInProgress = GetInt(dr, "HK_IN_PROGRESS"),
                        maintOpen = GetInt(dr, "MAINT_OPEN"),
                        maintCritical = GetInt(dr, "MAINT_CRITICAL"),
                        activeVisitorPasses = GetInt(dr, "ACTIVE_VISITOR_PASSES"),
                        blacklistEntries = GetInt(dr, "BLACKLIST_ENTRIES"),
                        depositBalanceHeld = GetDecimal(dr, "DEPOSIT_BALANCE_HELD")
                    };
                }

                return new ExecOperationalSnapshot();
            }
            catch (Exception ex)
            {
                throw new Exception("Error while fetching operational snapshot.", ex);
            }
        }

        // ==========================================
        // COMMON HELPERS
        // ==========================================

        private static void AddParameter(SqlCommand cmd, string name, object? value)
        {
            cmd.Parameters.AddWithValue(name, value ?? DBNull.Value);
        }

        private static string? GetString(SqlDataReader dr, string column)
        {
            return dr[column] == DBNull.Value ? null : Convert.ToString(dr[column]);
        }

        private static int GetInt(SqlDataReader dr, string column)
        {
            return dr[column] == DBNull.Value ? 0 : Convert.ToInt32(dr[column]);
        }

        private static decimal GetDecimal(SqlDataReader dr, string column)
        {
            return dr[column] == DBNull.Value ? 0 : Convert.ToDecimal(dr[column]);
        }

        private static DateTime? GetNullableDateTime(SqlDataReader dr, string column)
        {
            return dr[column] == DBNull.Value ? null : Convert.ToDateTime(dr[column]);
        }
    }
}
