using Suvarna.DGMS.Api.Models;
using Suvarna.DGMS.Api.Repositories;
using System.Text.RegularExpressions;

namespace Suvarna.DGMS.Api.Services
{
    public class CheckoutService
    {
        private readonly CheckoutRepository _repo;

        public CheckoutService(
            CheckoutRepository repo)
        {
            _repo = repo;
        }

        //====================================================
        // GET CHECKOUT BOOKINGS
        //====================================================

        public ApiResponse<List<CheckoutBooking>> GetCheckoutBookings(
    string sessionId)
        {
            var data = _repo.GetCheckoutBookings(sessionId);

            return ApiResponse<List<CheckoutBooking>>.Success(
                "Checkout bookings fetched successfully.",
                data);
        }

        //====================================================
        // GET CHECKOUT DETAILS
        //====================================================

        public ApiResponse<CheckoutDetails> GetCheckoutDetails(
            string sessionId,
            int bookingId)
        {
            var data =
                _repo.GetCheckoutDetails(
                    sessionId,
                    bookingId);

            if (data == null)
            {
                throw new Exception(
                    "Checkout details not found.");
            }

            return ApiResponse<CheckoutDetails>.Success(
                "Checkout details fetched successfully.",
                data);
        }

        //====================================================
        // CONFIRM CHECKOUT
        //====================================================

        public ApiResponse<CheckoutResult> ConfirmCheckout(
            CheckoutRequest request)
        {
            ValidateCheckout(request);

            var data =
                _repo.ConfirmCheckout(request);

            if (data.status != "S")
            {
                throw new Exception(
                    string.IsNullOrWhiteSpace(
                        data.message)
                    ? "Checkout failed."
                    : data.message);
            }

            return ApiResponse<CheckoutResult>.Success(
                data.message,
                data);
        }

        //====================================================
        // SERVER VALIDATION
        //====================================================

        private static void ValidateCheckout(
            CheckoutRequest request)
        {
            if (request == null)
            {
                throw new Exception(
                    "Checkout request is required.");
            }

            if (string.IsNullOrWhiteSpace(
                request.sessionId))
            {
                throw new Exception(
                    "Session Id is required.");
            }

            if (request.bookingId <= 0)
            {
                throw new Exception(
                    "Booking is required.");
            }

            if (request.checkoutDt == DateTime.MinValue)
            {
                throw new Exception(
                    "Checkout date is required.");
            }

            if (string.IsNullOrWhiteSpace(
                request.paymentMode))
            {
                throw new Exception(
                    "Payment mode is required.");
            }

            if (request.paymentAmount < 0)
            {
                throw new Exception(
                    "Payment amount cannot be negative.");
            }

            if (!string.IsNullOrWhiteSpace(
                request.paymentReference))
            {
                if (request.paymentReference.Length > 100)
                {
                    throw new Exception(
                        "Payment reference cannot exceed 100 characters.");
                }
            }

            if (request.discountAmount < 0)
            {
                throw new Exception(
                    "Discount amount cannot be negative.");
            }

            if (request.discountAmount > 0 &&
                string.IsNullOrWhiteSpace(
                    request.discountReason))
            {
                throw new Exception(
                    "Discount reason is required.");
            }

            if (!request.roomInspected)
            {
                throw new Exception(
                    "Room inspection must be completed.");
            }

            if (!request.keyReturned)
            {
                throw new Exception(
                    "Room key must be returned.");
            }

            if (!request.signatureObtained)
            {
                throw new Exception(
                    "Guest signature is required.");
            }

            if (!string.IsNullOrWhiteSpace(
                request.remarks))
            {
                if (request.remarks.Length > 500)
                {
                    throw new Exception(
                        "Remarks cannot exceed 500 characters.");
                }
            }
        }
    }
}