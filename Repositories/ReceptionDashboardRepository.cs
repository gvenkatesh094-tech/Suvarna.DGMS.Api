using System;
using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Suvarna.DGMS.Api.Models;

namespace Suvarna.DGMS.Api.Repositories
{
    public class ReceptionDashboardRepository
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;

        public ReceptionDashboardRepository(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("DefaultConnection");
        }

        //=========================================================
        // Dashboard Summary
        //=========================================================

        public ReceptionDashboardSummary GetDashboardSummary()
        {
            ReceptionDashboardSummary summary = new ReceptionDashboardSummary();

            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("DGMS.PR_DGMS_GET_RECEPTION_DASHBOARD_SUMMARY", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    con.Open();

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            summary.TotalRooms =
                                dr["TotalRooms"] == DBNull.Value
                                    ? 0
                                    : Convert.ToInt32(dr["TotalRooms"]);

                            summary.OccupiedRooms =
                                dr["OccupiedRooms"] == DBNull.Value
                                    ? 0
                                    : Convert.ToInt32(dr["OccupiedRooms"]);

                            summary.AvailableRooms =
                                dr["AvailableRooms"] == DBNull.Value
                                    ? 0
                                    : Convert.ToInt32(dr["AvailableRooms"]);

                            summary.PendingCheckIn =
                                dr["PendingCheckIn"] == DBNull.Value
                                    ? 0
                                    : Convert.ToInt32(dr["PendingCheckIn"]);

                            summary.DueCheckOut =
                                dr["DueCheckOut"] == DBNull.Value
                                    ? 0
                                    : Convert.ToInt32(dr["DueCheckOut"]);

                            summary.Reservations =
                                dr["Reservations"] == DBNull.Value
                                    ? 0
                                    : Convert.ToInt32(dr["Reservations"]);

                            summary.OccupancyRate = dr["OccupancyRate"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["OccupancyRate"]);
                        }
                    }
                }
            }

            return summary;
        }
        //=========================================================
        // Active Bookings
        //=========================================================

        public List<ReceptionBooking> GetActiveBookings()
        {
            List<ReceptionBooking> bookings = new List<ReceptionBooking>();

            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("DGMS.PR_DGMS_GET_RECEPTION_ACTIVE_BOOKINGS", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    con.Open();

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            ReceptionBooking booking = new ReceptionBooking
                            {
                                BookingId = dr["BookingId"] == DBNull.Value
                                    ? 0
                                    : Convert.ToInt32(dr["BookingId"]),

                                BookingRef = dr["BookingNo"] == DBNull.Value
                                    ? string.Empty
                                    : dr["BookingNo"].ToString(),

                                GuestName = dr["GuestName"] == DBNull.Value
                                    ? string.Empty
                                    : dr["GuestName"].ToString(),

                                MobileNo = dr["MobileNo"] == DBNull.Value
                                    ? string.Empty
                                    : dr["MobileNo"].ToString(),

                                RoomNo = dr["RoomNo"] == DBNull.Value
                                    ? string.Empty
                                    : dr["RoomNo"].ToString(),

                                Category = dr["AccommodationType"] == DBNull.Value
                                    ? string.Empty
                                    : dr["AccommodationType"].ToString(),

                                CheckInDate = dr["CheckInDate"] == DBNull.Value
                                    ? DateTime.MinValue
                                    : Convert.ToDateTime(dr["CheckInDate"]),

                                CheckOutDate = dr["CheckOutDate"] == DBNull.Value
                                    ? DateTime.MinValue
                                    : Convert.ToDateTime(dr["CheckOutDate"]),

                                ChargedAmount = dr["ChargedAmount"] == DBNull.Value
                                    ? 0
                                    : Convert.ToDecimal(dr["ChargedAmount"]),

                                GstPercent = dr["GSTPercent"] == DBNull.Value
                                    ? 0
                                    : Convert.ToDecimal(dr["GSTPercent"]),

                                DepositAmount = dr["DepositAmount"] == DBNull.Value
                                    ? 0
                                    : Convert.ToDecimal(dr["DepositAmount"])
                            };

                            bookings.Add(booking);
                        }
                    }
                }
            }

            return bookings;
        }
        //=========================================================
        // Room Status
        //=========================================================

        public List<ReceptionRoomStatus> GetRoomStatus()
        {
            List<ReceptionRoomStatus> rooms = new List<ReceptionRoomStatus>();

            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("DGMS.PR_DGMS_GET_RECEPTION_ROOM_STATUS", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    con.Open();

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            ReceptionRoomStatus room = new ReceptionRoomStatus
                            {
                                RoomId = dr["RoomId"] == DBNull.Value
                                    ? 0
                                    : Convert.ToInt32(dr["RoomId"]),

                                RoomNo = dr["RoomNo"] == DBNull.Value
                                    ? string.Empty
                                    : dr["RoomNo"].ToString(),

                                RoomName = dr["RoomName"] == DBNull.Value
                                    ? string.Empty
                                    : dr["RoomName"].ToString(),

                                RoomStatus = dr["RoomStatus"] == DBNull.Value
                                    ? string.Empty
                                    : dr["RoomStatus"].ToString()
                            };

                            rooms.Add(room);
                        }
                    }
                }
            }

            return rooms;
        }
        //=========================================================
        // Alerts
        //=========================================================

        public List<ReceptionAlert> GetAlerts()
        {
            List<ReceptionAlert> alerts = new List<ReceptionAlert>();

            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("DGMS.PR_DGMS_GET_RECEPTION_ALERTS", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    con.Open();

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            ReceptionAlert alert = new ReceptionAlert
                            {
                                AlertColor = dr["AlertColor"] == DBNull.Value
                                    ? string.Empty
                                    : dr["AlertColor"].ToString(),

                                AlertTitle = dr["AlertTitle"] == DBNull.Value
                                    ? string.Empty
                                    : dr["AlertTitle"].ToString(),

                                AlertDescription = dr["AlertDescription"] == DBNull.Value
                                    ? string.Empty
                                    : dr["AlertDescription"].ToString(),

                                ActionText = dr["ActionText"] == DBNull.Value
                                    ? string.Empty
                                    : dr["ActionText"].ToString()
                            };

                            alerts.Add(alert);
                        }
                    }
                }
            }

            return alerts;
        }
        //=========================================================
        // Revenue (Last 7 Days)
        //=========================================================

        public List<ReceptionRevenue> GetRevenue()
        {
            List<ReceptionRevenue> revenue = new List<ReceptionRevenue>();

            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("DGMS.PR_DGMS_GET_RECEPTION_REVENUE", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    con.Open();

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            ReceptionRevenue item = new ReceptionRevenue
                            {
                                DayName = dr["DayName"] == DBNull.Value
                                    ? string.Empty
                                    : dr["DayName"].ToString(),

                                Amount = dr["Amount"] == DBNull.Value
                                    ? 0
                                    : Convert.ToDecimal(dr["Amount"])
                            };

                            revenue.Add(item);
                        }
                    }
                }
            }

            return revenue;
        }
    }
}