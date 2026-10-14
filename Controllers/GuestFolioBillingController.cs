using Microsoft.AspNetCore.Mvc;
using Suvarna.DGMS.Api.Models;
using Suvarna.DGMS.Api.Services;

namespace Suvarna.DGMS.Api.Controllers
{
    [ApiController]
    [Route("api/guestfolio")]
    public class GuestFolioBillingController : ControllerBase
    {
        private readonly GuestFolioBillingService _service;

        public GuestFolioBillingController(GuestFolioBillingService service)
        {
            _service = service;
        }

        // ==========================================
        // "Select Guest" dropdown
        // ==========================================

        [HttpGet("bookings")]
        public IActionResult GetBookings([FromQuery] int sessionId, [FromQuery] int? propertyId = null)
        {
            return Ok(_service.GetBookings(sessionId, propertyId));
        }

        // ==========================================
        // "Add Charge -> Service" dropdown
        // ==========================================

        [HttpGet("services")]
        public IActionResult GetServices([FromQuery] int sessionId, [FromQuery] int? propertyId = null)
        {
            return Ok(_service.GetServices(sessionId, propertyId));
        }

        // ==========================================
        // Full folio for the selected guest
        // ==========================================

        [HttpGet("{bookingId:int}")]
        public IActionResult GetFolio(int bookingId, [FromQuery] int sessionId)
        {
            return Ok(_service.GetFolio(sessionId, bookingId));
        }

        // ==========================================
        // "+ Add Charge" button
        // ==========================================

        [HttpPost("charge")]
        public IActionResult AddCharge([FromBody] AddChargeRequest request)
        {
            return Ok(_service.AddCharge(request));
        }

        // ==========================================
        // "Update Charge" button (edit drawer)
        // ==========================================

        [HttpPut("charge")]
        public IActionResult UpdateCharge([FromBody] UpdateChargeRequest request)
        {
            return Ok(_service.UpdateCharge(request));
        }

        // ==========================================
        // "Cancel" button on a charge line — NEW
        // Replaces the "Edit" action in the Charge Lines table for the
        // common wrong-charge case (e.g. Breakfast posted by mistake).
        // ==========================================

        [HttpPost("charge/{chargeId:int}/cancel")]
        public IActionResult CancelCharge(int chargeId, [FromBody] CancelChargeRequest request)
        {
            request.chargeId = chargeId;
            return Ok(_service.CancelCharge(request));
        }

        // ==========================================
        // "Collect Cash" / "Process Card" buttons
        // ==========================================

        [HttpPost("payment")]
        public IActionResult CollectPayment([FromBody] CollectPaymentRequest request)
        {
            return Ok(_service.CollectPayment(request));
        }

        // ==========================================
        // "Transfer Charges to IP" button
        // ==========================================

        [HttpPost("transfer-ip")]
        public IActionResult TransferToIp([FromBody] TransferToIpRequest request)
        {
            return Ok(_service.TransferToIp(request));
        }

        // ==========================================
        // "Generate Invoice" button
        // ==========================================

        [HttpPost("invoice")]
        public IActionResult GenerateInvoice([FromBody] GenerateInvoiceRequest request)
        {
            return Ok(_service.GenerateInvoice(request));
        }
    }
}
