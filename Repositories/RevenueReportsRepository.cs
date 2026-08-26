using Microsoft.Data.SqlClient;
using Suvarna.DGMS.Api.Models;
using System.Data;

namespace Suvarna.DGMS.Api.Repositories
{
    public class RevenueReportsRepository
    {
        private readonly string _conn;

        public RevenueReportsRepository(IConfiguration configuration)
        {
            _conn = configuration.GetConnectionString("SqlServer");
        }

        // ==========================================
        // KPI SUMMARY (MTD)
        // ==========================================

        public RevenueKpiSummary GetKpiSummary(int sessionId, int? propertyId)
        {
            try
            {
                using SqlConnection conn = new SqlConnection(_conn);
                using SqlCommand cmd = new SqlCommand("DGMS.PR_DGMS_GET_REVENUE_KPI_SUMMARY", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@IP_SESSION_ID", sessionId);
                AddParameter(cmd, "@IP_PROPERTY_ID", propertyId);

                conn.Open();

                using SqlDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    return new RevenueKpiSummary
                    {
                        mtdRevenueExclGst = GetDecimal(dr, "MTD_REVENUE_EXCL_GST"),
                        accommodationRevenue = GetDecimal(dr, "ACCOMMODATION_REVENUE"),
                        serviceRevenue = GetDecimal(dr, "SERVICE_REVENUE"),
                        gstCollected = GetDecimal(dr, "GST_COLLECTED"),
                        grossRevenue = GetDecimal(dr, "GROSS_REVENUE"),
                        billCount = GetInt(dr, "BILL_COUNT")
                    };
                }

                return new RevenueKpiSummary();
            }
            catch (Exception ex)
            {
                throw new Exception("Error while fetching revenue KPI summary.", ex);
            }
        }

        // ==========================================
        // DAILY REVENUE
        // ==========================================

        public List<RevenueDailyItem> GetDailyRevenue(int sessionId, DateTime fromDt, DateTime toDt, int? propertyId)
        {
            List<RevenueDailyItem> list = new();

            try
            {
                using SqlConnection conn = new SqlConnection(_conn);
                using SqlCommand cmd = new SqlCommand("DGMS.PR_DGMS_GET_REVENUE_DAILY", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@IP_SESSION_ID", sessionId);
                cmd.Parameters.AddWithValue("@IP_FROM_DT", fromDt.Date);
                cmd.Parameters.AddWithValue("@IP_TO_DT", toDt.Date);
                AddParameter(cmd, "@IP_PROPERTY_ID", propertyId);

                conn.Open();

                using SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    list.Add(new RevenueDailyItem
                    {
                        revenueDate = GetNullableDateTime(dr, "REVENUE_DATE") ?? DateTime.MinValue,
                        dayLabel = GetString(dr, "DAY_LABEL") ?? "",
                        accommodationAmount = GetDecimal(dr, "ACCM_AMT"),
                        foodAmount = GetDecimal(dr, "FOOD_AMT"),
                        laundryAmount = GetDecimal(dr, "LAUNDRY_AMT"),
                        otherAmount = GetDecimal(dr, "OTHER_AMT"),
                        subtotalAmount = GetDecimal(dr, "SUBTOTAL_AMT"),
                        gstAmount = GetDecimal(dr, "GST_AMT"),
                        grossAmount = GetDecimal(dr, "GROSS_AMT")
                    });
                }

                return list;
            }
            catch (Exception ex)
            {
                throw new Exception("Error while fetching daily revenue.", ex);
            }
        }

        // ==========================================
        // PAYMENT MODE SPLIT
        // ==========================================

        public List<RevenuePaymentModeItem> GetPaymentModeSummary(int sessionId, DateTime fromDt, DateTime toDt)
        {
            List<RevenuePaymentModeItem> list = new();

            try
            {
                using SqlConnection conn = new SqlConnection(_conn);
                using SqlCommand cmd = new SqlCommand("DGMS.PR_DGMS_GET_REVENUE_PAYMENT_MODE_SUMMARY", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@IP_SESSION_ID", sessionId);
                cmd.Parameters.AddWithValue("@IP_FROM_DT", fromDt.Date);
                cmd.Parameters.AddWithValue("@IP_TO_DT", toDt.Date);

                conn.Open();

                using SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    list.Add(new RevenuePaymentModeItem
                    {
                        paymentMode = GetString(dr, "PAYMENT_MODE") ?? "",
                        totalAmount = GetDecimal(dr, "TOTAL_AMOUNT"),
                        txnCount = GetInt(dr, "TXN_COUNT")
                    });
                }

                return list;
            }
            catch (Exception ex)
            {
                throw new Exception("Error while fetching payment mode summary.", ex);
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
