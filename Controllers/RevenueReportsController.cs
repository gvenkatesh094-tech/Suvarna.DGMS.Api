using Microsoft.AspNetCore.Mvc;
using Suvarna.DGMS.Api.Services;

namespace Suvarna.DGMS.Api.Controllers
{
    [ApiController]
    [Route("api/revenuereports")]
    public class RevenueReportsController : ControllerBase
    {
        private readonly RevenueReportsService _service;

        public RevenueReportsController(RevenueReportsService service)
        {
            _service = service;
        }

        // ==========================================
        // KPI SUMMARY
        // GET api/revenuereports/kpi?sessionId=1&propertyId=
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
        // DAILY REVENUE
        // GET api/revenuereports/daily?sessionId=1&fromDt=&toDt=&propertyId=
        // ==========================================

        [HttpGet("daily")]
        public IActionResult GetDailyRevenue(
            [FromQuery] int sessionId,
            [FromQuery] DateTime fromDt,
            [FromQuery] DateTime toDt,
            [FromQuery] int? propertyId = null)
        {
            if (sessionId <= 0) return BadRequest("A valid session is required.");

            return Ok(_service.GetDailyRevenue(sessionId, fromDt, toDt, propertyId));
        }

        // ==========================================
        // PAYMENT MODE SUMMARY
        // GET api/revenuereports/payment-modes?sessionId=1&fromDt=&toDt=
        // ==========================================

        [HttpGet("payment-modes")]
        public IActionResult GetPaymentModeSummary(
            [FromQuery] int sessionId,
            [FromQuery] DateTime fromDt,
            [FromQuery] DateTime toDt)
        {
            if (sessionId <= 0) return BadRequest("A valid session is required.");

            return Ok(_service.GetPaymentModeSummary(sessionId, fromDt, toDt));
        }
    }
}
