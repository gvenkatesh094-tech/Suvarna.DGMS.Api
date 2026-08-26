using Microsoft.AspNetCore.Mvc;
using Suvarna.DGMS.Api.Models;
using Suvarna.DGMS.Api.Services;

namespace Suvarna.DGMS.Api.Controllers
{
    [ApiController]
    [Route("api/maintenancerequests")]
    public class MaintenanceRequestsController : ControllerBase
    {
        private readonly MaintenanceRequestsService _service;

        public MaintenanceRequestsController(MaintenanceRequestsService service)
        {
            _service = service;
        }

        // ==========================================
        // GET MAINTENANCE REQUESTS (grid)
        // GET api/maintenancerequests?sessionId=1&priority=&status=&category=&roomId=&fromDt=&toDt=
        // ==========================================

        [HttpGet]
        public IActionResult GetMaintRequests(
            [FromQuery] int sessionId,
            [FromQuery] string? priority = null,
            [FromQuery] string? status = null,
            [FromQuery] string? category = null,
            [FromQuery] int? roomId = null,
            [FromQuery] DateTime? fromDt = null,
            [FromQuery] DateTime? toDt = null)
        {
            if (sessionId <= 0) return BadRequest("A valid session is required.");

            return Ok(_service.GetMaintRequests(sessionId, priority, status, category, roomId, fromDt, toDt));
        }

        // ==========================================
        // ROOM LOOKUP (for the Raise-Request modal)
        // GET api/maintenancerequests/rooms?sessionId=1
        // ==========================================

        [HttpGet("rooms")]
        public IActionResult GetRoomLookup([FromQuery] int sessionId)
        {
            if (sessionId <= 0) return BadRequest("A valid session is required.");

            return Ok(_service.GetRoomLookup(sessionId));
        }

        // ==========================================
        // RAISE REQUEST
        // POST api/maintenancerequests
        // ==========================================

        [HttpPost]
        public IActionResult RaiseRequest([FromBody] RaiseMaintRequestPayload payload)
        {
            if (payload == null) return BadRequest("Request payload is required.");
            if (payload.sessionId <= 0) return BadRequest("A valid session is required.");

            return Ok(_service.RaiseRequest(payload));
        }

        // ==========================================
        // ACKNOWLEDGE
        // POST api/maintenancerequests/acknowledge
        // ==========================================

        [HttpPost("acknowledge")]
        public IActionResult AcknowledgeRequest([FromBody] AcknowledgeMaintRequestPayload payload)
        {
            if (payload == null) return BadRequest("Request payload is required.");
            if (payload.sessionId <= 0) return BadRequest("A valid session is required.");

            return Ok(_service.AcknowledgeRequest(payload));
        }

        // ==========================================
        // START WORK
        // POST api/maintenancerequests/start-work
        // ==========================================

        [HttpPost("start-work")]
        public IActionResult StartWork([FromBody] StartMaintWorkPayload payload)
        {
            if (payload == null) return BadRequest("Request payload is required.");
            if (payload.sessionId <= 0) return BadRequest("A valid session is required.");

            return Ok(_service.StartWork(payload));
        }

        // ==========================================
        // RESOLVE
        // POST api/maintenancerequests/resolve
        // ==========================================

        [HttpPost("resolve")]
        public IActionResult ResolveRequest([FromBody] ResolveMaintRequestPayload payload)
        {
            if (payload == null) return BadRequest("Request payload is required.");
            if (payload.sessionId <= 0) return BadRequest("A valid session is required.");

            return Ok(_service.ResolveRequest(payload));
        }

        // ==========================================
        // CLOSE
        // POST api/maintenancerequests/close
        // ==========================================

        [HttpPost("close")]
        public IActionResult CloseRequest([FromBody] CloseMaintRequestPayload payload)
        {
            if (payload == null) return BadRequest("Request payload is required.");
            if (payload.sessionId <= 0) return BadRequest("A valid session is required.");

            return Ok(_service.CloseRequest(payload));
        }

        // ==========================================
        // SUBMIT QC
        // POST api/maintenancerequests/qc
        // ==========================================

        [HttpPost("qc")]
        public IActionResult SubmitQc([FromBody] SubmitMaintQcPayload payload)
        {
            if (payload == null) return BadRequest("Request payload is required.");
            if (payload.sessionId <= 0) return BadRequest("A valid session is required.");

            return Ok(_service.SubmitQc(payload));
        }

        // ==========================================
        // WORK LOG
        // GET api/maintenancerequests/worklog?sessionId=1&maintReqId=101
        // ==========================================

        [HttpGet("worklog")]
        public IActionResult GetWorkLog(
            [FromQuery] int sessionId,
            [FromQuery] int maintReqId)
        {
            if (sessionId <= 0) return BadRequest("A valid session is required.");

            return Ok(_service.GetWorkLog(sessionId, maintReqId));
        }
    }
}
