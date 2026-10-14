using Suvarna.DGMS.Api.Models;
using Suvarna.DGMS.Api.Repositories;

namespace Suvarna.DGMS.Api.Services
{
    public class CheckoutService
    {
        private readonly CheckoutRepository _repo;

        private static readonly HashSet<string> AllowedPaymentModes =
            new(StringComparer.OrdinalIgnoreCase) { "CASH", "CARD", "UPI", "ONLINE", "CORP_BILL" };

        // Same list as DepositLedgerService.ValidRefundModes / PR_DGMS_REFUND_DEPOSIT
        private static readonly HashSet<string> AllowedRefundModes =
            new(StringComparer.OrdinalIgnoreCase) { "CASH", "UPI", "BANK_TRANSFER", "CARD_REVERSAL", "CHEQUE" };

        public CheckoutService(CheckoutRepository repo)
        {
            _repo = repo;
        }

        //====================================================
        // GET CHECKOUT BOOKINGS
        //====================================================

        public ApiResponse<List<CheckoutBooking>> GetCheckoutBookings(int sessionId, string? searchTerm = null)
        {
            var data = _repo.GetCheckoutBookings(sessionId, searchTerm);
            return ApiResponse<List<CheckoutBooking>>.Success("Checkout bookings fetched successfully.", data);
        }

        //====================================================
        // PREPARE CHECKOUT
        //====================================================

        public ApiResponse<CheckoutDetails> PrepareCheckout(int sessionId, int bookingId, DateTime checkoutDt)
        {
            var data = _repo.PrepareCheckout(sessionId, bookingId, checkoutDt)
                ?? throw new Exception("Checkout details not found.");

            // FIX: someone else may have checked this guest out already
            if (!string.IsNullOrEmpty(data.bookingStatus) && data.bookingStatus != "CHECKEDIN")
            {
                throw new Exception("This booking is no longer checked in — it may already have been checked out.");
            }

            return ApiResponse<CheckoutDetails>.Success("Checkout details fetched successfully.", data);
        }

        //====================================================
        // GET CHECKOUT DETAILS
        //====================================================

        public ApiResponse<CheckoutDetails> GetCheckoutDetails(int sessionId, int bookingId, DateTime? checkoutDt)
        {
            var data = _repo.GetCheckoutDetails(sessionId, bookingId, checkoutDt)
                ?? throw new Exception("Checkout details not found.");

            return ApiResponse<CheckoutDetails>.Success("Checkout details fetched successfully.", data);
        }

        //====================================================
        // CONFIRM CHECKOUT
        //====================================================

        public ApiResponse<CheckoutResult> ConfirmCheckout(CheckoutRequest request)
        {
            ValidateCheckout(request);

            var data = _repo.ConfirmCheckout(request);

            if (data.status != "S")
            {
                throw new Exception(string.IsNullOrWhiteSpace(data.message) ? "Checkout failed." : data.message);
            }

            return ApiResponse<CheckoutResult>.Success(data.message, data);
        }

        //====================================================
        // SERVER VALIDATION
        //====================================================

        private static void ValidateCheckout(CheckoutRequest request)
        {
            if (request == null) throw new Exception("Checkout request is required.");
            if (request.sessionId <= 0) throw new Exception("Session Id is required.");
            if (request.bookingId <= 0) throw new Exception("Booking is required.");
            if (request.checkoutDt == DateTime.MinValue) throw new Exception("Checkout date is required.");

            if (request.paymentAmount < 0) throw new Exception("Payment amount cannot be negative.");

            // FIX: payment mode is only mandatory when money is actually being
            // collected (or billed to corporate). When the deposit covers the
            // whole bill there is nothing to collect, and staff were forced to
            // pick a random mode. The SP re-checks this against the real balance.
            bool hasMode = !string.IsNullOrWhiteSpace(request.paymentMode);

            if (request.paymentAmount > 0 && !hasMode)
                throw new Exception("Payment mode is required.");

            if (hasMode && !AllowedPaymentModes.Contains(request.paymentMode!))
                throw new Exception("Invalid payment mode.");

            if (request.paymentMode == "CORP_BILL" && request.paymentAmount > 0)
                throw new Exception("Payment amount must be 0 when billing to Corporate — the balance will be invoiced separately.");

            // FIX: RRN / UTR mandatory for card, UPI and online — was only
            // enforced on the screen.
            if (request.paymentAmount > 0 &&
                (request.paymentMode == "CARD" || request.paymentMode == "UPI" || request.paymentMode == "ONLINE") &&
                string.IsNullOrWhiteSpace(request.paymentReference))
            {
                throw new Exception(request.paymentMode == "ONLINE"
                    ? "UTR / reference number is required for online payment."
                    : "RRN number is required for card / UPI payment.");
            }

            // SP column is VARCHAR(50) — 100 used to pass here and then fail/truncate in SQL
            if (!string.IsNullOrWhiteSpace(request.paymentReference) && request.paymentReference.Trim().Length > 50)
                throw new Exception("Payment reference cannot exceed 50 characters.");

            if (!string.IsNullOrWhiteSpace(request.cardLast4) &&
                (request.cardLast4.Length != 4 || !request.cardLast4.All(char.IsDigit)))
                throw new Exception("Card last 4 digits must be exactly 4 numbers.");

            if (!string.IsNullOrWhiteSpace(request.refundMode) && !AllowedRefundModes.Contains(request.refundMode))
                throw new Exception("Invalid refund mode.");

            if (request.discountAmount < 0) throw new Exception("Discount amount cannot be negative.");
            if (request.discountAmount > 0 && string.IsNullOrWhiteSpace(request.discountReason))
                throw new Exception("Discount reason is required.");
            if (!string.IsNullOrWhiteSpace(request.discountReason) && request.discountReason.Length > 200)
                throw new Exception("Discount reason cannot exceed 200 characters.");

            if (request.penaltyAmount < 0) throw new Exception("Penalty amount cannot be negative.");
            if (request.penaltyAmount > 0 && string.IsNullOrWhiteSpace(request.penaltyReasonCode))
                throw new Exception("Penalty reason is required when a penalty amount is entered.");
            if (request.penaltyAmount > 0 && string.IsNullOrWhiteSpace(request.penaltyRemarks))
                throw new Exception("Penalty remarks are required when a penalty amount is entered.");
            if (!string.IsNullOrWhiteSpace(request.penaltyRemarks) && request.penaltyRemarks.Length > 400)
                throw new Exception("Penalty remarks cannot exceed 400 characters.");

            if (!request.roomInspected) throw new Exception("Room inspection must be completed.");
            if (!request.keyReturned) throw new Exception("Room key must be returned.");
            if (!request.signatureObtained) throw new Exception("Guest signature is required.");

            if (!string.IsNullOrWhiteSpace(request.remarks) && request.remarks.Length > 500)
                throw new Exception("Remarks cannot exceed 500 characters.");
        }
    }
}
