using Suvarna.DGMS.Api.Models;
using Suvarna.DGMS.Api.Repositories;

namespace Suvarna.DGMS.Api.Services
{
    public class CorporateInvoicesService
    {
        private readonly CorporateInvoicesRepository _repo;

        public CorporateInvoicesService(CorporateInvoicesRepository repo)
        {
            _repo = repo;
        }

        public ApiResponse<List<CorpInvoiceRow>> GetInvoices(int sessionId)
        {
            var data = _repo.GetInvoices(sessionId);

            return ApiResponse<List<CorpInvoiceRow>>.Success(
                "Corporate invoices fetched successfully.",
                data
            );
        }

        public ApiResponse<List<CorpAgreementLookup>> GetAgreements(int sessionId, bool includeExpired = false)
        {
            var data = _repo.GetAgreements(sessionId, includeExpired);

            return ApiResponse<List<CorpAgreementLookup>>.Success(
                "Corporate agreements fetched successfully.",
                data
            );
        }

        public ApiResponse<CorpInvoiceGenerateResult> GenerateInvoice(GenerateCorpInvoiceRequest request)
        {
            var data = _repo.GenerateInvoice(request);

            if (data.status != "S")
            {
                throw new Suvarna.DGMS.Api.Helpers.UserMessageException(string.IsNullOrWhiteSpace(data.message) ? "Failed to generate corporate invoice." : data.message);
            }

            return ApiResponse<CorpInvoiceGenerateResult>.Success(data.message, data);
        }

        public ApiResponse<CorpInvoiceDetail?> GetInvoiceDetail(int sessionId, int invoiceId)
        {
            var data = _repo.GetInvoiceDetail(sessionId, invoiceId);

            if (data == null)
            {
                return ApiResponse<CorpInvoiceDetail?>.Success("Invoice not found.", null);
            }

            return ApiResponse<CorpInvoiceDetail?>.Success("Invoice detail fetched successfully.", data);
        }

        // NEW (Round B): company payment (amount received + TDS deducted = invoice amount)
        public ApiResponse<CorpInvoiceActionResult> RecordPayment(CorpInvoicePaymentRequest request)
        {
            if (request == null || request.invoiceId <= 0)
                throw new Suvarna.DGMS.Api.Helpers.UserMessageException("Select an invoice.");
            if (request.amount < 0 || request.tdsAmount < 0 || request.amount + request.tdsAmount <= 0)
                throw new Suvarna.DGMS.Api.Helpers.UserMessageException("Enter the amount received (and TDS deducted, if any).");
            if (request.amount != Math.Round(request.amount, 2) || request.tdsAmount != Math.Round(request.tdsAmount, 2))
                throw new Suvarna.DGMS.Api.Helpers.UserMessageException("Amounts can have at most 2 decimal places.");
            if (!string.IsNullOrWhiteSpace(request.paymentRef) && request.paymentRef.Trim().Length > 50)
                throw new Suvarna.DGMS.Api.Helpers.UserMessageException("Payment reference cannot exceed 50 characters.");

            var data = _repo.RecordPayment(request);
            if (data.status != "S")
                throw new Suvarna.DGMS.Api.Helpers.UserMessageException(string.IsNullOrWhiteSpace(data.message) ? "Failed to record the payment." : data.message);

            return ApiResponse<CorpInvoiceActionResult>.Success(data.message, data);
        }

        // NEW (Round B)
        public ApiResponse<CorpInvoiceActionResult> CancelInvoice(CorpInvoiceCancelRequest request)
        {
            if (request == null || request.invoiceId <= 0)
                throw new Suvarna.DGMS.Api.Helpers.UserMessageException("Select an invoice.");
            if (string.IsNullOrWhiteSpace(request.reason) || request.reason.Trim().Length < 5)
                throw new Suvarna.DGMS.Api.Helpers.UserMessageException("Cancellation reason must be at least 5 characters.");

            var data = _repo.CancelInvoice(request);
            if (data.status != "S")
                throw new Suvarna.DGMS.Api.Helpers.UserMessageException(string.IsNullOrWhiteSpace(data.message) ? "Failed to cancel the invoice." : data.message);

            return ApiResponse<CorpInvoiceActionResult>.Success(data.message, data);
        }
    }
}
