using Microsoft.Data.SqlClient;
using Suvarna.DGMS.Api.Models;
using System.Data;

namespace Suvarna.DGMS.Api.Repositories
{
    public class WaitingListRepository
    {
        private readonly string _conn;

        public WaitingListRepository(IConfiguration configuration)
        {
            _conn = configuration.GetConnectionString("SqlServer");
        }

        // ==========================================
        // GET WAITLIST
        // ==========================================

        public List<WaitlistEntryRow> GetWaitlist(int sessionId)
        {
            List<WaitlistEntryRow> list = new();

            using SqlConnection conn = new SqlConnection(_conn);
            using SqlCommand cmd = new SqlCommand("DGMS.PR_DGMS_GET_WAITLIST", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@IP_SESSION_ID", sessionId);
            conn.Open();

            using SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                list.Add(new WaitlistEntryRow
                {
                    position = GetInt(dr, "POSITION"),
                    waitlistId = GetInt(dr, "WAITLIST_ID"),
                    guestNm = GetString(dr, "GUEST_NM") ?? "",
                    mobileNo = GetString(dr, "MOBILE_NO") ?? "",
                    occCatId = GetInt(dr, "OCC_CAT_ID"),
                    occCatNm = GetString(dr, "OCC_CAT_NM") ?? "",
                    accmTypeId = GetNullableInt(dr, "ACCM_TYPE_ID"),
                    accmTypeNm = GetString(dr, "ACCM_TYPE_NM") ?? "",
                    wardPreference = GetString(dr, "WARD_PREFERENCE") ?? "",
                    requestDt = GetNullableDateTime(dr, "REQUEST_DT") ?? DateTime.MinValue,
                    status = GetString(dr, "STATUS") ?? "",
                    matchedRoomId = GetNullableInt(dr, "MATCHED_ROOM_ID"),
                    matchedRoomNo = GetString(dr, "MATCHED_ROOM_NO"),
                    matchedAt = GetNullableDateTime(dr, "MATCHED_AT")
                });
            }

            return list;
        }

        // ==========================================
        // ADD TO WAITLIST
        // (not wired into the current UI — ready when you add an "Add" flow)
        // ==========================================

        public WaitlistActionResult AddToWaitlist(AddWaitlistRequest request)
        {
            using SqlConnection conn = new SqlConnection(_conn);
            using SqlCommand cmd = new SqlCommand("DGMS.PR_DGMS_ADD_TO_WAITLIST", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@IP_SESSION_ID", request.sessionId);
            cmd.Parameters.AddWithValue("@IP_GUEST_NM", request.guestNm);
            cmd.Parameters.AddWithValue("@IP_MOBILE_NO", request.mobileNo);
            cmd.Parameters.AddWithValue("@IP_OCC_CAT_ID", request.occCatId);
            cmd.Parameters.AddWithValue("@IP_ACCM_TYPE_ID", (object?)request.accmTypeId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@IP_WARD_PREF", string.IsNullOrWhiteSpace(request.wardPreference) ? DBNull.Value : request.wardPreference);

            var outId = new SqlParameter("@OUT_WAITLIST_ID", SqlDbType.Int) { Direction = ParameterDirection.Output };
            var outStatus = new SqlParameter("@OUT_STATUS", SqlDbType.Char, 1) { Direction = ParameterDirection.Output };
            var outMessage = new SqlParameter("@OUT_MESSAGE", SqlDbType.VarChar, 500) { Direction = ParameterDirection.Output };
            cmd.Parameters.Add(outId);
            cmd.Parameters.Add(outStatus);
            cmd.Parameters.Add(outMessage);

            conn.Open();
            cmd.ExecuteNonQuery();

            return new WaitlistActionResult
            {
                waitlistId = outId.Value == DBNull.Value ? 0 : Convert.ToInt32(outId.Value),
                status = Convert.ToString(outStatus.Value) ?? "",
                message = Convert.ToString(outMessage.Value) ?? ""
            };
        }

        // ==========================================
        // NOTIFY
        // TODO: this only marks the entry NOTIFIED in the DB — it does NOT
        // send an SMS. Wire your actual SMS gateway call here (or in the
        // Service layer) once you tell me which provider you use.
        // ==========================================

        public WaitlistActionResult Notify(int sessionId, int waitlistId)
        {
            using SqlConnection conn = new SqlConnection(_conn);
            using SqlCommand cmd = new SqlCommand("DGMS.PR_DGMS_NOTIFY_WAITLIST", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@IP_SESSION_ID", sessionId);
            cmd.Parameters.AddWithValue("@IP_WAITLIST_ID", waitlistId);

            var outStatus = new SqlParameter("@OUT_STATUS", SqlDbType.Char, 1) { Direction = ParameterDirection.Output };
            var outMessage = new SqlParameter("@OUT_MESSAGE", SqlDbType.VarChar, 500) { Direction = ParameterDirection.Output };
            cmd.Parameters.Add(outStatus);
            cmd.Parameters.Add(outMessage);

            conn.Open();
            cmd.ExecuteNonQuery();

            return new WaitlistActionResult
            {
                waitlistId = waitlistId,
                status = Convert.ToString(outStatus.Value) ?? "",
                message = Convert.ToString(outMessage.Value) ?? ""
            };
        }

        // ==========================================
        // REMOVE
        // ==========================================

        public WaitlistActionResult Remove(int sessionId, int waitlistId)
        {
            using SqlConnection conn = new SqlConnection(_conn);
            using SqlCommand cmd = new SqlCommand("DGMS.PR_DGMS_REMOVE_FROM_WAITLIST", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@IP_SESSION_ID", sessionId);
            cmd.Parameters.AddWithValue("@IP_WAITLIST_ID", waitlistId);

            var outStatus = new SqlParameter("@OUT_STATUS", SqlDbType.Char, 1) { Direction = ParameterDirection.Output };
            var outMessage = new SqlParameter("@OUT_MESSAGE", SqlDbType.VarChar, 500) { Direction = ParameterDirection.Output };
            cmd.Parameters.Add(outStatus);
            cmd.Parameters.Add(outMessage);

            conn.Open();
            cmd.ExecuteNonQuery();

            return new WaitlistActionResult
            {
                waitlistId = waitlistId,
                status = Convert.ToString(outStatus.Value) ?? "",
                message = Convert.ToString(outMessage.Value) ?? ""
            };
        }

        // ==========================================
        // COMMON HELPERS
        // ==========================================

        private static string? GetString(SqlDataReader dr, string column) =>
            dr[column] == DBNull.Value ? null : Convert.ToString(dr[column]);

        private static int GetInt(SqlDataReader dr, string column) =>
            dr[column] == DBNull.Value ? 0 : Convert.ToInt32(dr[column]);

        private static int? GetNullableInt(SqlDataReader dr, string column) =>
            dr[column] == DBNull.Value ? null : Convert.ToInt32(dr[column]);

        private static DateTime? GetNullableDateTime(SqlDataReader dr, string column) =>
            dr[column] == DBNull.Value ? null : Convert.ToDateTime(dr[column]);
    }
}
