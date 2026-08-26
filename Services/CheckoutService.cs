using Suvarna.DGMS.Api.Models;
using Suvarna.DGMS.Api.Repositories;

namespace Suvarna.DGMS.Api.Services
{
    public class CheckoutService
    {
        private readonly CheckoutRepository _repo;

        public CheckoutService(CheckoutRepository repo)
        {
            _repo = repo;
        }

        //====================================================
        // GET CHECKOUT BOOKINGS
        // FIX: single global search term.
        //====================================================

        public ApiResponse<List<CheckoutBooking>> GetCheckoutBookings(
            int sessionId,
            string? searchTerm = null)
        {
            var data = _repo.GetCheckoutBookings(sessionId, searchTerm);

            return ApiResponse<List<CheckoutBooking>>.Success(
                "Checkout bookings fetched successfully.",
                data);
        }

        //====================================================
        // PREPARE CHECKOUT
        // FIX: no longer mutates any state — see CheckoutRepository.
        // Safe to call repeatedly (drawer open / cancel / reopen).
        //====================================================

        public ApiResponse<CheckoutDetails> PrepareCheckout(int sessionId, int bookingId, DateTime checkoutDt)
        {
            var data = _repo.PrepareCheckout(sessionId, bookingId, checkoutDt);

            if (data == null)
            {
                throw new Exception("Checkout details not found.");
            }

            return ApiResponse<CheckoutDetails>.Success(
                "Checkout details fetched successfully.",
                data);
        }

        //====================================================
        // GET CHECKOUT DETAILS
        // FIX: checkoutDt is now optional (defaults to "now" in the SP)
        // instead of being a silently-required parameter the frontend
        // never actually sent.
        //====================================================

        public ApiResponse<CheckoutDetails> GetCheckoutDetails(int sessionId, int bookingId, DateTime? checkoutDt)
        {
            var data = _repo.GetCheckoutDetails(sessionId, bookingId, checkoutDt);

            if (data == null)
            {
                throw new Exception("Checkout details not found.");
            }

            return ApiResponse<CheckoutDetails>.Success(
                "Checkout details fetched successfully.",
                data);
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
                throw new Exception(
                    string.IsNullOrWhiteSpace(data.message)
                    ? "Checkout failed."
                    : data.message);
            }

            return ApiResponse<CheckoutResult>.Success(data.message, data);
        }

        //====================================================
        // SERVER VALIDATION
        //====================================================

        private static void ValidateCheckout(CheckoutRequest request)
        {
            if (request == null)
            {
                throw new Exception("Checkout request is required.");
            }

            if (request.sessionId <= 0)
            {
                throw new Exception("Session Id is required.");
            }

            if (request.bookingId <= 0)
            {
                throw new Exception("Booking is required.");
            }

            if (request.checkoutDt == DateTime.MinValue)
            {
                throw new Exception("Checkout date is required.");
            }

            if (string.IsNullOrWhiteSpace(request.paymentMode))
            {
                throw new Exception("Payment mode is required.");
            }

            if (request.paymentAmount < 0)
            {
                throw new Exception("Payment amount cannot be negative.");
            }

            // FIX: "Bill to Corporate" should never carry a cash/card amount
            // collected today — the balance is invoiced later instead.
            if (request.paymentMode == "CORP_BILL" && request.paymentAmount > 0)
            {
                throw new Exception("Payment amount must be 0 when billing to Corporate — the balance will be invoiced separately.");
            }

            if (!string.IsNullOrWhiteSpace(request.paymentReference))
            {
                if (request.paymentReference.Length > 100)
                {
                    throw new Exception("Payment reference cannot exceed 100 characters.");
                }
            }

            if (request.discountAmount < 0)
            {
                throw new Exception("Discount amount cannot be negative.");
            }

            if (request.discountAmount > 0 && string.IsNullOrWhiteSpace(request.discountReason))
            {
                throw new Exception("Discount reason is required.");
            }

            if (!request.roomInspected)
            {
                throw new Exception("Room inspection must be completed.");
            }

            if (!request.keyReturned)
            {
                throw new Exception("Room key must be returned.");
            }

            if (!request.signatureObtained)
            {
                throw new Exception("Guest signature is required.");
            }

            if (!string.IsNullOrWhiteSpace(request.remarks))
            {
                if (request.remarks.Length > 500)
                {
                    throw new Exception("Remarks cannot exceed 500 characters.");
                }
            }
        }
    }
}
