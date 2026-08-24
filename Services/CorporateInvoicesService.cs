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

        public ApiResponse<List<CorpAgreementLookup>> GetAgreements(int sessionId)
        {
            var data = _repo.GetAgreements(sessionId);

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
                throw new Exception(string.IsNullOrWhiteSpace(data.message) ? "Failed to generate corporate invoice." : data.message);
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
    }
}
