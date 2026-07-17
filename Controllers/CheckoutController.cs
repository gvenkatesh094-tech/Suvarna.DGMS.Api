using Microsoft.AspNetCore.Mvc;
using Suvarna.DGMS.Api.Models;
using Suvarna.DGMS.Api.Services;

namespace Suvarna.DGMS.Api.Controllers
{
    [ApiController]
    [Route("api/checkout")]
    public class CheckoutController : ControllerBase
    {
        private readonly CheckoutService _service;

        public CheckoutController(
            CheckoutService service)
        {
            _service = service;
        }

        //=========================================================
        // GET CHECKOUT BOOKINGS
        //=========================================================

        [HttpGet("bookings")]
        public IActionResult GetCheckoutBookings(
            [FromQuery] string sessionId,
            [FromQuery] int? propertyId = null,
            [FromQuery] string? bookingNo = null,
            [FromQuery] string? guestName = null,
            [FromQuery] string? mobileNo = null,
            [FromQuery] string? roomNo = null)
        {
            return Ok(
                _service.GetCheckoutBookings(
                    sessionId,
                    propertyId,
                    bookingNo,
                    guestName,
                    mobileNo,
                    roomNo
                )
            );
        }

        //=========================================================
        // GET CHECKOUT DETAILS
        //=========================================================

        [HttpGet("details")]
        public IActionResult GetCheckoutDetails(
            [FromQuery] string sessionId,
            [FromQuery] int bookingId)
        {
            return Ok(
                _service.GetCheckoutDetails(
                    sessionId,
                    bookingId
                )
            );
        }

        //=========================================================
        // CONFIRM CHECKOUT
        //=========================================================

        [HttpPost("confirm")]
        public IActionResult ConfirmCheckout(
            [FromBody] CheckoutRequest request)
        {
            return Ok(
                _service.ConfirmCheckout(
                    request
                )
            );
        }
    }
}