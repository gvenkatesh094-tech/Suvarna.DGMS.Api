using Microsoft.Data.SqlClient;
using Suvarna.DGMS.Api.Models;
using System.Data;

namespace Suvarna.DGMS.Api.Repositories
{
    public class OccupancyReportsRepository
    {
        private readonly string _conn;

        public OccupancyReportsRepository(IConfiguration configuration)
        {
            _conn = configuration.GetConnectionString("SqlServer");
        }

        // ==========================================
        // KPI SUMMARY
        // ==========================================

        public OccKpiSummary GetKpiSummary(int sessionId, int? propertyId)
        {
            try
            {
                using SqlConnection conn = new SqlConnection(_conn);
                using SqlCommand cmd = new SqlCommand("DGMS.PR_DGMS_GET_OCC_KPI_SUMMARY", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@IP_SESSION_ID", sessionId);
                AddParameter(cmd, "@IP_PROPERTY_ID", propertyId);

                conn.Open();

                using SqlDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    return new OccKpiSummary
                    {
                        totalRooms = GetInt(dr, "TOTAL_ROOMS"),
                        occupiedRooms = GetInt(dr, "OCCUPIED_ROOMS"),
                        availableRooms = GetInt(dr, "AVAILABLE_ROOMS"),
                        avgStayNights = GetNullableDecimal(dr, "AVG_STAY_NIGHTS")
                    };
                }

                return new OccKpiSummary();
            }
            catch (Exception ex)
            {
                throw new Exception("Error while fetching occupancy KPI summary.", ex);
            }
        }

        // ==========================================
        // OCCUPANCY BY ROOM TYPE
        // ==========================================

        public List<OccRoomTypeStat> GetByRoomType(int sessionId)
        {
            List<OccRoomTypeStat> list = new();

            try
            {
                using SqlConnection conn = new SqlConnection(_conn);
                using SqlCommand cmd = new SqlCommand("DGMS.PR_DGMS_GET_OCC_BY_ROOM_TYPE", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@IP_SESSION_ID", sessionId);

                conn.Open();

                using SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    list.Add(new OccRoomTypeStat
                    {
                        accmTypeId = GetInt(dr, "ACCM_TYPE_ID"),
                        accmTypeName = GetString(dr, "ACCM_TYPE_NM") ?? "",
                        totalRooms = GetInt(dr, "TOTAL_ROOMS"),
                        occupiedRooms = GetInt(dr, "OCCUPIED_ROOMS"),
                        occPct = GetDecimal(dr, "OCC_PCT"),
                        adr = GetDecimal(dr, "ADR"),
                        revPar = GetDecimal(dr, "REVPAR")
                    });
                }

                return list;
            }
            catch (Exception ex)
            {
                throw new Exception("Error while fetching occupancy by room type.", ex);
            }
        }

        // ==========================================
        // CATEGORY BREAKDOWN
        // ==========================================

        public List<OccCategoryStat> GetCategoryBreakdown(int sessionId)
        {
            List<OccCategoryStat> list = new();

            try
            {
                using SqlConnection conn = new SqlConnection(_conn);
                using SqlCommand cmd = new SqlCommand("DGMS.PR_DGMS_GET_OCC_CATEGORY_BREAKDOWN", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@IP_SESSION_ID", sessionId);

                conn.Open();

                using SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    list.Add(new OccCategoryStat
                    {
                        occCatId = GetInt(dr, "OCC_CAT_ID"),
                        categoryName = GetString(dr, "CAT_NM") ?? "",
                        bookingCount = GetInt(dr, "BOOKING_COUNT"),
                        roomNights = GetInt(dr, "ROOM_NIGHTS"),
                        revenue = GetDecimal(dr, "REVENUE")
                    });
                }

                return list;
            }
            catch (Exception ex)
            {
                throw new Exception("Error while fetching occupant category breakdown.", ex);
            }
        }

        // ==========================================
        // TREND
        // ==========================================

        public List<OccTrendItem> GetTrend(int sessionId, int days)
        {
            List<OccTrendItem> list = new();

            try
            {
                using SqlConnection conn = new SqlConnection(_conn);
                using SqlCommand cmd = new SqlCommand("DGMS.PR_DGMS_GET_OCC_TREND", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@IP_SESSION_ID", sessionId);
                cmd.Parameters.AddWithValue("@IP_DAYS", days);

                conn.Open();

                using SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    list.Add(new OccTrendItem
                    {
                        occDate = GetNullableDateTime(dr, "OCC_DATE") ?? DateTime.MinValue,
                        dayLabel = GetString(dr, "DAY_LABEL") ?? "",
                        occPct = GetDecimal(dr, "OCC_PCT")
                    });
                }

                return list;
            }
            catch (Exception ex)
            {
                throw new Exception("Error while fetching occupancy trend.", ex);
            }
        }

        // ==========================================
        // ROOM STATUS GRID
        // ==========================================

        public List<OccRoomStatusItem> GetRoomStatusGrid(int sessionId, int? propertyId)
        {
            List<OccRoomStatusItem> list = new();

            try
            {
                using SqlConnection conn = new SqlConnection(_conn);
                using SqlCommand cmd = new SqlCommand("DGMS.PR_DGMS_GET_OCC_ROOM_STATUS_GRID", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@IP_SESSION_ID", sessionId);
                AddParameter(cmd, "@IP_PROPERTY_ID", propertyId);

                conn.Open();

                using SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    list.Add(new OccRoomStatusItem
                    {
                        roomId = GetInt(dr, "ROOM_ID"),
                        roomNo = GetString(dr, "ROOM_NO") ?? "",
                        roomStatus = GetString(dr, "ROOM_STATUS_CD") ?? ""
                    });
                }

                return list;
            }
            catch (Exception ex)
            {
                throw new Exception("Error while fetching room status grid.", ex);
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

        private static decimal? GetNullableDecimal(SqlDataReader dr, string column)
        {
            return dr[column] == DBNull.Value ? null : Convert.ToDecimal(dr[column]);
        }

        private static DateTime? GetNullableDateTime(SqlDataReader dr, string column)
        {
            return dr[column] == DBNull.Value ? null : Convert.ToDateTime(dr[column]);
        }
    }
}
