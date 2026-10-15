using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Suvarna.DGMS.Api.Services;

namespace Suvarna.DGMS.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/reception-dashboard")]
    public class ReceptionDashboardController : ControllerBase
    {
        private readonly ReceptionDashboardService _service;

        public ReceptionDashboardController(ReceptionDashboardService service)
        {
            _service = service;
        }

        // ==========================================
        // SUMMARY
        // GET api/reception-dashboard/summary?sessionId=1
        // ==========================================

        [HttpGet("summary")]
        public IActionResult GetSummary([FromQuery] int sessionId)
        {
            if (sessionId <= 0) return BadRequest("A valid session is required.");

            return Ok(_service.GetSummary(sessionId));
        }

        // ==========================================
        // ACTIVE BOOKINGS
        // GET api/reception-dashboard/active-bookings?sessionId=1
        // ==========================================

        [HttpGet("active-bookings")]
        public IActionResult GetActiveBookings([FromQuery] int sessionId)
        {
            if (sessionId <= 0) return BadRequest("A valid session is required.");

            return Ok(_service.GetActiveBookings(sessionId));
        }

        // ==========================================
        // ROOM STATUS
        // GET api/reception-dashboard/room-status?sessionId=1
        // ==========================================

        [HttpGet("room-status")]
        public IActionResult GetRoomStatus([FromQuery] int sessionId)
        {
            if (sessionId <= 0) return BadRequest("A valid session is required.");

            return Ok(_service.GetRoomStatus(sessionId));
        }

        // ==========================================
        // ALERTS
        // GET api/reception-dashboard/alerts?sessionId=1
        // ==========================================

        [HttpGet("alerts")]
        public IActionResult GetAlerts([FromQuery] int sessionId)
        {
            if (sessionId <= 0) return BadRequest("A valid session is required.");

            return Ok(_service.GetAlerts(sessionId));
        }

        // ==========================================
        // REVENUE
        // GET api/reception-dashboard/revenue?sessionId=1
        // ==========================================

        [HttpGet("revenue")]
        public IActionResult GetRevenue([FromQuery] int sessionId)
        {
            if (sessionId <= 0) return BadRequest("A valid session is required.");

            return Ok(_service.GetRevenue(sessionId));
        }
    }
}
