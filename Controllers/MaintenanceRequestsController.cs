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

        // GET api/maintenancerequests?sessionId=1&closedDays=7
        [HttpGet]
        public IActionResult GetMaintRequests(
            [FromQuery] int sessionId,
            [FromQuery] string? priority = null,
            [FromQuery] string? status = null,
            [FromQuery] string? category = null,
            [FromQuery] int? roomId = null,
            [FromQuery] DateTime? fromDt = null,
            [FromQuery] DateTime? toDt = null,
            [FromQuery] int? closedDays = null)
        {
            if (sessionId <= 0) return BadRequest("A valid session is required.");
            return Ok(_service.GetMaintRequests(sessionId, priority, status, category, roomId, fromDt, toDt, closedDays));
        }

        [HttpGet("rooms")]
        public IActionResult GetRoomLookup([FromQuery] int sessionId)
        {
            if (sessionId <= 0) return BadRequest("A valid session is required.");
            return Ok(_service.GetRoomLookup(sessionId));
        }

        [HttpPost]
        public IActionResult RaiseRequest([FromBody] RaiseMaintRequestPayload payload)
        {
            if (payload == null || payload.sessionId <= 0) return BadRequest("A valid session is required.");
            return Ok(_service.RaiseRequest(payload));
        }

        // NEW — assign or reassign a technician
        [HttpPost("assign")]
        public IActionResult AssignRequest([FromBody] AssignMaintRequestPayload payload)
        {
            if (payload == null || payload.sessionId <= 0) return BadRequest("A valid session is required.");
            return Ok(_service.AssignRequest(payload));
        }

        [HttpPost("acknowledge")]
        public IActionResult AcknowledgeRequest([FromBody] AcknowledgeMaintRequestPayload payload)
        {
            if (payload == null || payload.sessionId <= 0) return BadRequest("A valid session is required.");
            return Ok(_service.AcknowledgeRequest(payload));
        }

        [HttpPost("start-work")]
        public IActionResult StartWork([FromBody] StartMaintWorkPayload payload)
        {
            if (payload == null || payload.sessionId <= 0) return BadRequest("A valid session is required.");
            return Ok(_service.StartWork(payload));
        }

        [HttpPost("resolve")]
        public IActionResult ResolveRequest([FromBody] ResolveMaintRequestPayload payload)
        {
            if (payload == null || payload.sessionId <= 0) return BadRequest("A valid session is required.");
            return Ok(_service.ResolveRequest(payload));
        }

        [HttpPost("close")]
        public IActionResult CloseRequest([FromBody] CloseMaintRequestPayload payload)
        {
            if (payload == null || payload.sessionId <= 0) return BadRequest("A valid session is required.");
            return Ok(_service.CloseRequest(payload));
        }

        [HttpPost("qc")]
        public IActionResult SubmitQc([FromBody] SubmitMaintQcPayload payload)
        {
            if (payload == null || payload.sessionId <= 0) return BadRequest("A valid session is required.");
            return Ok(_service.SubmitQc(payload));
        }

        [HttpGet("worklog")]
        public IActionResult GetWorkLog([FromQuery] int sessionId, [FromQuery] int maintReqId)
        {
            if (sessionId <= 0) return BadRequest("A valid session is required.");
            return Ok(_service.GetWorkLog(sessionId, maintReqId));
        }

        // NEW — full activity history
        [HttpGet("activity")]
        public IActionResult GetActivityLog([FromQuery] int sessionId, [FromQuery] int maintReqId)
        {
            if (sessionId <= 0) return BadRequest("A valid session is required.");
            return Ok(_service.GetActivityLog(sessionId, maintReqId));
        }

        // NEW — technicians
        [HttpGet("staff")]
        public IActionResult GetStaff([FromQuery] int sessionId, [FromQuery] bool includeInactive = false)
        {
            if (sessionId <= 0) return BadRequest("A valid session is required.");
            return Ok(_service.GetStaff(sessionId, includeInactive));
        }

        [HttpPost("staff")]
        public IActionResult SaveStaff([FromBody] SaveMaintStaffPayload payload)
        {
            if (payload == null || payload.sessionId <= 0) return BadRequest("A valid session is required.");
            return Ok(_service.SaveStaff(payload));
        }
    }
}
