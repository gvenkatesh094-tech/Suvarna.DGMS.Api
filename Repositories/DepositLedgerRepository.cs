using Microsoft.Data.SqlClient;
using Suvarna.DGMS.Api.Models;
using System.Data;

namespace Suvarna.DGMS.Api.Repositories
{
    public class DepositLedgerRepository
    {
        private readonly string _conn;

        public DepositLedgerRepository(IConfiguration configuration)
        {
            _conn = configuration.GetConnectionString("SqlServer");
        }

        // ==========================================
        // GET DEPOSIT LEDGER
        // ==========================================

        public List<DepositLedgerBooking> GetDepositLedger(int sessionId, int? propertyId, string? statusFilter)
        {
            List<DepositLedgerBooking> list = new();

            try
            {
                using SqlConnection conn = new SqlConnection(_conn);
                using SqlCommand cmd = new SqlCommand("DGMS.PR_DGMS_GET_DEPOSIT_LEDGER", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@IP_SESSION_ID", sessionId);
                AddParameter(cmd, "@IP_PROPERTY_ID", propertyId);
                AddParameter(cmd, "@IP_STATUS_FILTER", string.IsNullOrWhiteSpace(statusFilter) ? null : statusFilter.Trim().ToUpperInvariant());

                conn.Open();

                using SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    list.Add(new DepositLedgerBooking
                    {
                        bookingId = GetInt(dr, "BOOKING_ID"),
                        bookingRef = GetString(dr, "BOOKING_REF") ?? "",
                        guestName = GetString(dr, "GUEST_NM") ?? "",
                        mobileNo = GetString(dr, "MOBILE_NO") ?? "",

                        roomNo = GetString(dr, "ROOM_NO") ?? "",
                        propertyId = GetInt(dr, "PROPERTY_ID"),
                        propertyName = GetString(dr, "PROPERTY_NM") ?? "",

                        bookingStatus = GetString(dr, "BOOKING_STATUS") ?? "",
                        checkinDt = GetNullableDateTime(dr, "CHECKIN_DT"),
                        expectedCheckoutDt = GetNullableDateTime(dr, "EXPECTED_CHECKOUT_DT"),

                        depositHdrId = GetInt(dr, "DEPOSIT_HDR_ID"),
                        depositNo = GetString(dr, "DEPOSIT_NO") ?? "",

                        depositAmount = GetDecimal(dr, "DEPOSIT_AMT"),
                        adjustedAmount = GetDecimal(dr, "ADJUSTED_AMT"),
                        refundedAmount = GetDecimal(dr, "REFUNDED_AMT"),
                        forfeitedAmount = GetDecimal(dr, "FORFEITED_AMT"),
                        balanceAmount = GetDecimal(dr, "BALANCE_AMT"),
                        depositStatus = GetString(dr, "DEPOSIT_STATUS") ?? "",

                        chargedAmount = GetDecimal(dr, "CHARGED_AMT"),

                        // NEW — estimated dues & refundable amount
                        duesAsOfDt = GetNullableDateTime(dr, "DUES_AS_OF_DT"),
                        estStayDays = GetInt(dr, "EST_STAY_DAYS"),
                        dailyRate = GetDecimal(dr, "DAILY_RATE"),
                        estRoomAmount = GetDecimal(dr, "EST_ROOM_AMT"),
                        estRoomGst = GetDecimal(dr, "EST_ROOM_GST"),
                        estServiceAmount = GetDecimal(dr, "EST_SERVICE_AMT"),
                        estServiceGst = GetDecimal(dr, "EST_SERVICE_GST"),
                        paidAmount = GetDecimal(dr, "PAID_AMT"),
                        estNetDues = GetDecimal(dr, "EST_NET_DUES"),
                        refundableAmount = GetDecimal(dr, "REFUNDABLE_AMT")
                    });
                }

                return list;
            }
            catch (Exception ex)
            {
                // FIX: keep the real reason — the generic text hid SQL errors
                throw new Exception("Error while fetching deposit ledger: " + ex.Message, ex);
            }
        }

        // ==========================================
        // GET DEPOSIT TRANSACTION HISTORY
        // ==========================================

        public List<DepositTxnHistory> GetDepositTxnHistory(int sessionId, int bookingId)
        {
            List<DepositTxnHistory> list = new();

            try
            {
                using SqlConnection conn = new SqlConnection(_conn);
                using SqlCommand cmd = new SqlCommand("DGMS.PR_DGMS_GET_DEPOSIT_TXN_HISTORY", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@IP_SESSION_ID", sessionId);
                cmd.Parameters.AddWithValue("@IP_BOOKING_ID", bookingId);

                conn.Open();

                using SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    list.Add(new DepositTxnHistory
                    {
                        txnId = GetInt(dr, "TXN_ID"),
                        txnType = GetString(dr, "TXN_TYPE") ?? "",
                        txnDt = GetNullableDateTime(dr, "TXN_DT") ?? DateTime.MinValue,
                        amount = GetDecimal(dr, "AMOUNT"),
                        paymentMode = GetString(dr, "PAYMENT_MODE"),
                        paymentRef = GetString(dr, "PAYMENT_REF"),
                        refundMode = GetString(dr, "REFUND_MODE"),
                        refundRef = GetString(dr, "REFUND_REF"),
                        remarks = GetString(dr, "REMARKS") ?? "",
                        createBy = GetInt(dr, "CREATE_BY"),
                        createDt = GetNullableDateTime(dr, "CREATE_DT") ?? DateTime.MinValue
                    });
                }

                return list;
            }
            catch (Exception ex)
            {
                throw new Exception("Error while fetching deposit transaction history: " + ex.Message, ex);
            }
        }

        // ==========================================
        // REFUND DEPOSIT
        // ==========================================

        public DepositRefundResult RefundDeposit(DepositRefundRequest request)
        {
            try
            {
                using SqlConnection conn = new SqlConnection(_conn);
                using SqlCommand cmd = new SqlCommand("DGMS.PR_DGMS_REFUND_DEPOSIT", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                AddParameter(cmd, "@IP_SESSION_ID", request.sessionId);
                AddParameter(cmd, "@IP_BOOKING_ID", request.bookingId);
                cmd.Parameters.Add(new SqlParameter("@IP_REFUND_AMOUNT", SqlDbType.Decimal) { Precision = 18, Scale = 2, Value = request.refundAmount });
                AddParameter(cmd, "@IP_REFUND_MODE", request.refundMode?.Trim().ToUpperInvariant());
                AddParameter(cmd, "@IP_REFUND_REF", string.IsNullOrWhiteSpace(request.refundRef) ? null : request.refundRef.Trim());
                AddParameter(cmd, "@IP_REMARKS", string.IsNullOrWhiteSpace(request.remarks) ? null : request.remarks.Trim());

                SqlParameter depositTxnId = new SqlParameter("@OUT_DEPOSIT_TXN_ID", SqlDbType.Int) { Direction = ParameterDirection.Output };
                SqlParameter balanceAmt = new SqlParameter("@OUT_BALANCE_AMT", SqlDbType.Decimal) { Direction = ParameterDirection.Output, Precision = 18, Scale = 2 };
                SqlParameter status = new SqlParameter("@OUT_STATUS", SqlDbType.Char, 1) { Direction = ParameterDirection.Output };
                SqlParameter message = new SqlParameter("@OUT_MESSAGE", SqlDbType.VarChar, 500) { Direction = ParameterDirection.Output };

                cmd.Parameters.Add(depositTxnId);
                cmd.Parameters.Add(balanceAmt);
                cmd.Parameters.Add(status);
                cmd.Parameters.Add(message);

                conn.Open();
                cmd.ExecuteNonQuery();

                return new DepositRefundResult
                {
                    depositTxnId = depositTxnId.Value == DBNull.Value ? 0 : Convert.ToInt32(depositTxnId.Value),
                    balanceAmount = balanceAmt.Value == DBNull.Value ? 0 : Convert.ToDecimal(balanceAmt.Value),
                    status = Convert.ToString(status.Value) ?? "",
                    message = Convert.ToString(message.Value) ?? ""
                };
            }
            catch (Exception ex)
            {
                throw new Exception("Error while processing deposit refund: " + ex.Message, ex);
            }
        }

        // ==========================================
        // COMMON HELPERS
        // ==========================================

        private static void AddParameter(SqlCommand cmd, string name, object? value) =>
            cmd.Parameters.AddWithValue(name, value ?? DBNull.Value);

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
