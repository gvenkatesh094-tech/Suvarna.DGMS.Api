using Microsoft.Data.SqlClient;
using Suvarna.DGMS.Api.Models;
using System.Data;

namespace Suvarna.DGMS.Api.Repositories
{
    public class GuestFolioBillingRepository
    {
        private readonly string _conn;

        public GuestFolioBillingRepository(IConfiguration configuration)
        {
            _conn = configuration.GetConnectionString("SqlServer");
        }

        // ==========================================
        // GET BOOKINGS (Select Guest dropdown)
        // ==========================================

        public List<BillingBookingLookup> GetBookings(int sessionId, int? propertyId)
        {
            List<BillingBookingLookup> list = new();

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
                    list.Add(new BillingBookingLookup
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
                throw new Exception("Error while fetching billing guest list.", ex);
            }
        }

        // ==========================================
        // GET SERVICES (Add Charge dropdown)
        // ==========================================

        public List<BillingServiceLookup> GetServices(int sessionId, int? propertyId)
        {
            List<BillingServiceLookup> list = new();

            try
            {
                using SqlConnection conn = new SqlConnection(_conn);
                using SqlCommand cmd = new SqlCommand("DGMS.PR_DGMS_GET_BILLING_SERVICES", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IP_SESSION_ID", sessionId);
                cmd.Parameters.AddWithValue("@IP_PROPERTY_ID", (object?)propertyId ?? DBNull.Value);
                conn.Open();

                using SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    list.Add(new BillingServiceLookup
                    {
                        serviceId = GetInt(dr, "SERVICE_ID"),
                        serviceNm = GetString(dr, "SERVICE_NM") ?? "",
                        rate = GetDecimal(dr, "RATE"),
                        gstPct = GetDecimal(dr, "GST_PCT"),
                        sacCode = GetString(dr, "SAC_CODE") ?? ""
                    });
                }

                return list;
            }
            catch (Exception ex)
            {
                throw new Exception("Error while fetching billing services.", ex);
            }
        }

        // ==========================================
        // GET FOLIO (header + charge lines)
        // ==========================================

        public BillingFolio? GetFolio(int sessionId, int bookingId)
        {
            try
            {
                using SqlConnection conn = new SqlConnection(_conn);
                using SqlCommand cmd = new SqlCommand("DGMS.PR_DGMS_GET_BILLING_FOLIO", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IP_SESSION_ID", sessionId);
                cmd.Parameters.AddWithValue("@IP_BOOKING_ID", bookingId);
                conn.Open();

                using SqlDataReader dr = cmd.ExecuteReader();

                if (!dr.Read())
                    return null;

                var folio = new BillingFolio
                {
                    bookingId = GetInt(dr, "BOOKING_ID"),
                    bookingRef = GetString(dr, "BOOKING_REF") ?? "",
                    guestNm = GetString(dr, "GUEST_NM") ?? "",
                    roomNo = GetString(dr, "ROOM_NO") ?? "",
                    checkinDt = GetNullableDateTime(dr, "CHECKIN_DT"),
                    checkoutDt = GetNullableDateTime(dr, "CHECKOUT_DT"),
                    categoryNm = GetString(dr, "CATEGORY_NM") ?? "",
                    nights = GetInt(dr, "NIGHTS"),
                    mobileNo = GetString(dr, "MOBILE_NO") ?? "",

                    billId = GetInt(dr, "BILL_ID"),
                    billNo = GetString(dr, "BILL_NO") ?? "",
                    totalAccmAmt = GetDecimal(dr, "TOTAL_ACCM_AMT"),
                    totalSvcAmt = GetDecimal(dr, "TOTAL_SVC_AMT"),
                    totalBaseAmt = GetDecimal(dr, "TOTAL_BASE_AMT"),
                    totalCgstAmt = GetDecimal(dr, "TOTAL_CGST_AMT"),
                    totalSgstAmt = GetDecimal(dr, "TOTAL_SGST_AMT"),
                    totalGstAmt = GetDecimal(dr, "TOTAL_GST_AMT"),
                    totalGrossAmt = GetDecimal(dr, "TOTAL_GROSS_AMT"),
                    netPayableAmt = GetDecimal(dr, "NET_PAYABLE_AMT"),
                    depositAdjustedAmt = GetDecimal(dr, "DEPOSIT_ADJUSTED_AMT"),
                    totalPaidAmt = GetDecimal(dr, "TOTAL_PAID_AMT"),
                    balanceAmt = GetDecimal(dr, "BALANCE_AMT"),
                    billStatus = GetString(dr, "BILL_STATUS") ?? "",

                    depositAmt = GetDecimal(dr, "DEPOSIT_AMT"),
                    depositBalance = GetDecimal(dr, "DEPOSIT_BALANCE"),
                    depositStatus = GetString(dr, "DEPOSIT_STATUS") ?? "",

                    umrNo = GetString(dr, "UMR_NO"),
                    admnNo = GetString(dr, "ADMN_NO"),
                    patientId = GetNullableInt(dr, "PATIENT_ID"),
                    admnId = GetNullableInt(dr, "ADMN_ID"),
                    wardNm = GetString(dr, "WARD_NM"),
                    transferStatus = GetString(dr, "TRANSFER_STATUS")
                };

                // Result Set 2 : charge lines
                if (dr.NextResult())
                {
                    while (dr.Read())
                    {
                        folio.chargeLines.Add(new BillingChargeLine
                        {
                            chargeId = GetInt(dr, "CHARGE_ID"),
                            chargeDt = GetNullableDateTime(dr, "CHARGE_DT") ?? DateTime.MinValue,
                            description = GetString(dr, "DESCRIPTION") ?? "",
                            sacCode = GetString(dr, "SAC_CODE") ?? "",
                            baseAmt = GetDecimal(dr, "BASE_AMT"),
                            gstPct = GetDecimal(dr, "GST_PCT"),
                            totalAmt = GetDecimal(dr, "TOTAL_AMT"),
                            status = GetString(dr, "STATUS") ?? ""
                        });
                    }
                }

                return folio;
            }
            catch (Exception ex)
            {
                throw new Exception("Error while fetching guest folio.", ex);
            }
        }

        // ==========================================
        // ADD CHARGE
        // ==========================================

        public ChargeResult AddCharge(AddChargeRequest request)
        {
            try
            {
                using SqlConnection conn = new SqlConnection(_conn);
                using SqlCommand cmd = new SqlCommand("DGMS.PR_DGMS_POST_BILLING_CHARGE", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@IP_SESSION_ID", request.sessionId);
                cmd.Parameters.AddWithValue("@IP_BOOKING_ID", request.bookingId);
                cmd.Parameters.AddWithValue("@IP_SERVICE_ID", request.serviceId);
                cmd.Parameters.AddWithValue("@IP_QTY", request.qty);

                var outChargeId = new SqlParameter("@OUT_CHARGE_ID", SqlDbType.Int) { Direction = ParameterDirection.Output };
                var outStatus = new SqlParameter("@OUT_STATUS", SqlDbType.Char, 1) { Direction = ParameterDirection.Output };
                var outMessage = new SqlParameter("@OUT_MESSAGE", SqlDbType.VarChar, 500) { Direction = ParameterDirection.Output };
                cmd.Parameters.Add(outChargeId);
                cmd.Parameters.Add(outStatus);
                cmd.Parameters.Add(outMessage);

                conn.Open();
                cmd.ExecuteNonQuery();

                return new ChargeResult
                {
                    chargeId = outChargeId.Value == DBNull.Value ? 0 : Convert.ToInt32(outChargeId.Value),
                    status = Convert.ToString(outStatus.Value) ?? "",
                    message = Convert.ToString(outMessage.Value) ?? ""
                };
            }
            catch (Exception ex)
            {
                throw new Exception("Error while posting charge.", ex);
            }
        }

        // ==========================================
        // UPDATE CHARGE
        // ==========================================

        public ChargeResult UpdateCharge(UpdateChargeRequest request)
        {
            try
            {
                using SqlConnection conn = new SqlConnection(_conn);
                using SqlCommand cmd = new SqlCommand("DGMS.PR_DGMS_UPDATE_BILLING_CHARGE", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@IP_SESSION_ID", request.sessionId);
                cmd.Parameters.AddWithValue("@IP_CHARGE_ID", request.chargeId);
                cmd.Parameters.AddWithValue("@IP_CHARGE_DT", request.chargeDt);
                cmd.Parameters.AddWithValue("@IP_DESCRIPTION", request.description ?? "");
                cmd.Parameters.AddWithValue("@IP_SAC_CODE", request.sacCode ?? "");
                cmd.Parameters.AddWithValue("@IP_BASE_AMT", request.baseAmt);
                cmd.Parameters.AddWithValue("@IP_GST_PCT", request.gstPct);
                cmd.Parameters.AddWithValue("@IP_STATUS", request.status ?? "POSTED");

                var outStatus = new SqlParameter("@OUT_STATUS", SqlDbType.Char, 1) { Direction = ParameterDirection.Output };
                var outMessage = new SqlParameter("@OUT_MESSAGE", SqlDbType.VarChar, 500) { Direction = ParameterDirection.Output };
                cmd.Parameters.Add(outStatus);
                cmd.Parameters.Add(outMessage);

                conn.Open();
                cmd.ExecuteNonQuery();

                return new ChargeResult
                {
                    chargeId = request.chargeId,
                    status = Convert.ToString(outStatus.Value) ?? "",
                    message = Convert.ToString(outMessage.Value) ?? ""
                };
            }
            catch (Exception ex)
            {
                throw new Exception("Error while updating charge.", ex);
            }
        }

        // ==========================================
        // COLLECT PAYMENT
        // ==========================================

        public PaymentResult CollectPayment(CollectPaymentRequest request)
        {
            try
            {
                using SqlConnection conn = new SqlConnection(_conn);
                using SqlCommand cmd = new SqlCommand("DGMS.PR_DGMS_COLLECT_BILLING_PAYMENT", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@IP_SESSION_ID", request.sessionId);
                cmd.Parameters.AddWithValue("@IP_BOOKING_ID", request.bookingId);
                cmd.Parameters.AddWithValue("@IP_PAYMENT_MODE", request.paymentMode);
                cmd.Parameters.AddWithValue("@IP_AMOUNT", request.amount);
                cmd.Parameters.AddWithValue("@IP_PAYMENT_REF", (object?)request.paymentRef ?? DBNull.Value);

                var outPaymentId = new SqlParameter("@OUT_PAYMENT_ID", SqlDbType.Int) { Direction = ParameterDirection.Output };
                var outStatus = new SqlParameter("@OUT_STATUS", SqlDbType.Char, 1) { Direction = ParameterDirection.Output };
                var outMessage = new SqlParameter("@OUT_MESSAGE", SqlDbType.VarChar, 500) { Direction = ParameterDirection.Output };
                cmd.Parameters.Add(outPaymentId);
                cmd.Parameters.Add(outStatus);
                cmd.Parameters.Add(outMessage);

                conn.Open();
                cmd.ExecuteNonQuery();

                return new PaymentResult
                {
                    paymentId = outPaymentId.Value == DBNull.Value ? 0 : Convert.ToInt32(outPaymentId.Value),
                    status = Convert.ToString(outStatus.Value) ?? "",
                    message = Convert.ToString(outMessage.Value) ?? ""
                };
            }
            catch (Exception ex)
            {
                throw new Exception("Error while collecting payment.", ex);
            }
        }

        // ==========================================
        // TRANSFER TO IP
        // ==========================================

        public TransferResult TransferToIp(TransferToIpRequest request)
        {
            try
            {
                using SqlConnection conn = new SqlConnection(_conn);
                using SqlCommand cmd = new SqlCommand("DGMS.PR_DGMS_TRANSFER_CHARGES_TO_IP", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@IP_SESSION_ID", request.sessionId);
                cmd.Parameters.AddWithValue("@IP_BOOKING_ID", request.bookingId);

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
                throw new Exception("Error while transferring charges to IP.", ex);
            }
        }

        // ==========================================
        // GENERATE INVOICE
        // ==========================================

        public InvoiceResult GenerateInvoice(GenerateInvoiceRequest request)
        {
            try
            {
                using SqlConnection conn = new SqlConnection(_conn);
                using SqlCommand cmd = new SqlCommand("DGMS.PR_DGMS_GENERATE_GST_INVOICE", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@IP_SESSION_ID", request.sessionId);
                cmd.Parameters.AddWithValue("@IP_BOOKING_ID", request.bookingId);

                var outInvoiceId = new SqlParameter("@OUT_INVOICE_ID", SqlDbType.Int) { Direction = ParameterDirection.Output };
                var outInvoiceNo = new SqlParameter("@OUT_INVOICE_NO", SqlDbType.VarChar, 30) { Direction = ParameterDirection.Output };
                var outStatus = new SqlParameter("@OUT_STATUS", SqlDbType.Char, 1) { Direction = ParameterDirection.Output };
                var outMessage = new SqlParameter("@OUT_MESSAGE", SqlDbType.VarChar, 500) { Direction = ParameterDirection.Output };
                cmd.Parameters.Add(outInvoiceId);
                cmd.Parameters.Add(outInvoiceNo);
                cmd.Parameters.Add(outStatus);
                cmd.Parameters.Add(outMessage);

                conn.Open();
                cmd.ExecuteNonQuery();

                return new InvoiceResult
                {
                    invoiceId = outInvoiceId.Value == DBNull.Value ? 0 : Convert.ToInt32(outInvoiceId.Value),
                    invoiceNo = Convert.ToString(outInvoiceNo.Value) ?? "",
                    status = Convert.ToString(outStatus.Value) ?? "",
                    message = Convert.ToString(outMessage.Value) ?? ""
                };
            }
            catch (Exception ex)
            {
                throw new Exception("Error while generating GST invoice.", ex);
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

        private static int? GetNullableInt(SqlDataReader dr, string column)
        {
            return dr[column] == DBNull.Value ? null : Convert.ToInt32(dr[column]);
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
