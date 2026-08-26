using Microsoft.AspNetCore.Mvc;
using Suvarna.DGMS.Api.Services;

namespace Suvarna.DGMS.Api.Controllers
{
    [ApiController]
    [Route("api/executivedashboard")]
    public class ExecutiveDashboardController : ControllerBase
    {
        private readonly ExecutiveDashboardService _service;

        public ExecutiveDashboardController(ExecutiveDashboardService service)
        {
            _service = service;
        }

        // ==========================================
        // KPI SUMMARY
        // GET api/executivedashboard/kpi?sessionId=1&propertyId=
        // ==========================================

        [HttpGet("kpi")]
        public IActionResult GetKpiSummary(
            [FromQuery] int sessionId,
            [FromQuery] int? propertyId = null)
        {
            if (sessionId <= 0) return BadRequest("A valid session is required.");

            return Ok(_service.GetKpiSummary(sessionId, propertyId));
        }

        // ==========================================
        // PROPERTY PERFORMANCE
        // GET api/executivedashboard/property-performance?sessionId=1
        // ==========================================

        [HttpGet("property-performance")]
        public IActionResult GetPropertyPerformance([FromQuery] int sessionId)
        {
            if (sessionId <= 0) return BadRequest("A valid session is required.");

            return Ok(_service.GetPropertyPerformance(sessionId));
        }

        // ==========================================
        // REVENUE TREND
        // GET api/executivedashboard/revenue-trend?sessionId=1&days=7
        // ==========================================

        [HttpGet("revenue-trend")]
        public IActionResult GetRevenueTrend(
            [FromQuery] int sessionId,
            [FromQuery] int? days = 7)
        {
            if (sessionId <= 0) return BadRequest("A valid session is required.");

            return Ok(_service.GetRevenueTrend(sessionId, days));
        }

        // ==========================================
        // GST SLAB SUMMARY
        // GET api/executivedashboard/gst-slab-summary?sessionId=1
        // ==========================================

        [HttpGet("gst-slab-summary")]
        public IActionResult GetGstSlabSummary([FromQuery] int sessionId)
        {
            if (sessionId <= 0) return BadRequest("A valid session is required.");

            return Ok(_service.GetGstSlabSummary(sessionId));
        }

        // ==========================================
        // ALERTS
        // GET api/executivedashboard/alerts?sessionId=1&topN=10
        // ==========================================

        [HttpGet("alerts")]
        public IActionResult GetAlerts(
            [FromQuery] int sessionId,
            [FromQuery] int? topN = 10)
        {
            if (sessionId <= 0) return BadRequest("A valid session is required.");

            return Ok(_service.GetAlerts(sessionId, topN));
        }

        // ==========================================
        // OPERATIONAL SNAPSHOT
        // GET api/executivedashboard/operational-snapshot?sessionId=1
        // ==========================================

        [HttpGet("operational-snapshot")]
        public IActionResult GetOperationalSnapshot([FromQuery] int sessionId)
        {
            if (sessionId <= 0) return BadRequest("A valid session is required.");

            return Ok(_service.GetOperationalSnapshot(sessionId));
        }
    }
}
