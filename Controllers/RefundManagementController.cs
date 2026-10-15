using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Suvarna.DGMS.Api.Models;
using Suvarna.DGMS.Api.Services;

namespace Suvarna.DGMS.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/refundmanagement")]
    public class RefundManagementController : ControllerBase
    {
        private readonly RefundManagementService _service;

        public RefundManagementController(RefundManagementService service)
        {
            _service = service;
        }

        [HttpGet("eligible-bookings")]
        public IActionResult GetEligibleBookings([FromQuery] int sessionId)
        {
            return Ok(_service.GetEligibleBookings(sessionId));
        }

        [HttpPost("request")]
        public IActionResult SubmitRequest([FromBody] SubmitRefundRequest request)
        {
            return Ok(_service.SubmitRequest(request));
        }

        [HttpGet("requests")]
        public IActionResult GetRequests([FromQuery] int sessionId, [FromQuery] string? status = null)
        {
            return Ok(_service.GetRequests(sessionId, status));
        }

        [HttpPost("decide")]
        public IActionResult Decide([FromBody] DecideRefundRequest request)
        {
            return Ok(_service.Decide(request));
        }

        [HttpPost("{requestId:int}/complete")]
        public IActionResult Complete(int requestId, [FromQuery] int sessionId)
        {
            return Ok(_service.Complete(sessionId, requestId));
        }
    }
}
