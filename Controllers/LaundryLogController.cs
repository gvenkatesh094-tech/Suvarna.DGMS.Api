using Microsoft.AspNetCore.Mvc;
using Suvarna.DGMS.Api.Models;
using Suvarna.DGMS.Api.Services;

namespace Suvarna.DGMS.Api.Controllers
{
    [ApiController]
    [Route("api/laundrylog")]
    public class LaundryLogController : ControllerBase
    {
        private readonly LaundryLogService _service;

        public LaundryLogController(LaundryLogService service)
        {
            _service = service;
        }

        [HttpGet]
        public IActionResult GetEntries(
            [FromQuery] int sessionId,
            [FromQuery] string? status = null,
            [FromQuery] string? type = null,
            [FromQuery] string? scope = "ACTIVE")       // ACTIVE | ALL
        {
            return Ok(_service.GetEntries(sessionId, status, type, scope));
        }

        [HttpGet("active-bookings")]
        public IActionResult GetActiveBookings([FromQuery] int sessionId)
        {
            return Ok(_service.GetActiveBookings(sessionId));
        }

        [HttpPost("log")]
        public IActionResult CreateLog([FromBody] CreateLaundryLogRequest request)
        {
            return Ok(_service.CreateLog(request));
        }

        [HttpPost("{laundryLogId:int}/pickup")]
        public IActionResult MarkPickedUp(int laundryLogId, [FromQuery] int sessionId)
        {
            return Ok(_service.MarkPickedUp(sessionId, laundryLogId));
        }

        [HttpPost("{laundryLogId:int}/deliver")]
        public IActionResult MarkDelivered(int laundryLogId, [FromQuery] int sessionId)
        {
            return Ok(_service.MarkDelivered(sessionId, laundryLogId));
        }

        [HttpPost("{laundryLogId:int}/cancel")]
        public IActionResult Cancel(int laundryLogId, [FromBody] CancelLaundryLogRequest request)
        {
            request.laundryLogId = laundryLogId;
            return Ok(_service.Cancel(request));
        }

        // "Reverse Charge" — un-bills a delivered entry's auto-posted
        // laundry charge (e.g. wrong item count / guest dispute) without
        // touching its Delivered status. This is the ONLY supported way
        // to remove a laundry charge — Guest Folio Billing's own Cancel
        // action refuses charges with CHARGE_SOURCE = 'LAUNDRY'.
        [HttpPost("{laundryLogId:int}/reverse-charge")]
        public IActionResult ReverseCharge(int laundryLogId, [FromBody] ReverseLaundryChargeRequest request)
        {
            return Ok(_service.ReverseCharge(request.sessionId, laundryLogId, request.reason));
        }

       

        // Amount to collect before returning laundry to a checked-out guest
        [HttpGet("{laundryLogId:int}/return-preview")]
        public IActionResult GetReturnPreview(int laundryLogId, [FromQuery] int sessionId)
        {
            return Ok(_service.GetReturnPreview(sessionId, laundryLogId));
        }

        // Return laundry + post charge + record payment, one transaction
        [HttpPost("{laundryLogId:int}/return-collect")]
        public IActionResult ReturnWithPayment(int laundryLogId, [FromBody] LaundryReturnRequest request)
        {
            request.laundryLogId = laundryLogId;
            return Ok(_service.ReturnWithPayment(request));
        }

    }
}
