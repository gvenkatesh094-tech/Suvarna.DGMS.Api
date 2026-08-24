using Microsoft.Data.SqlClient;
using Suvarna.DGMS.Api.Models;
using System.Data;

namespace Suvarna.DGMS.Api.Repositories
{
    public class GstReportsRepository
    {
        private readonly string _conn;

        public GstReportsRepository(IConfiguration configuration)
        {
            _conn = configuration.GetConnectionString("SqlServer");
        }

        // ==========================================
        // GST INVOICE REGISTER
        // ==========================================

        public List<GstInvoiceListItem> GetGstInvoices(
            int sessionId,
            DateTime fromDt,
            DateTime toDt,
            string? status,
            decimal? slabPct,
            string? returnType)
        {
            List<GstInvoiceListItem> list = new();

            try
            {
                using SqlConnection conn = new SqlConnection(_conn);
                using SqlCommand cmd = new SqlCommand("DGMS.PR_DGMS_GET_GST_INVOICES", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@IP_SESSION_ID", sessionId);
                cmd.Parameters.AddWithValue("@IP_FROM_DT", fromDt.Date);
                cmd.Parameters.AddWithValue("@IP_TO_DT", toDt.Date);
                AddParameter(cmd, "@IP_STATUS", status);
                AddParameter(cmd, "@IP_SLAB_PCT", slabPct);
                AddParameter(cmd, "@IP_RETURN_TYPE", returnType);

                conn.Open();

                using SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    list.Add(new GstInvoiceListItem
                    {
                        invoiceId = GetInt(dr, "INVOICE_ID"),
                        invoiceNo = GetString(dr, "INVOICE_NO") ?? "",
                        invoiceDt = GetNullableDateTime(dr, "INVOICE_DT") ?? DateTime.MinValue,
                        bookingId = GetInt(dr, "BOOKING_ID"),

                        customerName = GetString(dr, "CUSTOMER_NM") ?? "",
                        customerGstin = GetString(dr, "CUSTOMER_GSTIN"),

                        sacCode = GetString(dr, "SAC_CODE") ?? "",
                        gstSlabPct = GetDecimal(dr, "GST_SLAB_PCT"),

                        baseAmount = GetDecimal(dr, "BASE_AMT"),
                        cgstAmount = GetDecimal(dr, "CGST_AMT"),
                        sgstAmount = GetDecimal(dr, "SGST_AMT"),
                        totalTaxAmount = GetDecimal(dr, "TOTAL_TAX_AMT"),
                        grossAmount = GetDecimal(dr, "GROSS_AMT"),

                        invoiceStatus = GetString(dr, "INVOICE_STATUS") ?? "",
                        returnType = GetString(dr, "RETURN_TYPE") ?? ""
                    });
                }

                return list;
            }
            catch (Exception ex)
            {
                throw new Exception("Error while fetching GST invoices.", ex);
            }
        }

        // ==========================================
        // HSN / SAC SUMMARY
        // ==========================================

        public List<GstHsnSummary> GetHsnSummary(int sessionId, DateTime fromDt, DateTime toDt)
        {
            List<GstHsnSummary> list = new();

            try
            {
                using SqlConnection conn = new SqlConnection(_conn);
                using SqlCommand cmd = new SqlCommand("DGMS.PR_DGMS_GET_GST_HSN_SUMMARY", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@IP_SESSION_ID", sessionId);
                cmd.Parameters.AddWithValue("@IP_FROM_DT", fromDt.Date);
                cmd.Parameters.AddWithValue("@IP_TO_DT", toDt.Date);

                conn.Open();

                using SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    list.Add(new GstHsnSummary
                    {
                        sacCode = GetString(dr, "SAC_CODE") ?? "",
                        description = GetString(dr, "SAC_DESCRIPTION") ?? "",
                        taxableAmount = GetDecimal(dr, "TAXABLE_AMT"),
                        gstAmount = GetDecimal(dr, "GST_AMT")
                    });
                }

                return list;
            }
            catch (Exception ex)
            {
                throw new Exception("Error while fetching HSN/SAC summary.", ex);
            }
        }

        // ==========================================
        // MARK INVOICE PAID
        // ==========================================

        public GstActionResult MarkInvoicePaid(int sessionId, int invoiceId)
        {
            try
            {
                using SqlConnection conn = new SqlConnection(_conn);
                using SqlCommand cmd = new SqlCommand("DGMS.PR_DGMS_MARK_GST_INVOICE_PAID", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@IP_SESSION_ID", sessionId);
                cmd.Parameters.AddWithValue("@IP_INVOICE_ID", invoiceId);

                return ExecuteActionProc(conn, cmd);
            }
            catch (Exception ex)
            {
                throw new Exception("Error while marking GST invoice as paid.", ex);
            }
        }

        // ==========================================
        // CANCEL INVOICE
        // ==========================================

        public GstActionResult CancelInvoice(int sessionId, int invoiceId, string cancelReason)
        {
            try
            {
                using SqlConnection conn = new SqlConnection(_conn);
                using SqlCommand cmd = new SqlCommand("DGMS.PR_DGMS_CANCEL_GST_INVOICE", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@IP_SESSION_ID", sessionId);
                cmd.Parameters.AddWithValue("@IP_INVOICE_ID", invoiceId);
                cmd.Parameters.AddWithValue("@IP_CANCEL_REASON", cancelReason);

                return ExecuteActionProc(conn, cmd);
            }
            catch (Exception ex)
            {
                throw new Exception("Error while cancelling GST invoice.", ex);
            }
        }

        // ==========================================
        // GSTR-1 — B2B
        // ==========================================

        public List<Gstr1B2BInvoice> GetGstr1B2B(int sessionId, DateTime fromDt, DateTime toDt)
        {
            List<Gstr1B2BInvoice> list = new();

            try
            {
                using SqlConnection conn = new SqlConnection(_conn);
                using SqlCommand cmd = new SqlCommand("DGMS.PR_DGMS_GET_GSTR1_B2B", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@IP_SESSION_ID", sessionId);
                cmd.Parameters.AddWithValue("@IP_FROM_DT", fromDt.Date);
                cmd.Parameters.AddWithValue("@IP_TO_DT", toDt.Date);

                conn.Open();

                using SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    list.Add(new Gstr1B2BInvoice
                    {
                        invoiceNo = GetString(dr, "INVOICE_NO") ?? "",
                        invoiceDt = GetNullableDateTime(dr, "INVOICE_DT") ?? DateTime.MinValue,
                        invoiceValue = GetDecimal(dr, "INVOICE_VALUE"),
                        sacCode = GetString(dr, "SAC_CODE") ?? "",
                        taxableValue = GetDecimal(dr, "TAXABLE_VALUE"),
                        gstRate = GetDecimal(dr, "GST_RATE"),
                        cgst = GetDecimal(dr, "CGST"),
                        sgst = GetDecimal(dr, "SGST"),
                        igst = GetDecimal(dr, "IGST"),
                        receiverGstin = GetString(dr, "RECEIVER_GSTIN"),
                        receiverName = GetString(dr, "RECEIVER_NAME") ?? ""
                    });
                }

                return list;
            }
            catch (Exception ex)
            {
                throw new Exception("Error while fetching GSTR-1 B2B data.", ex);
            }
        }

        // ==========================================
        // GSTR-1 — B2C
        // ==========================================

        public List<Gstr1B2CSummary> GetGstr1B2C(int sessionId, DateTime fromDt, DateTime toDt)
        {
            List<Gstr1B2CSummary> list = new();

            try
            {
                using SqlConnection conn = new SqlConnection(_conn);
                using SqlCommand cmd = new SqlCommand("DGMS.PR_DGMS_GET_GSTR1_B2C", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@IP_SESSION_ID", sessionId);
                cmd.Parameters.AddWithValue("@IP_FROM_DT", fromDt.Date);
                cmd.Parameters.AddWithValue("@IP_TO_DT", toDt.Date);

                conn.Open();

                using SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    list.Add(new Gstr1B2CSummary
                    {
                        stateCode = GetString(dr, "STATE_CD") ?? "",
                        sacCode = GetString(dr, "SAC_CODE") ?? "",
                        gstRate = GetDecimal(dr, "GST_RATE"),
                        taxableValue = GetDecimal(dr, "TAXABLE_VALUE"),
                        cgst = GetDecimal(dr, "CGST"),
                        sgst = GetDecimal(dr, "SGST"),
                        totalTax = GetDecimal(dr, "TOTAL_TAX"),
                        invoiceCount = GetInt(dr, "INVOICE_COUNT")
                    });
                }

                return list;
            }
            catch (Exception ex)
            {
                throw new Exception("Error while fetching GSTR-1 B2C data.", ex);
            }
        }

        // ==========================================
        // GSTR-3B SUMMARY
        // ==========================================

        public Gstr3BSummary GetGstr3BSummary(int sessionId, DateTime fromDt, DateTime toDt)
        {
            try
            {
                using SqlConnection conn = new SqlConnection(_conn);
                using SqlCommand cmd = new SqlCommand("DGMS.PR_DGMS_GET_GSTR3B_SUMMARY", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@IP_SESSION_ID", sessionId);
                cmd.Parameters.AddWithValue("@IP_FROM_DT", fromDt.Date);
                cmd.Parameters.AddWithValue("@IP_TO_DT", toDt.Date);

                conn.Open();

                using SqlDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    return new Gstr3BSummary
                    {
                        invoiceCount = GetInt(dr, "INVOICE_COUNT"),
                        totalTaxableValue = GetDecimal(dr, "TOTAL_TAXABLE_VALUE"),
                        totalCgst = GetDecimal(dr, "TOTAL_CGST"),
                        totalSgst = GetDecimal(dr, "TOTAL_SGST"),
                        totalIgst = GetDecimal(dr, "TOTAL_IGST"),
                        totalTax = GetDecimal(dr, "TOTAL_TAX"),
                        totalGross = GetDecimal(dr, "TOTAL_GROSS")
                    };
                }

                return new Gstr3BSummary();
            }
            catch (Exception ex)
            {
                throw new Exception("Error while fetching GSTR-3B summary.", ex);
            }
        }

        // ==========================================
        // COMPLIANCE STATUS
        // ==========================================

        public List<GstComplianceItem> GetComplianceStatus(int sessionId, int topN)
        {
            List<GstComplianceItem> list = new();

            try
            {
                using SqlConnection conn = new SqlConnection(_conn);
                using SqlCommand cmd = new SqlCommand("DGMS.PR_DGMS_GET_GST_COMPLIANCE_STATUS", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@IP_SESSION_ID", sessionId);
                cmd.Parameters.AddWithValue("@IP_TOP_N", topN);

                conn.Open();

                using SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    list.Add(new GstComplianceItem
                    {
                        filingId = GetInt(dr, "FILING_ID"),
                        returnType = GetString(dr, "RETURN_TYPE") ?? "",
                        periodMonth = GetInt(dr, "PERIOD_MONTH"),
                        periodYear = GetInt(dr, "PERIOD_YEAR"),
                        dueDt = GetNullableDateTime(dr, "DUE_DT") ?? DateTime.MinValue,
                        filedDt = GetNullableDateTime(dr, "FILED_DT"),
                        arnNo = GetString(dr, "ARN_NO"),
                        filingStatus = GetString(dr, "FILING_STATUS") ?? "",
                        daysToDue = GetInt(dr, "DAYS_TO_DUE")
                    });
                }

                return list;
            }
            catch (Exception ex)
            {
                throw new Exception("Error while fetching GST compliance status.", ex);
            }
        }

        // ==========================================
        // MARK RETURN AS FILED
        // ==========================================

        public GstActionResult MarkFiled(int sessionId, int filingId, string? arnNo)
        {
            try
            {
                using SqlConnection conn = new SqlConnection(_conn);
                using SqlCommand cmd = new SqlCommand("DGMS.PR_DGMS_MARK_GST_FILED", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@IP_SESSION_ID", sessionId);
                cmd.Parameters.AddWithValue("@IP_FILING_ID", filingId);
                AddParameter(cmd, "@IP_ARN_NO", arnNo);

                return ExecuteActionProc(conn, cmd);
            }
            catch (Exception ex)
            {
                throw new Exception("Error while marking GST return as filed.", ex);
            }
        }

        // ==========================================
        // COMMON HELPERS
        // ==========================================

        private static GstActionResult ExecuteActionProc(SqlConnection conn, SqlCommand cmd)
        {
            SqlParameter status = new SqlParameter("@OUT_STATUS", SqlDbType.Char, 1)
            { Direction = ParameterDirection.Output };
            cmd.Parameters.Add(status);

            SqlParameter message = new SqlParameter("@OUT_MESSAGE", SqlDbType.VarChar, 500)
            { Direction = ParameterDirection.Output };
            cmd.Parameters.Add(message);

            conn.Open();
            cmd.ExecuteNonQuery();

            return new GstActionResult
            {
                status = Convert.ToString(status.Value) ?? "",
                message = Convert.ToString(message.Value) ?? ""
            };
        }

        private static void AddParameter(SqlCommand cmd, string name, object? value)
        {
            cmd.Parameters.AddWithValue(name, value ?? DBNull.Value);
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
