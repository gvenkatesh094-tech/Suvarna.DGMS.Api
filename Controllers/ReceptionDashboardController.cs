using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Suvarna.DGMS.Api.Models;
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
    
        // ==========================================
        // 03-Nov: ROOM POPUP
        // GET api/reception-dashboard/room-detail?sessionId=1&roomId=5
        // ==========================================
        [HttpGet("room-detail")]
        public IActionResult GetRoomDetail([FromQuery] int sessionId, [FromQuery] int roomId)
        {
            if (sessionId <= 0) return BadRequest("A valid session is required.");
            if (roomId <= 0) return BadRequest("A valid room is required.");
            return Ok(_service.GetRoomDetail(sessionId, roomId));
        }

        // GET api/reception-dashboard/today?sessionId=1
        [HttpGet("today")]
        public IActionResult GetToday([FromQuery] int sessionId)
        {
            if (sessionId <= 0) return BadRequest("A valid session is required.");
            return Ok(_service.GetToday(sessionId));
        }

        // GET api/reception-dashboard/search?sessionId=1&q=ravi
        [HttpGet("search")]
        public IActionResult Search([FromQuery] int sessionId, [FromQuery] string? q)
        {
            if (sessionId <= 0) return BadRequest("A valid session is required.");
            return Ok(_service.Search(sessionId, q));
        }

        // GET api/reception-dashboard/suggest-rooms?sessionId=1&bookingId=10
        [HttpGet("suggest-rooms")]
        public IActionResult SuggestRooms([FromQuery] int sessionId, [FromQuery] int bookingId)
        {
            if (sessionId <= 0) return BadRequest("A valid session is required.");
            if (bookingId <= 0) return BadRequest("A valid reservation is required.");
            return Ok(_service.SuggestRooms(sessionId, bookingId));
        }

        // POST api/reception-dashboard/hk-priority  { sessionId, roomId }
        [HttpPost("hk-priority")]
        public IActionResult SetHkPriority([FromBody] ReceptionRoomActionRequest request)
        {
            if (request == null || request.sessionId <= 0) return BadRequest("A valid session is required.");
            if (request.roomId <= 0) return BadRequest("A valid room is required.");
            return Ok(_service.SetHkPriority(request));
        }

        // GET api/reception-dashboard/shift-handover?sessionId=1
        [HttpGet("shift-handover")]
        public IActionResult GetShiftHandover([FromQuery] int sessionId)
        {
            if (sessionId <= 0) return BadRequest("A valid session is required.");
            return Ok(_service.GetShiftHandover(sessionId));
        }

        // POST api/reception-dashboard/shift-handover  { sessionId, notes, handedTo }
        [HttpPost("shift-handover")]
        public IActionResult SaveShiftHandover([FromBody] ShiftHandoverSaveRequest request)
        {
            if (request == null || request.sessionId <= 0) return BadRequest("A valid session is required.");
            return Ok(_service.SaveShiftHandover(request));
        }
    }
}
