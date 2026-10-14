using Microsoft.Data.SqlClient;
using Suvarna.DGMS.Api.Models;
using System.Data;
using System.Text.Json;

namespace Suvarna.DGMS.Api.Repositories
{
    public class LaundryLogRepository
    {
        private readonly string _conn;

        public LaundryLogRepository(IConfiguration configuration)
        {
            _conn = configuration.GetConnectionString("SqlServer");
        }

        // ==========================================
        // GET LAUNDRY LOG
        // NEW: scope = ACTIVE (in-house + open entries of checked-out guests)
        //      or ALL (full history)
        // ==========================================

        public List<LaundryLogRow> GetEntries(int sessionId, string? status, string? type, string? scope)
        {
            List<LaundryLogRow> list = new();

            using SqlConnection conn = new SqlConnection(_conn);
            using SqlCommand cmd = new SqlCommand("DGMS.PR_DGMS_GET_LAUNDRY_LOG", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@IP_SESSION_ID", sessionId);
            cmd.Parameters.AddWithValue("@IP_STATUS", string.IsNullOrWhiteSpace(status) ? DBNull.Value : status);
            cmd.Parameters.AddWithValue("@IP_TYPE", string.IsNullOrWhiteSpace(type) ? DBNull.Value : type);
            cmd.Parameters.AddWithValue("@IP_SCOPE", string.Equals(scope, "ALL", StringComparison.OrdinalIgnoreCase) ? "ALL" : "ACTIVE");

            conn.Open();
            using SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                list.Add(new LaundryLogRow
                {
                    laundryLogId = GetInt(dr, "LAUNDRY_LOG_ID"),
                    slipNo = GetString(dr, "LAUNDRY_SLIP_NO") ?? "",
                    bookingId = GetInt(dr, "BOOKING_ID"),
                    bookingNo = GetString(dr, "BOOKING_NO") ?? "",
                    bookingStatus = GetString(dr, "BOOKING_STATUS") ?? "",          // NEW
                    checkoutDt = GetNullableDateTime(dr, "ACTUAL_CHECKOUT_DT"),      // NEW
                    roomNo = GetString(dr, "ROOM_NO") ?? "",
                    guestNm = GetString(dr, "GUEST_NM") ?? "",
                    laundryType = GetString(dr, "LAUNDRY_TYPE") ?? "",
                    itemCount = GetInt(dr, "ITEM_COUNT"),
                    itemDetailsJson = GetString(dr, "ITEM_DETAILS"),
                    pickupDt = GetNullableDateTime(dr, "PICKUP_DT"),
                    deliveryDt = GetNullableDateTime(dr, "DELIVERY_DT"),
                    chargePosted = GetString(dr, "CHARGE_POSTED") ?? "N",
                    chargeId = GetNullableInt(dr, "CHARGE_ID"),
                    remarks = GetString(dr, "REMARKS"),
                    createDt = Convert.ToDateTime(dr["CREATE_DT"]),
                    status = GetString(dr, "STATUS") ?? ""
                });
            }

            return list;
        }

        // ==========================================
        // GET ACTIVE BOOKINGS (for "Room Number" dropdown)
        // ==========================================

        public List<LaundryBookingLookup> GetActiveBookings(int sessionId)
        {
            List<LaundryBookingLookup> list = new();

            using SqlConnection conn = new SqlConnection(_conn);
            using SqlCommand cmd = new SqlCommand("DGMS.PR_DGMS_GET_BILLING_BOOKINGS", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@IP_SESSION_ID", sessionId);
            cmd.Parameters.AddWithValue("@IP_PROPERTY_ID", DBNull.Value);

            conn.Open();
            using SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                list.Add(new LaundryBookingLookup
                {
                    bookingId = GetInt(dr, "BOOKING_ID"),
                    bookingRef = GetString(dr, "BOOKING_REF") ?? "",
                    guestNm = GetString(dr, "GUEST_NM") ?? "",
                    roomId = GetInt(dr, "ROOM_ID"),
                    roomNo = GetString(dr, "ROOM_NO") ?? "",
                    roomName = GetString(dr, "ROOM_NAME") ?? "",
                    propertyId = GetInt(dr, "PROPERTY_ID")
                });
            }

            return list;
        }

        // ==========================================
        // CREATE LOG
        // ==========================================

        public LaundryActionResult CreateLog(CreateLaundryLogRequest request)
        {
            using SqlConnection conn = new SqlConnection(_conn);
            using SqlCommand cmd = new SqlCommand("DGMS.PR_DGMS_CREATE_LAUNDRY_LOG", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@IP_SESSION_ID", request.sessionId);
            cmd.Parameters.AddWithValue("@IP_BOOKING_ID", request.bookingId);
            cmd.Parameters.AddWithValue("@IP_ROOM_ID", request.roomId);
            cmd.Parameters.AddWithValue("@IP_LAUNDRY_TYPE", request.laundryType);
            cmd.Parameters.AddWithValue("@IP_ITEM_DETAILS_JSON", JsonSerializer.Serialize(request.items));
            cmd.Parameters.AddWithValue("@IP_REMARKS", string.IsNullOrWhiteSpace(request.remarks) ? DBNull.Value : request.remarks);

            var outId = new SqlParameter("@OUT_LAUNDRY_LOG_ID", SqlDbType.Int) { Direction = ParameterDirection.Output };
            var outSlip = new SqlParameter("@OUT_SLIP_NO", SqlDbType.VarChar, 30) { Direction = ParameterDirection.Output };
            var outStatus = new SqlParameter("@OUT_STATUS", SqlDbType.Char, 1) { Direction = ParameterDirection.Output };
            var outMessage = new SqlParameter("@OUT_MESSAGE", SqlDbType.VarChar, 500) { Direction = ParameterDirection.Output };
            cmd.Parameters.Add(outId);
            cmd.Parameters.Add(outSlip);
            cmd.Parameters.Add(outStatus);
            cmd.Parameters.Add(outMessage);

            conn.Open();
            cmd.ExecuteNonQuery();

            return new LaundryActionResult
            {
                laundryLogId = outId.Value == DBNull.Value ? 0 : Convert.ToInt32(outId.Value),
                slipNo = Convert.ToString(outSlip.Value) ?? "",
                status = Convert.ToString(outStatus.Value) ?? "",
                message = Convert.ToString(outMessage.Value) ?? ""
            };
        }

        // ==========================================
        // RETURN & COLLECT — guest came back after check-out (NEW)
        // ==========================================

        public LaundryReturnPreview? GetReturnPreview(int sessionId, int laundryLogId)
        {
            using SqlConnection conn = new SqlConnection(_conn);
            using SqlCommand cmd = new SqlCommand("DGMS.PR_DGMS_GET_LAUNDRY_RETURN_PREVIEW", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@IP_SESSION_ID", sessionId);
            cmd.Parameters.AddWithValue("@IP_LAUNDRY_LOG_ID", laundryLogId);

            conn.Open();
            using SqlDataReader dr = cmd.ExecuteReader();
            if (!dr.Read()) return null;

            return new LaundryReturnPreview
            {
                laundryLogId = GetInt(dr, "LAUNDRY_LOG_ID"),
                slipNo = GetString(dr, "LAUNDRY_SLIP_NO") ?? "",
                laundryType = GetString(dr, "LAUNDRY_TYPE") ?? "",
                itemCount = GetInt(dr, "ITEM_COUNT"),
                bookingNo = GetString(dr, "BOOKING_NO") ?? "",
                bookingStatus = GetString(dr, "BOOKING_STATUS") ?? "",
                checkoutDt = GetNullableDateTime(dr, "ACTUAL_CHECKOUT_DT"),
                roomNo = GetString(dr, "ROOM_NO") ?? "",
                guestNm = GetString(dr, "GUEST_NM") ?? "",
                billNo = GetString(dr, "BILL_NO") ?? "",
                priceConfigured = (GetString(dr, "PRICE_CONFIGURED") ?? "N") == "Y",
                serviceNm = GetString(dr, "SERVICE_NM") ?? "",
                unitRate = GetDecimal(dr, "UNIT_RATE"),
                gstPct = GetDecimal(dr, "GST_PCT"),
                baseAmt = GetDecimal(dr, "BASE_AMT"),
                cgstAmt = GetDecimal(dr, "CGST_AMT"),
                sgstAmt = GetDecimal(dr, "SGST_AMT"),
                grossAmt = GetDecimal(dr, "GROSS_AMT")
            };
        }

        public LaundryReturnResult ReturnWithPayment(LaundryReturnRequest request)
        {
            using SqlConnection conn = new SqlConnection(_conn);
            using SqlCommand cmd = new SqlCommand("DGMS.PR_DGMS_RETURN_LAUNDRY_WITH_PAYMENT", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@IP_SESSION_ID", request.sessionId);
            cmd.Parameters.AddWithValue("@IP_LAUNDRY_LOG_ID", request.laundryLogId);
            cmd.Parameters.AddWithValue("@IP_PAYMENT_MODE", request.paymentMode ?? "");
            cmd.Parameters.Add(new SqlParameter("@IP_PAYMENT_AMOUNT", SqlDbType.Decimal) { Precision = 18, Scale = 2, Value = request.paymentAmount });
            cmd.Parameters.AddWithValue("@IP_PAYMENT_REF", string.IsNullOrWhiteSpace(request.paymentReference) ? DBNull.Value : request.paymentReference.Trim());
            cmd.Parameters.AddWithValue("@IP_CARD_LAST4", string.IsNullOrWhiteSpace(request.cardLast4) ? DBNull.Value : request.cardLast4);
            cmd.Parameters.AddWithValue("@IP_UPI_VPA", string.IsNullOrWhiteSpace(request.upiVpa) ? DBNull.Value : request.upiVpa);
            cmd.Parameters.AddWithValue("@IP_PAYMENT_REMARKS", string.IsNullOrWhiteSpace(request.paymentRemarks) ? DBNull.Value : request.paymentRemarks);

            var outChg = new SqlParameter("@OUT_CHARGE_ID", SqlDbType.Int) { Direction = ParameterDirection.Output };
            var outPay = new SqlParameter("@OUT_PAYMENT_ID", SqlDbType.Int) { Direction = ParameterDirection.Output };
            var outGross = new SqlParameter("@OUT_GROSS_AMT", SqlDbType.Decimal) { Precision = 18, Scale = 2, Direction = ParameterDirection.Output };
            var outBillNo = new SqlParameter("@OUT_BILL_NO", SqlDbType.VarChar, 30) { Direction = ParameterDirection.Output };
            var outStatus = new SqlParameter("@OUT_STATUS", SqlDbType.Char, 1) { Direction = ParameterDirection.Output };
            var outMessage = new SqlParameter("@OUT_MESSAGE", SqlDbType.VarChar, 500) { Direction = ParameterDirection.Output };
            cmd.Parameters.Add(outChg);
            cmd.Parameters.Add(outPay);
            cmd.Parameters.Add(outGross);
            cmd.Parameters.Add(outBillNo);
            cmd.Parameters.Add(outStatus);
            cmd.Parameters.Add(outMessage);

            conn.Open();
            cmd.ExecuteNonQuery();

            return new LaundryReturnResult
            {
                laundryLogId = request.laundryLogId,
                chargeId = outChg.Value == DBNull.Value ? 0 : Convert.ToInt32(outChg.Value),
                paymentId = outPay.Value == DBNull.Value ? 0 : Convert.ToInt32(outPay.Value),
                grossAmt = outGross.Value == DBNull.Value ? 0 : Convert.ToDecimal(outGross.Value),
                billNo = Convert.ToString(outBillNo.Value) ?? "",
                status = Convert.ToString(outStatus.Value) ?? "",
                message = Convert.ToString(outMessage.Value) ?? ""
            };
        }

        // ==========================================
        // MARK PICKED UP / DELIVERED / CANCEL / REVERSE CHARGE
        // ==========================================

        public LaundryActionResult MarkPickedUp(int sessionId, int laundryLogId) =>
            ExecAction("DGMS.PR_DGMS_MARK_LAUNDRY_PICKED_UP", sessionId, laundryLogId, null, false);

        public LaundryActionResult MarkDelivered(int sessionId, int laundryLogId) =>
            ExecAction("DGMS.PR_DGMS_MARK_LAUNDRY_DELIVERED", sessionId, laundryLogId, null, false);

        public LaundryActionResult Cancel(int sessionId, int laundryLogId, string? reason) =>
            ExecAction("DGMS.PR_DGMS_CANCEL_LAUNDRY_LOG", sessionId, laundryLogId, reason, true);

        public LaundryActionResult ReverseCharge(int sessionId, int laundryLogId, string? reason) =>
            ExecAction("DGMS.PR_DGMS_REVERSE_LAUNDRY_CHARGE", sessionId, laundryLogId, reason, true);

        private LaundryActionResult ExecAction(string procName, int sessionId, int laundryLogId, string? reason, bool hasReason)
        {
            using SqlConnection conn = new SqlConnection(_conn);
            using SqlCommand cmd = new SqlCommand(procName, conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@IP_SESSION_ID", sessionId);
            cmd.Parameters.AddWithValue("@IP_LAUNDRY_LOG_ID", laundryLogId);

            if (hasReason)
            {
                cmd.Parameters.AddWithValue("@IP_REASON", string.IsNullOrWhiteSpace(reason) ? DBNull.Value : reason);
            }

            var outStatus = new SqlParameter("@OUT_STATUS", SqlDbType.Char, 1) { Direction = ParameterDirection.Output };
            var outMessage = new SqlParameter("@OUT_MESSAGE", SqlDbType.VarChar, 500) { Direction = ParameterDirection.Output };
            cmd.Parameters.Add(outStatus);
            cmd.Parameters.Add(outMessage);

            conn.Open();
            cmd.ExecuteNonQuery();

            return new LaundryActionResult
            {
                laundryLogId = laundryLogId,
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

        private static decimal GetDecimal(SqlDataReader dr, string column) =>
            dr[column] == DBNull.Value ? 0 : Convert.ToDecimal(dr[column]);

        private static DateTime? GetNullableDateTime(SqlDataReader dr, string column) =>
            dr[column] == DBNull.Value ? null : Convert.ToDateTime(dr[column]);
    }
}
