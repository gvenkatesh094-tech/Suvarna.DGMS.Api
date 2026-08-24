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
                        status = GetString(dr, "STATUS") ?? ""
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

        public List<CorpAgreementLookup> GetAgreements(int sessionId)
        {
            List<CorpAgreementLookup> list = new();

            try
            {
                using SqlConnection conn = new SqlConnection(_conn);
                using SqlCommand cmd = new SqlCommand("DGMS.PR_DGMS_GET_CORP_AGREEMENTS_LOOKUP", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IP_SESSION_ID", sessionId);
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
                    status = GetString(dr, "STATUS") ?? ""
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
                            totalGrossAmt = GetDecimal(dr, "TOTAL_GROSS_AMT")
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
