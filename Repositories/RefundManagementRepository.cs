using Microsoft.Data.SqlClient;
using Suvarna.DGMS.Api.Models;
using System.Data;

namespace Suvarna.DGMS.Api.Repositories
{
    public class RefundManagementRepository
    {
        private readonly string _conn;

        public RefundManagementRepository(IConfiguration configuration)
        {
            _conn = configuration.GetConnectionString("SqlServer");
        }

        // ==========================================
        // GET ELIGIBLE BOOKINGS
        // ==========================================

        public List<RefundEligibleBooking> GetEligibleBookings(int sessionId)
        {
            List<RefundEligibleBooking> list = new();

            using SqlConnection conn = new SqlConnection(_conn);
            using SqlCommand cmd = new SqlCommand("DGMS.PR_DGMS_GET_REFUND_ELIGIBLE_BOOKINGS", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@IP_SESSION_ID", sessionId);

            conn.Open();
            using SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                list.Add(new RefundEligibleBooking
                {
                    bookingId = GetInt(dr, "BOOKING_ID"),
                    bookingRef = GetString(dr, "BOOKING_REF") ?? "",
                    guestNm = GetString(dr, "GUEST_NM") ?? "",
                    roomNo = GetString(dr, "ROOM_NO") ?? "",
                    depositHdrId = GetInt(dr, "DEPOSIT_HDR_ID"),
                    depositHeld = GetDecimal(dr, "DEPOSIT_HELD"),
                    chargedAmt = GetDecimal(dr, "CHARGED_AMT")
                });
            }

            return list;
        }

        // ==========================================
        // SUBMIT REFUND REQUEST
        // ==========================================

        public SubmitRefundResult SubmitRequest(SubmitRefundRequest request)
        {
            using SqlConnection conn = new SqlConnection(_conn);
            using SqlCommand cmd = new SqlCommand("DGMS.PR_DGMS_SUBMIT_REFUND_REQUEST", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@IP_SESSION_ID", request.sessionId);
            cmd.Parameters.AddWithValue("@IP_BOOKING_ID", request.bookingId);
            cmd.Parameters.AddWithValue("@IP_AMOUNT", request.amount);
            cmd.Parameters.AddWithValue("@IP_MODE", request.mode);
            cmd.Parameters.AddWithValue("@IP_ACCOUNT_REF", string.IsNullOrWhiteSpace(request.accountRef) ? DBNull.Value : request.accountRef);
            cmd.Parameters.AddWithValue("@IP_REASON_CD", request.reasonCd);
            cmd.Parameters.AddWithValue("@IP_REMARKS", string.IsNullOrWhiteSpace(request.remarks) ? DBNull.Value : request.remarks);

            var outId = new SqlParameter("@OUT_REQUEST_ID", SqlDbType.Int) { Direction = ParameterDirection.Output };
            var outNo = new SqlParameter("@OUT_REQUEST_NO", SqlDbType.VarChar, 30) { Direction = ParameterDirection.Output };
            var outRequires = new SqlParameter("@OUT_REQUIRES_APPROVAL", SqlDbType.Char, 1) { Direction = ParameterDirection.Output };
            var outStatus = new SqlParameter("@OUT_STATUS", SqlDbType.Char, 1) { Direction = ParameterDirection.Output };
            var outMessage = new SqlParameter("@OUT_MESSAGE", SqlDbType.VarChar, 500) { Direction = ParameterDirection.Output };
            cmd.Parameters.Add(outId);
            cmd.Parameters.Add(outNo);
            cmd.Parameters.Add(outRequires);
            cmd.Parameters.Add(outStatus);
            cmd.Parameters.Add(outMessage);

            conn.Open();
            cmd.ExecuteNonQuery();

            return new SubmitRefundResult
            {
                requestId = outId.Value == DBNull.Value ? 0 : Convert.ToInt32(outId.Value),
                requestNo = Convert.ToString(outNo.Value) ?? "",
                requiresApproval = Convert.ToString(outRequires.Value) == "Y",
                status = Convert.ToString(outStatus.Value) ?? "",
                message = Convert.ToString(outMessage.Value) ?? ""
            };
        }

        // ==========================================
        // GET REFUND REQUESTS (history/approval grid)
        // ==========================================

        public List<RefundRequestRow> GetRequests(int sessionId, string? status)
        {
            List<RefundRequestRow> list = new();

            using SqlConnection conn = new SqlConnection(_conn);
            using SqlCommand cmd = new SqlCommand("DGMS.PR_DGMS_GET_REFUND_REQUESTS", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@IP_SESSION_ID", sessionId);
            cmd.Parameters.AddWithValue("@IP_STATUS", (object?)status ?? DBNull.Value);

            conn.Open();
            using SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                list.Add(new RefundRequestRow
                {
                    requestId = GetInt(dr, "REQUEST_ID"),
                    requestNo = GetString(dr, "REQUEST_NO") ?? "",
                    bookingId = GetInt(dr, "BOOKING_ID"),
                    bookingNo = GetString(dr, "BOOKING_NO") ?? "",
                    guestNm = GetString(dr, "GUEST_NM") ?? "",
                    roomNo = GetString(dr, "ROOM_NO") ?? "",
                    refundAmount = GetDecimal(dr, "REFUND_AMOUNT"),
                    refundMode = GetString(dr, "REFUND_MODE") ?? "",
                    accountRef = GetString(dr, "ACCOUNT_REF"),
                    reasonCd = GetString(dr, "REASON_CD") ?? "",
                    remarks = GetString(dr, "REMARKS"),
                    requiresApproval = GetString(dr, "REQUIRES_APPROVAL") == "Y",
                    status = GetString(dr, "STATUS") ?? "",
                    approvedByName = GetString(dr, "APPROVED_BY_NAME"),
                    approvedDt = GetNullableDateTime(dr, "APPROVED_DT"),
                    approvalRemarks = GetString(dr, "APPROVAL_REMARKS"),
                    processedByName = GetString(dr, "PROCESSED_BY_NAME"),
                    processedDt = GetNullableDateTime(dr, "PROCESSED_DT"),
                    createDt = Convert.ToDateTime(dr["CREATE_DT"])
                });
            }

            return list;
        }

        // ==========================================
        // APPROVE / REJECT
        // ==========================================

        public RefundActionResult Decide(DecideRefundRequest request)
        {
            using SqlConnection conn = new SqlConnection(_conn);
            using SqlCommand cmd = new SqlCommand("DGMS.PR_DGMS_DECIDE_REFUND_REQUEST", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@IP_SESSION_ID", request.sessionId);
            cmd.Parameters.AddWithValue("@IP_REQUEST_ID", request.requestId);
            cmd.Parameters.AddWithValue("@IP_DECISION", request.decision);
            cmd.Parameters.AddWithValue("@IP_REMARKS", string.IsNullOrWhiteSpace(request.remarks) ? DBNull.Value : request.remarks);

            var outStatus = new SqlParameter("@OUT_STATUS", SqlDbType.Char, 1) { Direction = ParameterDirection.Output };
            var outMessage = new SqlParameter("@OUT_MESSAGE", SqlDbType.VarChar, 500) { Direction = ParameterDirection.Output };
            cmd.Parameters.Add(outStatus);
            cmd.Parameters.Add(outMessage);

            conn.Open();
            cmd.ExecuteNonQuery();

            return new RefundActionResult
            {
                status = Convert.ToString(outStatus.Value) ?? "",
                message = Convert.ToString(outMessage.Value) ?? ""
            };
        }

        // ==========================================
        // COMPLETE
        // ==========================================

        public RefundActionResult Complete(int sessionId, int requestId)
        {
            using SqlConnection conn = new SqlConnection(_conn);
            using SqlCommand cmd = new SqlCommand("DGMS.PR_DGMS_COMPLETE_REFUND_REQUEST", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@IP_SESSION_ID", sessionId);
            cmd.Parameters.AddWithValue("@IP_REQUEST_ID", requestId);

            var outStatus = new SqlParameter("@OUT_STATUS", SqlDbType.Char, 1) { Direction = ParameterDirection.Output };
            var outMessage = new SqlParameter("@OUT_MESSAGE", SqlDbType.VarChar, 500) { Direction = ParameterDirection.Output };
            cmd.Parameters.Add(outStatus);
            cmd.Parameters.Add(outMessage);

            conn.Open();
            cmd.ExecuteNonQuery();

            return new RefundActionResult
            {
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

        private static decimal GetDecimal(SqlDataReader dr, string column) =>
            dr[column] == DBNull.Value ? 0 : Convert.ToDecimal(dr[column]);

        private static DateTime? GetNullableDateTime(SqlDataReader dr, string column) =>
            dr[column] == DBNull.Value ? null : Convert.ToDateTime(dr[column]);
    }
}
