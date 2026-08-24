using Microsoft.AspNetCore.Mvc;
using Suvarna.DGMS.Api.Models;
using Suvarna.DGMS.Api.Services;

namespace Suvarna.DGMS.Api.Controllers
{
    [ApiController]
    [Route("api/hk")]
    public class HkDashboardController : ControllerBase
    {
        private readonly HkDashboardService _service;

        public HkDashboardController(HkDashboardService service)
        {
            _service = service;
        }

        // ==========================================
        // TASK LIST
        // ==========================================

        [HttpGet("tasks")]
        public IActionResult GetTasks(
            [FromQuery] string sessionId,
            [FromQuery] int? propertyId = null,
            [FromQuery] string? priority = null,
            [FromQuery] string? status = null,
            [FromQuery] bool slaBreachOnly = false)
        {
            return Ok(_service.GetTasks(sessionId, propertyId, priority, status, slaBreachOnly));
        }

        // ==========================================
        // CHECKLIST FOR A TASK (lazy-loaded on expand)
        // ==========================================

        [HttpGet("tasks/{taskId:int}/checklist")]
        public IActionResult GetTaskChecklist(int taskId, [FromQuery] string sessionId)
        {
            return Ok(_service.GetTaskChecklist(sessionId, taskId));
        }

        // ==========================================
        // ROOM STATUS GRID
        // ==========================================

        [HttpGet("rooms")]
        public IActionResult GetRoomGrid(
            [FromQuery] string sessionId,
            [FromQuery] int? propertyId = null,
            [FromQuery] short? floorSeq = null)
        {
            return Ok(_service.GetRoomGrid(sessionId, propertyId, floorSeq));
        }

        // ==========================================
        // STAFF ON SHIFT
        // ==========================================

        [HttpGet("staff")]
        public IActionResult GetStaffOnShift(
            [FromQuery] string sessionId,
            [FromQuery] int? propertyId = null)
        {
            return Ok(_service.GetStaffOnShift(sessionId, propertyId));
        }

        // ==========================================
        // ASSIGN / REASSIGN TASK
        // ==========================================

        [HttpPost("tasks/assign")]
        public IActionResult AssignTask([FromBody] AssignHkTaskRequest request)
        {
            return Ok(_service.AssignTask(request));
        }

        // ==========================================
        // TOGGLE CHECKLIST ITEM
        // ==========================================

        [HttpPost("tasks/checklist/toggle")]
        public IActionResult ToggleChecklistItem([FromBody] ToggleHkChecklistItemRequest request)
        {
            return Ok(_service.ToggleChecklistItem(request));
        }

        // ==========================================
        // MARK TASK COMPLETE
        // ==========================================

        [HttpPost("tasks/complete")]
        public IActionResult CompleteTask([FromBody] CompleteHkTaskRequest request)
        {
            return Ok(_service.CompleteTask(request));
        }

        // ==========================================
        // SUPERVISOR INSPECTION
        // ==========================================

        [HttpPost("tasks/inspect")]
        public IActionResult InspectTask([FromBody] InspectHkTaskRequest request)
        {
            return Ok(_service.InspectTask(request));
        }
        // ==========================================
        // GET MY TASKS
        // ==========================================

        [HttpGet("mytasks/{sessionId}")]
        public IActionResult GetMyTasks(string sessionId)
        {
            return Ok(
                _service.GetMyTasks(sessionId)
            );
        }
        // ==========================================
        // UPLOAD PHOTO
        // ==========================================

        [HttpPost("tasks/upload-photo")]
        public IActionResult UploadPhoto(
            [FromForm] string sessionId,
            [FromForm] int taskId,
            [FromForm] string? notes,
            IFormFile photo)
        {
            return Ok(
                _service.UploadPhoto(
                    sessionId,
                    taskId,
                    notes,
                    photo
                )
            );
        }
        [HttpGet("quickstats")]
        public IActionResult GetQuickStats([FromQuery] int sessionId, int locationId)
        {
            return Ok(_service.GetQuickStats(sessionId, locationId));
        }



        // ==========================================
        // SUPERVISOR ESCALATIONS
        // ==========================================

        [HttpGet("escalations")]
        public IActionResult GetEscalations([FromQuery] int sessionId)
        {
            return Ok(_service.GetEscalations(sessionId));
        }

        // ==========================================
        // MY NOTIFICATIONS
        // ==========================================

        [HttpGet("notifications")]
        public IActionResult GetMyNotifications([FromQuery] int sessionId)
        {
            return Ok(_service.GetMyNotifications(sessionId));
        }

        [HttpPost("notifications/read")]
        public IActionResult MarkNotificationRead([FromQuery] int sessionId, [FromQuery] int? notificationId = null)
        {
            return Ok(_service.MarkNotificationRead(sessionId, notificationId));
        }

    }
}
