using Microsoft.Data.SqlClient;
using Suvarna.DGMS.Api.Models;
using Suvarna.DGMS.Api.Repositories;

namespace Suvarna.DGMS.Api.Services
{
    public class ReceiptsService
    {
        private static readonly HashSet<string> DocTypes = new(StringComparer.OrdinalIgnoreCase)
        {
            "CHECKIN", "ADVANCE", "FOLIO", "LAUNDRY", "REFUND", "VISITOR",
            "FINAL_BILL", "TAX_INVOICE", "CORP_RECEIPT", "BILL_DETAILS"
        };

        private readonly ReceiptsRepository _repo;
        private readonly ILogger<ReceiptsService> _log;

        public ReceiptsService(ReceiptsRepository repo, ILogger<ReceiptsService> log)
        {
            _repo = repo;
            _log = log;
        }

        public ApiResponse<ReceiptPrintInfo> LogPrint(ReceiptPrintRequest req)
        {
            if (!DocTypes.Contains(req.DocType ?? ""))
                return ApiResponse<ReceiptPrintInfo>.Fail("Unknown document type.", "400");
            if (string.IsNullOrWhiteSpace(req.DocNo) || req.DocNo.Length > 60)
                return ApiResponse<ReceiptPrintInfo>.Fail("Document number is required.", "400");

            req.DocType = req.DocType!.ToUpperInvariant();
            req.DocNo = req.DocNo.Trim();
            req.SizeGroup = string.Equals(req.SizeGroup, "OFFICE", StringComparison.OrdinalIgnoreCase) ? "OFFICE" : "COUNTER";

            try
            {
                var info = _repo.LogPrint(req);
                return info == null
                    ? ApiResponse<ReceiptPrintInfo>.Fail("Print could not be logged.", "500")
                    : ApiResponse<ReceiptPrintInfo>.Success("Print logged.", info);
            }
            catch (SqlException ex) when (ex.Number >= 50000)
            {
                return ApiResponse<ReceiptPrintInfo>.Fail(ex.Message, "400");
            }
            catch (Exception ex)
            {
                _log.LogError(ex, "Receipt print log failed for {DocType} {DocNo}", req.DocType, req.DocNo);
                return ApiResponse<ReceiptPrintInfo>.Fail("Print could not be logged.", "500");
            }
        }
    }
}
