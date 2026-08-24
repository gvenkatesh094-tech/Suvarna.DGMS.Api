using Microsoft.Data.SqlClient;
using Suvarna.DGMS.Api.Models;
using System.Data;

namespace Suvarna.DGMS.Api.Repositories
{
    public class RoomTransferRepository
    {
        private readonly string _conn;

        public RoomTransferRepository(IConfiguration configuration)
        {
            _conn = configuration.GetConnectionString("SqlServer");
        }

        // ==========================================
        // "Select Booking" dropdown
        // Reuses PR_DGMS_GET_BILLING_BOOKINGS — same shape (bookingId,
        // bookingRef, guestNm, roomNo, propertyId), single source of truth
        // for "which guests are currently checked in".
        // ==========================================

        public List<TransferBookingLookup> GetBookings(int sessionId, int? propertyId)
        {
            List<TransferBookingLookup> list = new();

            try
            {
                using SqlConnection conn = new SqlConnection(_conn);
                using SqlCommand cmd = new SqlCommand("DGMS.PR_DGMS_GET_BILLING_BOOKINGS", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IP_SESSION_ID", sessionId);
                cmd.Parameters.AddWithValue("@IP_PROPERTY_ID", (object?)propertyId ?? DBNull.Value);
                conn.Open();

                using SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    list.Add(new TransferBookingLookup
                    {
                        bookingId = GetInt(dr, "BOOKING_ID"),
                        bookingRef = GetString(dr, "BOOKING_REF") ?? "",
                        guestNm = GetString(dr, "GUEST_NM") ?? "",
                        roomNo = GetString(dr, "ROOM_NO") ?? "",
                        propertyId = GetInt(dr, "PROPERTY_ID")
                    });
                }

                return list;
            }
            catch (Exception ex)
            {
                throw new Exception("Error while fetching bookings for room transfer.", ex);
            }
        }

        // ==========================================
        // "Transfer To Room" dropdown
        // ==========================================

        public List<TransferAvailableRoom> GetAvailableRooms(int sessionId, int? propertyId, int? excludeRoomId)
        {
            List<TransferAvailableRoom> list = new();

            try
            {
                using SqlConnection conn = new SqlConnection(_conn);
                using SqlCommand cmd = new SqlCommand("DGMS.PR_DGMS_GET_TRANSFER_AVAILABLE_ROOMS", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IP_SESSION_ID", sessionId);
                cmd.Parameters.AddWithValue("@IP_PROPERTY_ID", (object?)propertyId ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@IP_EXCLUDE_ROOM_ID", (object?)excludeRoomId ?? DBNull.Value);
                conn.Open();

                using SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    list.Add(new TransferAvailableRoom
                    {
                        roomId = GetInt(dr, "ROOM_ID"),
                        roomNo = GetString(dr, "ROOM_NO") ?? "",
                        roomType = GetString(dr, "ROOM_TYPE") ?? "",
                        propertyId = GetInt(dr, "PROPERTY_ID"),
                        dailyRate = GetDecimal(dr, "DAILY_RATE"),
                        gstPct = GetDecimal(dr, "GST_PCT")
                    });
                }

                return list;
            }
            catch (Exception ex)
            {
                throw new Exception("Error while fetching available rooms for transfer.", ex);
            }
        }

        // ==========================================
        // "Confirm Transfer" button
        // ==========================================

        public TransferResult ConfirmTransfer(ConfirmTransferRequest request)
        {
            try
            {
                using SqlConnection conn = new SqlConnection(_conn);
                using SqlCommand cmd = new SqlCommand("DGMS.PR_DGMS_CONFIRM_ROOM_TRANSFER", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@IP_SESSION_ID", request.sessionId);
                cmd.Parameters.AddWithValue("@IP_BOOKING_ID", request.bookingId);
                cmd.Parameters.AddWithValue("@IP_TARGET_ROOM_ID", request.targetRoomId);
                cmd.Parameters.AddWithValue("@IP_TRANSFER_DT", request.transferDate);
                cmd.Parameters.AddWithValue("@IP_REASON", (object?)request.reason ?? DBNull.Value);

                var outStatus = new SqlParameter("@OUT_STATUS", SqlDbType.Char, 1) { Direction = ParameterDirection.Output };
                var outMessage = new SqlParameter("@OUT_MESSAGE", SqlDbType.VarChar, 500) { Direction = ParameterDirection.Output };
                cmd.Parameters.Add(outStatus);
                cmd.Parameters.Add(outMessage);

                conn.Open();
                cmd.ExecuteNonQuery();

                return new TransferResult
                {
                    status = Convert.ToString(outStatus.Value) ?? "",
                    message = Convert.ToString(outMessage.Value) ?? ""
                };
            }
            catch (Exception ex)
            {
                throw new Exception("Error while confirming room transfer.", ex);
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
    }
}
