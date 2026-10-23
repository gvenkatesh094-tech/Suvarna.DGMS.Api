using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Suvarna.DGMS.Api.Models;
using Suvarna.DGMS.Api.Services;

namespace Suvarna.DGMS.Api.Controllers
{
    [Authorize]
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

        // includeCheckedOut=true (NEW, Round B): guests checked out in the last 60 days are listed too
        [HttpGet("bookings")]
        public IActionResult GetBookings([FromQuery] int sessionId, [FromQuery] int? propertyId = null, [FromQuery] bool includeCheckedOut = false)
        {
            return Ok(_service.GetBookings(sessionId, propertyId, includeCheckedOut));
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

        // REMOVED (Round B): PUT charge ("Update Charge") - no screen used it and it allowed any amount / GST /
        // status to be written into a posted charge. A wrong charge is cancelled and posted again.

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

        // ==========================================
        // NEW (Round B): "Print Invoice" - GET api/guestfolio/invoice-print?sessionId=&bookingId= (or &invoiceId=)
        // ==========================================

        [HttpGet("invoice-print")]
        public IActionResult GetInvoicePrint([FromQuery] int sessionId, [FromQuery] int? bookingId = null, [FromQuery] int? invoiceId = null)
        {
            return Ok(_service.GetInvoicePrint(sessionId, bookingId, invoiceId));
        }
    }
}
