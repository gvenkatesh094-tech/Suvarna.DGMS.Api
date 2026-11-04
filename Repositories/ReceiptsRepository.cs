using Microsoft.Data.SqlClient;
using Suvarna.DGMS.Api.Models;
using System.Data;

namespace Suvarna.DGMS.Api.Repositories
{
    /// <summary>
    /// NEW 04-Nov: standard receipt print log + header. Proc DGMS.PR_DGMS_RECEIPT_PRINT
    /// (scripts/receipts/2026-11-04_receipt_standard.sql).
    /// </summary>
    public class ReceiptsRepository
    {
        private readonly string _conn;

        public ReceiptsRepository(IConfiguration configuration)
        {
            _conn = configuration.GetConnectionString("SqlServer");
        }

        public ReceiptPrintInfo? LogPrint(ReceiptPrintRequest req)
        {
            using SqlConnection conn = new SqlConnection(_conn);
            using SqlCommand cmd = new SqlCommand("DGMS.PR_DGMS_RECEIPT_PRINT", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandTimeout = 30;

            cmd.Parameters.Add("@IP_SESSION_ID", SqlDbType.VarChar, 100).Value = req.SessionId.ToString();
            cmd.Parameters.Add("@IP_DOC_TYPE", SqlDbType.VarChar, 20).Value = req.DocType;
            cmd.Parameters.Add("@IP_DOC_NO", SqlDbType.VarChar, 60).Value = req.DocNo;
            cmd.Parameters.Add("@IP_SIZE_GROUP", SqlDbType.VarChar, 10).Value = req.SizeGroup;
            cmd.Parameters.Add("@IP_PROPERTY_ID", SqlDbType.Int).Value = (object?)req.PropertyId ?? DBNull.Value;
            cmd.Parameters.Add("@IP_REPRINT", SqlDbType.Char, 1).Value = req.Reprint ? "Y" : "N";

            conn.Open();
            using SqlDataReader dr = cmd.ExecuteReader();
            if (!dr.Read()) return null;

            return new ReceiptPrintInfo
            {
                CopyNo = dr["COPY_NO"] == DBNull.Value ? 1 : Convert.ToInt32(dr["COPY_NO"]),
                PaperSize = Str(dr, "PAPER_SIZE") ?? "80MM",
                PrintedDt = dr["PRINTED_DT"] == DBNull.Value
                    ? DateTime.UtcNow
                    : DateTime.SpecifyKind(Convert.ToDateTime(dr["PRINTED_DT"]), DateTimeKind.Utc),
                PrintedByNm = Str(dr, "PRINTED_BY_NM"),
                PrintedById = dr["PRINTED_BY_ID"] == DBNull.Value ? null : Convert.ToInt32(dr["PRINTED_BY_ID"]),
                PropertyNm = Str(dr, "PROPERTY_NM"),
                PropertyAddress = Str(dr, "PROPERTY_ADDRESS"),
                PropertyPhone = Str(dr, "PROPERTY_PHONE"),
                PropertyGstin = Str(dr, "PROPERTY_GSTIN")
            };
        }

        private static string? Str(SqlDataReader dr, string col)
            => dr[col] == DBNull.Value ? null : Convert.ToString(dr[col]);
    }
}
