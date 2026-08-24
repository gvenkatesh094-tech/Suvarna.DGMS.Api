using Suvarna.DGMS.Api.Models;
using Suvarna.DGMS.Api.Repositories;

namespace Suvarna.DGMS.Api.Services
{
    public class GstReportsService
    {
        private static readonly string[] ValidInvoiceStatuses = { "ISSUED", "PAID", "CANCELLED" };
        private static readonly string[] ValidReturnTypes = { "B2B", "B2C" };
        private static readonly decimal[] ValidSlabs = { 0m, 5m, 12m, 18m, 28m };
        private const int MaxDateRangeDays = 366;

        private readonly GstReportsRepository _repo;

        public GstReportsService(GstReportsRepository repo)
        {
            _repo = repo;
        }

        // ==========================================
        // GST INVOICE REGISTER
        // ==========================================

        public ApiResponse<List<GstInvoiceListItem>> GetGstInvoices(
            int sessionId,
            DateTime fromDt,
            DateTime toDt,
            string? status,
            decimal? slabPct,
            string? returnType)
        {
            ValidateDateRange(fromDt, toDt);

            if (!string.IsNullOrWhiteSpace(status) && !ValidInvoiceStatuses.Contains(status.Trim().ToUpperInvariant()))
                throw new Exception("Invalid status filter. Use ISSUED, PAID or CANCELLED.");

            if (slabPct.HasValue && !ValidSlabs.Contains(slabPct.Value))
                throw new Exception("Invalid GST slab filter.");

            if (!string.IsNullOrWhiteSpace(returnType) && !ValidReturnTypes.Contains(returnType.Trim().ToUpperInvariant()))
                throw new Exception("Invalid return type filter. Use B2B or B2C.");

            var data = _repo.GetGstInvoices(sessionId, fromDt, toDt, status, slabPct, returnType);

            return ApiResponse<List<GstInvoiceListItem>>.Success(
                data.Count == 0 ? "No GST invoices found for the selected filters." : "GST invoices fetched successfully.",
                data
            );
        }

        // ==========================================
        // HSN / SAC SUMMARY
        // ==========================================

        public ApiResponse<List<GstHsnSummary>> GetHsnSummary(int sessionId, DateTime fromDt, DateTime toDt)
        {
            ValidateDateRange(fromDt, toDt);

            var data = _repo.GetHsnSummary(sessionId, fromDt, toDt);

            return ApiResponse<List<GstHsnSummary>>.Success(
                data.Count == 0 ? "No SAC-wise charges found for the selected period." : "HSN/SAC summary fetched successfully.",
                data
            );
        }

        // ==========================================
        // MARK INVOICE PAID
        // ==========================================

        public ApiResponse<GstActionResult> MarkInvoicePaid(MarkGstInvoicePaidRequest request)
        {
            if (request == null) throw new Exception("Request payload is required.");
            if (request.invoiceId <= 0) throw new Exception("A valid invoice is required.");

            var data = _repo.MarkInvoicePaid(request.sessionId, request.invoiceId);

            if (data.status != "S")
                throw new Exception(string.IsNullOrWhiteSpace(data.message) ? "Unable to mark invoice as paid." : data.message);

            return ApiResponse<GstActionResult>.Success(data.message, data);
        }

        // ==========================================
        // CANCEL INVOICE
        // ==========================================

        public ApiResponse<GstActionResult> CancelInvoice(CancelGstInvoiceRequest request)
        {
            if (request == null) throw new Exception("Request payload is required.");
            if (request.invoiceId <= 0) throw new Exception("A valid invoice is required.");

            var reason = request.cancelReason?.Trim() ?? "";
            if (reason.Length < 5)
                throw new Exception("Cancellation reason must be at least 5 characters.");
            if (reason.Length > 300)
                throw new Exception("Cancellation reason cannot exceed 300 characters.");

            var data = _repo.CancelInvoice(request.sessionId, request.invoiceId, reason);

            if (data.status != "S")
                throw new Exception(string.IsNullOrWhiteSpace(data.message) ? "Unable to cancel invoice." : data.message);

            return ApiResponse<GstActionResult>.Success(data.message, data);
        }

        // ==========================================
        // GSTR-1 (B2B + B2C)
        // ==========================================

        public ApiResponse<List<Gstr1B2BInvoice>> GetGstr1B2B(int sessionId, DateTime fromDt, DateTime toDt)
        {
            ValidateDateRange(fromDt, toDt);
            var data = _repo.GetGstr1B2B(sessionId, fromDt, toDt);
            return ApiResponse<List<Gstr1B2BInvoice>>.Success(
                data.Count == 0 ? "No B2B invoices found for the selected period." : "GSTR-1 B2B data fetched successfully.",
                data
            );
        }

        public ApiResponse<List<Gstr1B2CSummary>> GetGstr1B2C(int sessionId, DateTime fromDt, DateTime toDt)
        {
            ValidateDateRange(fromDt, toDt);
            var data = _repo.GetGstr1B2C(sessionId, fromDt, toDt);
            return ApiResponse<List<Gstr1B2CSummary>>.Success(
                data.Count == 0 ? "No B2C invoices found for the selected period." : "GSTR-1 B2C data fetched successfully.",
                data
            );
        }

        // ==========================================
        // GSTR-3B SUMMARY
        // ==========================================

        public ApiResponse<Gstr3BSummary> GetGstr3BSummary(int sessionId, DateTime fromDt, DateTime toDt)
        {
            ValidateDateRange(fromDt, toDt);
            var data = _repo.GetGstr3BSummary(sessionId, fromDt, toDt);
            return ApiResponse<Gstr3BSummary>.Success("GSTR-3B summary fetched successfully.", data);
        }

        // ==========================================
        // COMPLIANCE STATUS
        // ==========================================

        public ApiResponse<List<GstComplianceItem>> GetComplianceStatus(int sessionId, int topN)
        {
            if (topN <= 0 || topN > 50) topN = 6;

            var data = _repo.GetComplianceStatus(sessionId, topN);

            return ApiResponse<List<GstComplianceItem>>.Success(
                data.Count == 0 ? "No filing records configured yet." : "Compliance status fetched successfully.",
                data
            );
        }

        public ApiResponse<GstActionResult> MarkFiled(MarkGstFiledRequest request)
        {
            if (request == null) throw new Exception("Request payload is required.");
            if (request.filingId <= 0) throw new Exception("A valid filing record is required.");

            if (!string.IsNullOrWhiteSpace(request.arnNo) && request.arnNo.Trim().Length > 30)
                throw new Exception("ARN number cannot exceed 30 characters.");

            var data = _repo.MarkFiled(request.sessionId, request.filingId, request.arnNo?.Trim());

            if (data.status != "S")
                throw new Exception(string.IsNullOrWhiteSpace(data.message) ? "Unable to mark return as filed." : data.message);

            return ApiResponse<GstActionResult>.Success(data.message, data);
        }

        // ==========================================
        // VALIDATION HELPERS
        // ==========================================

        private static void ValidateDateRange(DateTime fromDt, DateTime toDt)
        {
            if (fromDt == default || toDt == default)
                throw new Exception("From and To dates are required.");

            if (fromDt.Date > toDt.Date)
                throw new Exception("From date cannot be after To date.");

            if ((toDt.Date - fromDt.Date).TotalDays > MaxDateRangeDays)
                throw new Exception($"Date range cannot exceed {MaxDateRangeDays} days.");
        }
    }
}
