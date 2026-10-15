using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Suvarna.DGMS.Api.Models;
using Suvarna.DGMS.Api.Services;

namespace Suvarna.DGMS.Api.Controllers
{
    /// <summary>Security module. The session id comes from the X-Session-Id header (never from the URL/body); the SPs resolve user/org/location.</summary>
    [ApiController]
    [Authorize]
    [Route("api/security")]
    public class SecurityController : ControllerBase
    {
        private readonly SecurityService _s;
        public SecurityController(SecurityService s) { _s = s; }

        [HttpGet("lookups")] public IActionResult Lookups() => Ok(_s.Lookups());
        [HttpGet("hosts")] public IActionResult Hosts([FromQuery] string? search) => Ok(_s.Hosts(search));
        [HttpGet("kpis")] public IActionResult Kpis([FromQuery] DateTime fromUtc, [FromQuery] DateTime toUtc) => Ok(_s.Kpis(fromUtc, toUtc));

        [HttpPost("blacklist/check")] public IActionResult Check([FromBody] SecBlacklistCheckRequest q) => Ok(_s.CheckBlacklist(q));
        [HttpPost("passes")] public IActionResult Issue([FromBody] SecIssuePassRequest q) => Ok(_s.IssuePass(q));
        [HttpGet("passes")] public IActionResult Passes([FromQuery] string? status, [FromQuery] string? search, [FromQuery] DateTime? fromUtc, [FromQuery] DateTime? toUtc) => Ok(_s.Passes(status, search, fromUtc, toUtc));
        [HttpPost("passes/revoke")] public IActionResult Revoke([FromBody] SecRevokeRequest q) => Ok(_s.Revoke(q));

        [HttpPost("scan")] public IActionResult Scan([FromBody] SecScanRequest q) => Ok(_s.Scan(q, HttpContext.Connection.RemoteIpAddress?.ToString()));
        [HttpGet("entry-log")] public IActionResult Log([FromQuery] DateTime fromUtc, [FromQuery] DateTime toUtc, [FromQuery] string? result, [FromQuery] string? search, [FromQuery] int page = 1, [FromQuery] int pageSize = 50)
            => Ok(_s.EntryLog(fromUtc, toUtc, result, search, page, pageSize));

        [HttpGet("blacklist")] public IActionResult Blacklist([FromQuery] string? search, [FromQuery] bool includeExpired = false) => Ok(_s.Blacklist(search, includeExpired));
        [HttpPost("blacklist")] public IActionResult SaveBlacklist([FromBody] SecBlacklistSaveRequest q) => Ok(_s.SaveBlacklist(q));

        [HttpGet("incidents")] public IActionResult Incidents([FromQuery] string? status, [FromQuery] string? type, [FromQuery] DateTime? fromUtc, [FromQuery] DateTime? toUtc, [FromQuery] string? search) => Ok(_s.Incidents(status, type, fromUtc, toUtc, search));
        [HttpPost("incidents")] public IActionResult SaveIncident([FromBody] SecIncidentSaveRequest q) => Ok(_s.SaveIncident(q));
    }
}
