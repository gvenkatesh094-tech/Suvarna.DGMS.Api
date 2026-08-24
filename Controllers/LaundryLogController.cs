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
            [FromQuery] string? type = null)
        {
            return Ok(_service.GetEntries(sessionId, status, type));
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
    }
}
