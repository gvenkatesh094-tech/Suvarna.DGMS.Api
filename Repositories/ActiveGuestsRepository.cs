using Microsoft.Data.SqlClient;
using Suvarna.DGMS.Api.Models;
using System.Data;

namespace Suvarna.DGMS.Api.Repositories
{
    public class ActiveGuestsRepository
    {
        private readonly string _conn;

        public ActiveGuestsRepository(IConfiguration configuration)
        {
            _conn = configuration.GetConnectionString("SqlServer");
        }

        // ==========================================
        // GET ACTIVE (CHECKED-IN) GUESTS
        // ==========================================

        public List<ActiveGuestRow> GetActiveGuests(int sessionId, int? propertyId)
        {
            List<ActiveGuestRow> list = new();

            try
            {
                using SqlConnection conn = new SqlConnection(_conn);
                using SqlCommand cmd = new SqlCommand("DGMS.PR_DGMS_GET_ACTIVE_GUESTS", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IP_SESSION_ID", sessionId);
                cmd.Parameters.AddWithValue("@IP_PROPERTY_ID", (object?)propertyId ?? DBNull.Value);
                conn.Open();

                using SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    list.Add(new ActiveGuestRow
                    {
                        bookingId = GetInt(dr, "BOOKING_ID"),
                        bookingRef = GetString(dr, "BOOKING_REF") ?? "",
                        guestNm = GetString(dr, "GUEST_NM") ?? "",
                        mobileNo = GetString(dr, "MOBILE_NO") ?? "",
                        roomNo = GetString(dr, "ROOM_NO") ?? "",
                        categoryNm = GetString(dr, "CATEGORY_NM") ?? "",
                        checkinDt = GetNullableDateTime(dr, "CHECKIN_DT"),
                        checkoutDt = GetNullableDateTime(dr, "CHECKOUT_DT"),
                        nights = GetInt(dr, "NIGHTS"),
                        dailyRate = GetDecimal(dr, "DAILY_RATE"),
                        gstPct = GetDecimal(dr, "GST_PCT"),
                        chargedAmt = GetDecimal(dr, "CHARGED_AMT"),
                        depositAmt = GetDecimal(dr, "DEPOSIT_AMT"),
                        umrNo = GetString(dr, "UMR_NO"),
                        admnNo = GetString(dr, "ADMN_NO"),
                        wardNm = GetString(dr, "WARD_NM"),
                        corpNm = GetString(dr, "CORP_NM")
                    });
                }

                return list;
            }
            catch (Exception ex)
            {
                throw new Exception("Error while fetching active guests.", ex);
            }
        }

        // ==========================================
        // COMMON HELPERS (same pattern as CheckinRepository)
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
