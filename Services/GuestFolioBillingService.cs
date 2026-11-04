using Suvarna.DGMS.Api.Helpers;
using Suvarna.DGMS.Api.Models;
using Suvarna.DGMS.Api.Repositories;

namespace Suvarna.DGMS.Api.Services
{
    public class GuestFolioBillingService
    {
        private readonly GuestFolioBillingRepository _repo;
        private readonly PosPaymentService _pos;          // NEW: POS machine payments
        private readonly SessionContext _session;

        public GuestFolioBillingService(GuestFolioBillingRepository repo, PosPaymentService pos, SessionContext session)
        {
            _repo = repo;
            _pos = pos;
            _session = session;
        }

        // ==========================================
        // GET BOOKINGS
        // ==========================================

        public ApiResponse<List<BillingBookingLookup>> GetBookings(int sessionId, int? propertyId, bool includeCheckedOut = false, bool includeSettled = false)
        {
            var data = _repo.GetBookings(sessionId, propertyId, includeCheckedOut, includeSettled);

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
                throw new UserMessageException(string.IsNullOrWhiteSpace(data.message) ? "Failed to post charge." : data.message);
            }

            return ApiResponse<ChargeResult>.Success(data.message, data);
        }

        // REMOVED (Round B): UPDATE CHARGE - no screen used it, and it let any caller rewrite a charge's
        // amount, GST and status directly. Wrong charges are cancelled and posted again.

        // ==========================================
        // CANCEL CHARGE — NEW
        // ==========================================

        public ApiResponse<ChargeResult> CancelCharge(CancelChargeRequest request)
        {
            var data = _repo.CancelCharge(request);

            if (data.status != "S")
            {
                // "Already cancelled" / "bill transferred" land here and
                // surface to the UI as a toast rather than a false success.
                throw new UserMessageException(string.IsNullOrWhiteSpace(data.message) ? "Failed to cancel charge." : data.message);
            }

            return ApiResponse<ChargeResult>.Success(data.message, data);
        }

        // ==========================================
        // COLLECT PAYMENT
        // ==========================================

        public ApiResponse<PaymentResult> CollectPayment(CollectPaymentRequest request)
        {
            // NEW: card paid on the POS machine -> the RRN comes from the verified POS payment
            var sid = _session.Require(request.sessionId);
            var pos = _pos.ClaimForPayment(sid, request.posTxnId, "FOLIO", request.bookingId, request.amount, request.paymentMode);
            if (pos != null) request.paymentRef = pos.Rrn;

            PaymentResult data;
            try
            {
                data = _repo.CollectPayment(request);
            }
            catch
            {
                _pos.SafeRelease(sid, pos);
                throw;
            }

            if (data.status != "S")
            {
                _pos.SafeRelease(sid, pos);
                throw new UserMessageException(string.IsNullOrWhiteSpace(data.message) ? "Failed to collect payment." : data.message);
            }

            var posWarning = _pos.LinkAfterSave(sid, pos, request.bookingId, null, data.paymentId);
            if (posWarning != null) data.message = (data.message ?? "") + " | " + posWarning;

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
                throw new UserMessageException(string.IsNullOrWhiteSpace(data.message) ? "Failed to transfer charges to IP." : data.message);
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
                throw new UserMessageException(string.IsNullOrWhiteSpace(data.message) ? "Failed to generate invoice." : data.message);
            }

            return ApiResponse<InvoiceResult>.Success(data.message, data);
        }

        // ==========================================
        // NEW 04-Nov: PAYMENT RECEIPT
        // ==========================================

        public ApiResponse<FolioReceipt?> GetReceipt(int sessionId, int paymentId)
        {
            if (paymentId <= 0)
                throw new UserMessageException("Receipt number is required.");

            var data = _repo.GetReceipt(sessionId, paymentId);

            if (data == null)
                return ApiResponse<FolioReceipt?>.Fail("Receipt not found.", "404");

            return ApiResponse<FolioReceipt?>.Success("Receipt fetched successfully.", data);
        }

        // ==========================================
        // NEW (Round B): PRINT GST INVOICE
        // ==========================================

        public ApiResponse<GstInvoicePrint?> GetInvoicePrint(int sessionId, int? bookingId, int? invoiceId)
        {
            if ((bookingId ?? 0) <= 0 && (invoiceId ?? 0) <= 0)
                throw new UserMessageException("Booking or invoice is required.");

            var data = _repo.GetInvoicePrint(sessionId, bookingId > 0 ? bookingId : null, invoiceId > 0 ? invoiceId : null);

            if (data == null)
                return ApiResponse<GstInvoicePrint?>.Fail("No GST invoice found. It is issued after check-out.", "404");

            return ApiResponse<GstInvoicePrint?>.Success("GST invoice fetched successfully.", data);
        }
    }
}
