using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Suvarna.DGMS.Api.Models;
using Suvarna.DGMS.Api.Services;

namespace Suvarna.DGMS.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/gstreports")]
    public class GstReportsController : ControllerBase
    {
        private readonly GstReportsService _service;

        public GstReportsController(GstReportsService service)
        {
            _service = service;
        }

        // ==========================================
        // GST INVOICE REGISTER
        // GET api/gstreports?sessionId=1&fromDt=2026-08-01&toDt=2026-08-31&status=&slabPct=&returnType=
        // ==========================================

        [HttpGet]
        public IActionResult GetGstInvoices(
            [FromQuery] int sessionId,
            [FromQuery] DateTime fromDt,
            [FromQuery] DateTime toDt,
            [FromQuery] string? status = null,
            [FromQuery] decimal? slabPct = null,
            [FromQuery] string? returnType = null)
        {
            if (sessionId <= 0) return BadRequest("A valid session is required.");

            return Ok(_service.GetGstInvoices(sessionId, fromDt, toDt, status, slabPct, returnType));
        }

        // ==========================================
        // HSN / SAC SUMMARY
        // GET api/gstreports/hsn-summary?sessionId=1&fromDt=&toDt=
        // ==========================================

        [HttpGet("hsn-summary")]
        public IActionResult GetHsnSummary(
            [FromQuery] int sessionId,
            [FromQuery] DateTime fromDt,
            [FromQuery] DateTime toDt)
        {
            if (sessionId <= 0) return BadRequest("A valid session is required.");

            return Ok(_service.GetHsnSummary(sessionId, fromDt, toDt));
        }

        // ==========================================
        // MARK INVOICE PAID
        // POST api/gstreports/mark-paid
        // ==========================================

        [HttpPost("mark-paid")]
        public IActionResult MarkInvoicePaid([FromBody] MarkGstInvoicePaidRequest request)
        {
            if (request == null) return BadRequest("Request payload is required.");
            if (request.sessionId <= 0) return BadRequest("A valid session is required.");

            return Ok(_service.MarkInvoicePaid(request));
        }

        // ==========================================
        // CANCEL INVOICE
        // POST api/gstreports/cancel
        // ==========================================

        [HttpPost("cancel")]
        public IActionResult CancelInvoice([FromBody] CancelGstInvoiceRequest request)
        {
            if (request == null) return BadRequest("Request payload is required.");
            if (request.sessionId <= 0) return BadRequest("A valid session is required.");

            return Ok(_service.CancelInvoice(request));
        }

        // ==========================================
        // GSTR-1 — B2B
        // GET api/gstreports/gstr1/b2b?sessionId=1&fromDt=&toDt=
        // ==========================================

        [HttpGet("gstr1/b2b")]
        public IActionResult GetGstr1B2B(
            [FromQuery] int sessionId,
            [FromQuery] DateTime fromDt,
            [FromQuery] DateTime toDt)
        {
            if (sessionId <= 0) return BadRequest("A valid session is required.");

            return Ok(_service.GetGstr1B2B(sessionId, fromDt, toDt));
        }

        // ==========================================
        // GSTR-1 — B2C
        // GET api/gstreports/gstr1/b2c?sessionId=1&fromDt=&toDt=
        // ==========================================

        [HttpGet("gstr1/b2c")]
        public IActionResult GetGstr1B2C(
            [FromQuery] int sessionId,
            [FromQuery] DateTime fromDt,
            [FromQuery] DateTime toDt)
        {
            if (sessionId <= 0) return BadRequest("A valid session is required.");

            return Ok(_service.GetGstr1B2C(sessionId, fromDt, toDt));
        }

        // ==========================================
        // GSTR-3B SUMMARY
        // GET api/gstreports/gstr3b?sessionId=1&fromDt=&toDt=
        // ==========================================

        [HttpGet("gstr3b")]
        public IActionResult GetGstr3BSummary(
            [FromQuery] int sessionId,
            [FromQuery] DateTime fromDt,
            [FromQuery] DateTime toDt)
        {
            if (sessionId <= 0) return BadRequest("A valid session is required.");

            return Ok(_service.GetGstr3BSummary(sessionId, fromDt, toDt));
        }

        // ==========================================
        // COMPLIANCE STATUS
        // GET api/gstreports/compliance?sessionId=1&topN=6
        // ==========================================

        [HttpGet("compliance")]
        public IActionResult GetComplianceStatus(
            [FromQuery] int sessionId,
            [FromQuery] int topN = 6)
        {
            if (sessionId <= 0) return BadRequest("A valid session is required.");

            return Ok(_service.GetComplianceStatus(sessionId, topN));
        }

        // ==========================================
        // MARK RETURN AS FILED
        // POST api/gstreports/compliance/mark-filed
        // ==========================================

        [HttpPost("compliance/mark-filed")]
        public IActionResult MarkFiled([FromBody] MarkGstFiledRequest request)
        {
            if (request == null) return BadRequest("Request payload is required.");
            if (request.sessionId <= 0) return BadRequest("A valid session is required.");

            return Ok(_service.MarkFiled(request));
        }
    }
}
