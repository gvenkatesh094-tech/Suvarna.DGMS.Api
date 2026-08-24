using Suvarna.DGMS.Api.Models;
using Suvarna.DGMS.Api.Repositories;

namespace Suvarna.DGMS.Api.Services
{
    public class GuestFolioBillingService
    {
        private readonly GuestFolioBillingRepository _repo;

        public GuestFolioBillingService(GuestFolioBillingRepository repo)
        {
            _repo = repo;
        }

        // ==========================================
        // GET BOOKINGS
        // ==========================================

        public ApiResponse<List<BillingBookingLookup>> GetBookings(int sessionId, int? propertyId)
        {
            var data = _repo.GetBookings(sessionId, propertyId);

            return ApiResponse<List<BillingBookingLookup>>.Success(
                "Guest list fetched successfully.",
                data
            );
        }

        // ==========================================
        // GET SERVICES
        // ==========================================

        public ApiResponse<List<BillingServiceLookup>> GetServices(int sessionId, int? propertyId)
        {
            var data = _repo.GetServices(sessionId, propertyId);

            return ApiResponse<List<BillingServiceLookup>>.Success(
                "Services fetched successfully.",
                data
            );
        }

        // ==========================================
        // GET FOLIO
        // ==========================================

        public ApiResponse<BillingFolio?> GetFolio(int sessionId, int bookingId)
        {
            var data = _repo.GetFolio(sessionId, bookingId);

            if (data == null)
            {
                return ApiResponse<BillingFolio?>.Success("No folio found for this booking.", null);
            }

            return ApiResponse<BillingFolio?>.Success("Guest folio fetched successfully.", data);
        }

        // ==========================================
        // ADD CHARGE
        // ==========================================

        public ApiResponse<ChargeResult> AddCharge(AddChargeRequest request)
        {
            var data = _repo.AddCharge(request);

            if (data.status != "S")
            {
                throw new Exception(string.IsNullOrWhiteSpace(data.message) ? "Failed to post charge." : data.message);
            }

            return ApiResponse<ChargeResult>.Success(data.message, data);
        }

        // ==========================================
        // UPDATE CHARGE
        // ==========================================

        public ApiResponse<ChargeResult> UpdateCharge(UpdateChargeRequest request)
        {
            var data = _repo.UpdateCharge(request);

            if (data.status != "S")
            {
                throw new Exception(string.IsNullOrWhiteSpace(data.message) ? "Failed to update charge." : data.message);
            }

            return ApiResponse<ChargeResult>.Success(data.message, data);
        }

        // ==========================================
        // COLLECT PAYMENT
        // ==========================================

        public ApiResponse<PaymentResult> CollectPayment(CollectPaymentRequest request)
        {
            var data = _repo.CollectPayment(request);

            if (data.status != "S")
            {
                throw new Exception(string.IsNullOrWhiteSpace(data.message) ? "Failed to collect payment." : data.message);
            }

            return ApiResponse<PaymentResult>.Success(data.message, data);
        }

        // ==========================================
        // TRANSFER TO IP
        // ==========================================

        public ApiResponse<TransferResult> TransferToIp(TransferToIpRequest request)
        {
            var data = _repo.TransferToIp(request);

            if (data.status != "S")
            {
                throw new Exception(string.IsNullOrWhiteSpace(data.message) ? "Failed to transfer charges to IP." : data.message);
            }

            return ApiResponse<TransferResult>.Success(data.message, data);
        }

        // ==========================================
        // GENERATE INVOICE
        // ==========================================

        public ApiResponse<InvoiceResult> GenerateInvoice(GenerateInvoiceRequest request)
        {
            var data = _repo.GenerateInvoice(request);

            if (data.status != "S")
            {
                throw new Exception(string.IsNullOrWhiteSpace(data.message) ? "Failed to generate invoice." : data.message);
            }

            return ApiResponse<InvoiceResult>.Success(data.message, data);
        }
    }
}
