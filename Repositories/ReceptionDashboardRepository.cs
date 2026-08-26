using Microsoft.Data.SqlClient;
using Suvarna.DGMS.Api.Models;
using System.Data;

namespace Suvarna.DGMS.Api.Repositories
{
    public class ReceptionDashboardRepository
    {
        private readonly string _conn;

        public ReceptionDashboardRepository(IConfiguration configuration)
        {
            _conn = configuration.GetConnectionString("SqlServer");
        }

        // ==========================================
        // SUMMARY
        // ==========================================

        public ReceptionDashboardSummary GetSummary(int sessionId)
        {
            try
            {
                using SqlConnection conn = new SqlConnection(_conn);
                using SqlCommand cmd = new SqlCommand("DGMS.PR_DGMS_GET_RECEPTION_DASHBOARD_SUMMARY", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IP_SESSION_ID", sessionId);

                conn.Open();

                using SqlDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    return new ReceptionDashboardSummary
                    {
                        totalRooms = GetInt(dr, "TOTAL_ROOMS"),
                        occupiedRooms = GetInt(dr, "OCCUPIED_ROOMS"),
                        availableRooms = GetInt(dr, "AVAILABLE_ROOMS"),
                        pendingCheckin = GetInt(dr, "PENDING_CHECKIN"),
                        overdueCheckin = GetInt(dr, "OVERDUE_CHECKIN"),
                        dueCheckout = GetInt(dr, "DUE_CHECKOUT"),
                        overdueCheckout = GetInt(dr, "OVERDUE_CHECKOUT"),
                        reservations = GetInt(dr, "RESERVATIONS"),
                        arrivingTomorrow = GetInt(dr, "ARRIVING_TOMORROW"),
                        occupancyRate = GetDecimal(dr, "OCCUPANCY_RATE"),
                        todayRevenue = GetDecimal(dr, "TODAY_REVENUE"),
                        mtdRevenue = GetDecimal(dr, "MTD_REVENUE")
                    };
                }

                return new ReceptionDashboardSummary();
            }
            catch (Exception ex)
            {
                throw new Exception("Error while fetching reception dashboard summary.", ex);
            }
        }

        // ==========================================
        // ACTIVE BOOKINGS
        // ==========================================

        public List<ReceptionActiveBooking> GetActiveBookings(int sessionId)
        {
            List<ReceptionActiveBooking> list = new();

            try
            {
                using SqlConnection conn = new SqlConnection(_conn);
                using SqlCommand cmd = new SqlCommand("DGMS.PR_DGMS_GET_RECEPTION_ACTIVE_BOOKINGS", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IP_SESSION_ID", sessionId);

                conn.Open();

                using SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    list.Add(new ReceptionActiveBooking
                    {
                        bookingId = GetInt(dr, "BOOKING_ID"),
                        bookingRef = GetString(dr, "BOOKING_REF") ?? "",
                        guestName = GetString(dr, "GUEST_NAME") ?? "",
                        mobileNo = GetString(dr, "MOBILE_NO") ?? "",
                        roomNo = GetString(dr, "ROOM_NO") ?? "",
                        categoryName = GetString(dr, "CATEGORY_NAME") ?? "",
                        checkinDt = GetNullableDateTime(dr, "CHECKIN_DT"),
                        expectedCheckoutDt = GetNullableDateTime(dr, "EXPECTED_CHECKOUT_DT"),
                        chargedAmount = GetDecimal(dr, "CHARGED_AMOUNT"),
                        gstPercent = GetDecimal(dr, "GST_PERCENT"),
                        depositAmount = GetDecimal(dr, "DEPOSIT_AMOUNT")
                    });
                }

                return list;
            }
            catch (Exception ex)
            {
                throw new Exception("Error while fetching active bookings.", ex);
            }
        }

        // ==========================================
        // ROOM STATUS
        // ==========================================

        public List<ReceptionRoomStatus> GetRoomStatus(int sessionId)
        {
            List<ReceptionRoomStatus> list = new();

            try
            {
                using SqlConnection conn = new SqlConnection(_conn);
                using SqlCommand cmd = new SqlCommand("DGMS.PR_DGMS_GET_RECEPTION_ROOM_STATUS", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IP_SESSION_ID", sessionId);

                conn.Open();

                using SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    list.Add(new ReceptionRoomStatus
                    {
                        roomId = GetInt(dr, "ROOM_ID"),
                        roomNo = GetString(dr, "ROOM_NO") ?? "",
                        roomName = GetString(dr, "ROOM_NAME"),
                        roomStatus = GetString(dr, "ROOM_STATUS") ?? ""
                    });
                }

                return list;
            }
            catch (Exception ex)
            {
                throw new Exception("Error while fetching room status.", ex);
            }
        }

        // ==========================================
        // ALERTS
        // ==========================================

        public List<ReceptionAlert> GetAlerts(int sessionId)
        {
            List<ReceptionAlert> list = new();

            try
            {
                using SqlConnection conn = new SqlConnection(_conn);
                using SqlCommand cmd = new SqlCommand("DGMS.PR_DGMS_GET_RECEPTION_ALERTS", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IP_SESSION_ID", sessionId);

                conn.Open();

                using SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    list.Add(new ReceptionAlert
                    {
                        alertId = GetInt(dr, "ALERT_ID"),
                        alertColor = GetString(dr, "ALERT_COLOR") ?? "",
                        alertTitle = GetString(dr, "ALERT_TITLE") ?? "",
                        alertSubtitle = GetString(dr, "ALERT_SUBTITLE") ?? "",
                        actionText = GetString(dr, "ACTION_TEXT") ?? ""
                    });
                }

                return list;
            }
            catch (Exception ex)
            {
                throw new Exception("Error while fetching alerts.", ex);
            }
        }

        // ==========================================
        // REVENUE
        // ==========================================

        public List<ReceptionRevenueItem> GetRevenue(int sessionId)
        {
            List<ReceptionRevenueItem> list = new();

            try
            {
                using SqlConnection conn = new SqlConnection(_conn);
                using SqlCommand cmd = new SqlCommand("DGMS.PR_DGMS_GET_RECEPTION_REVENUE", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IP_SESSION_ID", sessionId);

                conn.Open();

                using SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    list.Add(new ReceptionRevenueItem
                    {
                        dayName = GetString(dr, "DAY_NAME") ?? "",
                        amount = GetDecimal(dr, "AMOUNT")
                    });
                }

                return list;
            }
            catch (Exception ex)
            {
                throw new Exception("Error while fetching revenue.", ex);
            }
        }

        // ==========================================
        // COMMON HELPERS
        // ==========================================

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
