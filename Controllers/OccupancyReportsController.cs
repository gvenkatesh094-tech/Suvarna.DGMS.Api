using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Suvarna.DGMS.Api.Services;

namespace Suvarna.DGMS.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/occupancyreports")]
    public class OccupancyReportsController : ControllerBase
    {
        private readonly OccupancyReportsService _service;

        public OccupancyReportsController(OccupancyReportsService service)
        {
            _service = service;
        }

        [HttpGet("kpi")]
        public IActionResult GetKpiSummary(
            [FromQuery] int sessionId,
            [FromQuery] int? propertyId = null)
        {
            if (sessionId <= 0) return BadRequest("A valid session is required.");
            return Ok(_service.GetKpiSummary(sessionId, propertyId));
        }

        [HttpGet("by-room-type")]
        public IActionResult GetByRoomType([FromQuery] int sessionId)
        {
            if (sessionId <= 0) return BadRequest("A valid session is required.");
            return Ok(_service.GetByRoomType(sessionId));
        }

        [HttpGet("category-breakdown")]
        public IActionResult GetCategoryBreakdown([FromQuery] int sessionId)
        {
            if (sessionId <= 0) return BadRequest("A valid session is required.");
            return Ok(_service.GetCategoryBreakdown(sessionId));
        }

        [HttpGet("trend")]
        public IActionResult GetTrend(
            [FromQuery] int sessionId,
            [FromQuery] int? days = 7)
        {
            if (sessionId <= 0) return BadRequest("A valid session is required.");
            return Ok(_service.GetTrend(sessionId, days));
        }

        [HttpGet("room-status-grid")]
        public IActionResult GetRoomStatusGrid(
            [FromQuery] int sessionId,
            [FromQuery] int? propertyId = null)
        {
            if (sessionId <= 0) return BadRequest("A valid session is required.");
            return Ok(_service.GetRoomStatusGrid(sessionId, propertyId));
        }
    }
}
