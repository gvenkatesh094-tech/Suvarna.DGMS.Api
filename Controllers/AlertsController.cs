using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Suvarna.DGMS.Api.Models;
using Suvarna.DGMS.Api.Services;

namespace Suvarna.DGMS.Api.Controllers
{
    /// <summary>
    /// Unified user alerts (28-Oct): header bell + Alerts screen.
    /// sessionId in the body is replaced with the checked X-Session-Id by SessionBindingFilter.
    /// </summary>
    [Authorize]
    [ApiController]
    [Route("api/alerts")]
    public class AlertsController : ControllerBase
    {
        private readonly AlertsService _service;

        public AlertsController(AlertsService service)
        {
            _service = service;
        }

        // ==========================================
        // LIST + SUMMARY (bell uses pageSize 8, Alerts screen uses filters + paging)
        // POST api/alerts/list
        // ==========================================

        [HttpPost("list")]
        public IActionResult List([FromBody] AlertListRequest req)
        {
            if (req == null || req.SessionId <= 0) return BadRequest("A valid session is required.");

            return Ok(_service.GetAlerts(req));
        }

        // ==========================================
        // READ / UNREAD / READ_ALL
        // POST api/alerts/read   { sessionId, action: "READ" | "UNREAD" | "READ_ALL", alertId?, routes?, category? }
        // ==========================================

        [HttpPost("read")]
        public IActionResult SetRead([FromBody] AlertReadRequest req)
        {
            if (req == null || req.SessionId <= 0) return BadRequest("A valid session is required.");

            return Ok(_service.SetRead(req));
        }
    }
}
