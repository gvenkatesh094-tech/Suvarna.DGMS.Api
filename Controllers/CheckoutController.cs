using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Suvarna.DGMS.Api.Models;
using Suvarna.DGMS.Api.Services;

namespace Suvarna.DGMS.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/checkout")]
    public class CheckoutController : ControllerBase
    {
        private readonly CheckoutService _service;

        public CheckoutController(CheckoutService service)
        {
            _service = service;
        }

        //=========================================================
        // GET CHECKOUT BOOKINGS
        // FIX: single global search term, matching SP + frontend.
        //=========================================================

        [HttpGet("bookings")]
        public IActionResult GetCheckoutBookings(
            [FromQuery] int sessionId,
            [FromQuery] string? searchTerm = null)
        {
            return Ok(_service.GetCheckoutBookings(sessionId, searchTerm));
        }

        //=========================================================
        // GET CHECKOUT DETAILS
        // FIX: checkoutDt is now optional. Previously required with no
        // default, but the frontend never sent it — every call to this
        // endpoint would 400. The SP itself defaults to "now" if omitted.
        //=========================================================

        [HttpGet("details")]
        public IActionResult GetCheckoutDetails(
            [FromQuery] int sessionId,
            [FromQuery] int bookingId,
            [FromQuery] DateTime? checkoutDt = null)
        {
            return Ok(_service.GetCheckoutDetails(sessionId, bookingId, checkoutDt));
        }

        //=========================================================
        // PREPARE CHECKOUT
        // FIX: no longer mutates state — safe to call repeatedly.
        //=========================================================

        [HttpPost("prepare")]
        public IActionResult PrepareCheckout(
            [FromQuery] int sessionId,
            [FromQuery] int bookingId,
            [FromQuery] DateTime checkoutDt)
        {
            return Ok(_service.PrepareCheckout(sessionId, bookingId, checkoutDt));
        }

        //=========================================================
        // NEW (28-Oct): did the room go to housekeeping, room status,
        // HK task. bookingId = one booking (receipt); none = today's check-outs.
        //=========================================================

        [HttpGet("hk-status")]
        public IActionResult GetCheckoutHkStatus(
            [FromQuery] int sessionId,
            [FromQuery] int? bookingId = null)
        {
            return Ok(_service.GetCheckoutHkStatus(sessionId, bookingId));
        }

        //=========================================================
        // CONFIRM CHECKOUT
        //=========================================================

        [HttpPost("confirm")]
        public IActionResult ConfirmCheckout([FromBody] CheckoutRequest request)
        {
            return Ok(_service.ConfirmCheckout(request));
        }
    }
}
