using Microsoft.Data.SqlClient;
using Suvarna.DGMS.Api.Models;
using System.Data;

namespace Suvarna.DGMS.Api.Repositories
{
    public class CorporateInvoicesRepository
    {
        private readonly string _conn;

        public CorporateInvoicesRepository(IConfiguration configuration)
        {
            _conn = configuration.GetConnectionString("SqlServer");
        }

        // ==========================================
        // MAIN TABLE
        // ==========================================

        public List<CorpInvoiceRow> GetInvoices(int sessionId)
        {
            List<CorpInvoiceRow> list = new();

            try
            {
                using SqlConnection conn = new SqlConnection(_conn);
                using SqlCommand cmd = new SqlCommand("DGMS.PR_DGMS_GET_CORP_INVOICES", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IP_SESSION_ID", sessionId);
                conn.Open();

                using SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    list.Add(new CorpInvoiceRow
                    {
                        invoiceId = GetInt(dr, "INVOICE_ID"),
                        invoiceNo = GetString(dr, "INVOICE_NO") ?? "",
                        corpNm = GetString(dr, "CORP_NM") ?? "",
                        period = GetString(dr, "PERIOD") ?? "",
                        totalBookings = GetInt(dr, "TOTAL_BOOKINGS"),
                        totalBaseAmt = GetDecimal(dr, "TOTAL_BASE_AMT"),
                        totalGstAmt = GetDecimal(dr, "TOTAL_GST_AMT"),
                        netPayableAmt = GetDecimal(dr, "NET_PAYABLE_AMT"),
                        dueDt = GetNullableDateTime(dr, "DUE_DT"),
                        status = GetString(dr, "STATUS") ?? "",
                        // NEW (Round B) - read only when 07 is run
                        paymentDt = Has(dr, "PAYMENT_DT") ? GetNullableDateTime(dr, "PAYMENT_DT") : null,
                        paymentRef = Has(dr, "PAYMENT_REF") ? GetString(dr, "PAYMENT_REF") : null,
                        isOverdue = Has(dr, "IS_OVERDUE") ? (GetString(dr, "IS_OVERDUE") ?? "N") : "N",
                        agreementId = Has(dr, "AGREEMENT_ID") ? GetInt(dr, "AGREEMENT_ID") : 0
                    });
                }

                return list;
            }
            catch (Exception ex)
            {
                throw new Exception("Error while fetching corporate invoices.", ex);
            }
        }

        // ==========================================
        // "Generate Invoice" panel — Corporate dropdown
        // ==========================================

        public List<CorpAgreementLookup> GetAgreements(int sessionId, bool includeExpired = false)
        {
            List<CorpAgreementLookup> list = new();

            try
            {
                using SqlConnection conn = new SqlConnection(_conn);
                using SqlCommand cmd = new SqlCommand("DGMS.PR_DGMS_GET_CORP_AGREEMENTS_LOOKUP", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IP_SESSION_ID", sessionId);
                cmd.Parameters.AddWithValue("@IP_INCLUDE_EXPIRED", includeExpired ? "Y" : "N");   // NEW (Round B)
                Suvarna.DGMS.Api.Helpers.ProcParameterFilter.RemoveUnknown(cmd);
                conn.Open();

                using SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    list.Add(new CorpAgreementLookup
                    {
                        agreementId = GetInt(dr, "AGREEMENT_ID"),
                        corpNm = GetString(dr, "CORP_NM") ?? "",
                        agreementNo = GetString(dr, "AGREEMENT_NO") ?? "",
                        creditDays = GetInt(dr, "CREDIT_DAYS"),
                        discountPct = GetDecimal(dr, "DISCOUNT_PCT")
                    });
                }

                return list;
            }
            catch (Exception ex)
            {
                throw new Exception("Error while fetching corporate agreements.", ex);
            }
        }

        // ==========================================
        // GENERATE
        // ==========================================

        public CorpInvoiceGenerateResult GenerateInvoice(GenerateCorpInvoiceRequest request)
        {
            try
            {
                using SqlConnection conn = new SqlConnection(_conn);
                using SqlCommand cmd = new SqlCommand("DGMS.PR_DGMS_GENERATE_CORP_INVOICE", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@IP_SESSION_ID", request.sessionId);
                cmd.Parameters.AddWithValue("@IP_AGREEMENT_ID", request.agreementId);
                cmd.Parameters.AddWithValue("@IP_PERIOD_FROM", request.periodFrom);
                cmd.Parameters.AddWithValue("@IP_PERIOD_TO", request.periodTo);

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

                return new CorpInvoiceGenerateResult
                {
                    invoiceId = outInvoiceId.Value == DBNull.Value ? 0 : Convert.ToInt32(outInvoiceId.Value),
                    invoiceNo = Convert.ToString(outInvoiceNo.Value) ?? "",
                    status = Convert.ToString(outStatus.Value) ?? "",
                    message = Convert.ToString(outMessage.Value) ?? ""
                };
            }
            catch (Exception ex)
            {
                throw new Exception("Error while generating corporate invoice.", ex);
            }
        }

        // ==========================================
        // DETAIL (View button)
        // ==========================================

        public CorpInvoiceDetail? GetInvoiceDetail(int sessionId, int invoiceId)
        {
            try
            {
                using SqlConnection conn = new SqlConnection(_conn);
                using SqlCommand cmd = new SqlCommand("DGMS.PR_DGMS_GET_CORP_INVOICE_DETAIL", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IP_SESSION_ID", sessionId);
                cmd.Parameters.AddWithValue("@IP_INVOICE_ID", invoiceId);
                conn.Open();

                using SqlDataReader dr = cmd.ExecuteReader();

                if (!dr.Read())
                    return null;

                var detail = new CorpInvoiceDetail
                {
                    invoiceId = GetInt(dr, "INVOICE_ID"),
                    invoiceNo = GetString(dr, "INVOICE_NO") ?? "",
                    corpNm = GetString(dr, "CORP_NM") ?? "",
                    corpGstin = GetString(dr, "CORP_GSTIN"),
                    billingPeriodFrom = GetNullableDateTime(dr, "BILLING_PERIOD_FROM") ?? DateTime.MinValue,
                    billingPeriodTo = GetNullableDateTime(dr, "BILLING_PERIOD_TO") ?? DateTime.MinValue,
                    totalBookings = GetInt(dr, "TOTAL_BOOKINGS"),
                    totalBaseAmt = GetDecimal(dr, "TOTAL_BASE_AMT"),
                    totalGstAmt = GetDecimal(dr, "TOTAL_GST_AMT"),
                    totalGrossAmt = GetDecimal(dr, "TOTAL_GROSS_AMT"),
                    discountAmt = GetDecimal(dr, "DISCOUNT_AMT"),
                    netPayableAmt = GetDecimal(dr, "NET_PAYABLE_AMT"),
                    dueDt = GetNullableDateTime(dr, "DUE_DT") ?? DateTime.MinValue,
                    status = GetString(dr, "STATUS") ?? "",
                    // NEW (Round B)
                    corpAddress = Has(dr, "CORP_ADDRESS") ? GetString(dr, "CORP_ADDRESS") : null,
                    invoiceDt = Has(dr, "CORP_INVOICE_DT") ? GetNullableDateTime(dr, "CORP_INVOICE_DT") : null,
                    paymentDt = Has(dr, "PAYMENT_DT") ? GetNullableDateTime(dr, "PAYMENT_DT") : null,
                    paymentRef = Has(dr, "PAYMENT_REF") ? GetString(dr, "PAYMENT_REF") : null,
                    remarks = Has(dr, "REMARKS") ? GetString(dr, "REMARKS") : null
                };

                if (dr.NextResult())
                {
                    while (dr.Read())
                    {
                        detail.bookings.Add(new CorpInvoiceBookingLine
                        {
                            bookingNo = GetString(dr, "BOOKING_NO") ?? "",
                            guestNm = GetString(dr, "GUEST_NM") ?? "",
                            roomNo = GetString(dr, "ROOM_NO") ?? "",
                            billDt = GetNullableDateTime(dr, "BILL_DT") ?? DateTime.MinValue,
                            totalGrossAmt = GetDecimal(dr, "TOTAL_GROSS_AMT"),
                            gstInvoiceNo = Has(dr, "GST_INVOICE_NO") ? GetString(dr, "GST_INVOICE_NO") : null,
                            checkoutDt = Has(dr, "CHECKOUT_DT") ? GetNullableDateTime(dr, "CHECKOUT_DT") : null,
                            baseAmt = Has(dr, "BASE_AMT") ? GetDecimal(dr, "BASE_AMT") : 0,
                            gstAmt = Has(dr, "GST_AMT") ? GetDecimal(dr, "GST_AMT") : 0
                        });
                    }
                }

                return detail;
            }
            catch (Exception ex)
            {
                throw new Exception("Error while fetching corporate invoice detail.", ex);
            }
        }

        // ==========================================
        // NEW (Round B): COMPANY PAYMENT / CANCEL
        // ==========================================

        public CorpInvoiceActionResult RecordPayment(CorpInvoicePaymentRequest request)
        {
            try
            {
                using SqlConnection conn = new SqlConnection(_conn);
                using SqlCommand cmd = new SqlCommand("DGMS.PR_DGMS_CORP_INVOICE_PAYMENT", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IP_SESSION_ID", request.sessionId);
                cmd.Parameters.AddWithValue("@IP_INVOICE_ID", request.invoiceId);
                cmd.Parameters.Add(new SqlParameter("@IP_AMOUNT", SqlDbType.Decimal) { Precision = 18, Scale = 2, Value = request.amount });
                cmd.Parameters.Add(new SqlParameter("@IP_TDS_AMOUNT", SqlDbType.Decimal) { Precision = 18, Scale = 2, Value = request.tdsAmount });
                cmd.Parameters.AddWithValue("@IP_PAYMENT_MODE", request.paymentMode ?? "");
                cmd.Parameters.AddWithValue("@IP_PAYMENT_REF", string.IsNullOrWhiteSpace(request.paymentRef) ? DBNull.Value : request.paymentRef.Trim());
                cmd.Parameters.AddWithValue("@IP_PAYMENT_DT", (object?)request.paymentDt ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@IP_REMARKS", string.IsNullOrWhiteSpace(request.remarks) ? DBNull.Value : request.remarks.Trim());
                return ExecAction(cmd, conn);
            }
            catch (Exception ex)
            {
                throw new Exception("Error while recording the company payment.", ex);
            }
        }

        public CorpInvoiceActionResult CancelInvoice(CorpInvoiceCancelRequest request)
        {
            try
            {
                using SqlConnection conn = new SqlConnection(_conn);
                using SqlCommand cmd = new SqlCommand("DGMS.PR_DGMS_CANCEL_CORP_INVOICE", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IP_SESSION_ID", request.sessionId);
                cmd.Parameters.AddWithValue("@IP_INVOICE_ID", request.invoiceId);
                cmd.Parameters.AddWithValue("@IP_REASON", (object?)request.reason?.Trim() ?? DBNull.Value);
                return ExecAction(cmd, conn);
            }
            catch (Exception ex)
            {
                throw new Exception("Error while cancelling the corporate invoice.", ex);
            }
        }

        private static CorpInvoiceActionResult ExecAction(SqlCommand cmd, SqlConnection conn)
        {
            var outStatus = new SqlParameter("@OUT_STATUS", SqlDbType.Char, 1) { Direction = ParameterDirection.Output };
            var outMessage = new SqlParameter("@OUT_MESSAGE", SqlDbType.VarChar, 500) { Direction = ParameterDirection.Output };
            cmd.Parameters.Add(outStatus);
            cmd.Parameters.Add(outMessage);
            conn.Open();
            cmd.ExecuteNonQuery();
            return new CorpInvoiceActionResult
            {
                status = Convert.ToString(outStatus.Value) ?? "",
                message = Convert.ToString(outMessage.Value) ?? ""
            };
        }

        // ==========================================
        // COMMON HELPERS
        // ==========================================

        private static bool Has(SqlDataReader dr, string column)
        {
            for (int i = 0; i < dr.FieldCount; i++)
                if (string.Equals(dr.GetName(i), column, StringComparison.OrdinalIgnoreCase)) return true;
            return false;
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

        private static DateTime? GetNullableDateTime(SqlDataReader dr, string column)
        {
            return dr[column] == DBNull.Value ? null : Convert.ToDateTime(dr[column]);
        }
    }
}
