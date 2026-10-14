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
        // ==========================================
        // UPLOAD CHECKLIST ITEM PHOTO
        // NEW: backs the mandatory-photo-per-checklist-item flow on the
        // hk-tasks page. Was previously missing entirely — the frontend
        // (uploadChecklistItemPhoto in hk-dashboard.service.ts) already
        // called this route, so every photo upload was silently 404ing.
        // ==========================================

        [HttpPost("tasks/checklist/upload-photo")]
        public IActionResult UploadChecklistItemPhoto(
            [FromForm] string sessionId,
            [FromForm] int taskId,
            [FromForm] string itemCd,
            IFormFile photo)
        {
            return Ok(
                _service.UploadChecklistItemPhoto(
                    sessionId,
                    taskId,
                    itemCd,
                    photo
                )
            );
        }

        // ==========================================
        // NEW: TASK AUDIT TRAIL (points 2 & 6)
        // ==========================================

        [HttpGet("tasks/history")]
        public IActionResult GetTaskHistory([FromQuery] string sessionId, [FromQuery] int taskId)
        {
            return Ok(_service.GetTaskHistory(sessionId, taskId));
        }

        // ==========================================
        // NEW: REQUEST NEW TASK
        // ==========================================

        [HttpPost("tasks/request")]
        public IActionResult RequestNewTask([FromQuery] string sessionId)
        {
            return Ok(_service.RequestNewTask(sessionId));
        }

        [HttpGet("tasks/requests")]
        public IActionResult GetTaskRequests([FromQuery] string sessionId)
        {
            return Ok(_service.GetTaskRequests(sessionId));
        }

        [HttpPost("tasks/requests/dismiss")]
        public IActionResult DismissTaskRequest([FromQuery] string sessionId, [FromQuery] int requestId)
        {
            return Ok(_service.DismissTaskRequest(sessionId, requestId));
        }

        // ==========================================
        // NEW: CREATE MANUAL TASK (supervisor picks Room + Type + Priority)
        // ==========================================

        [HttpPost("tasks/create-manual")]
        public IActionResult CreateManualTask([FromBody] CreateManualHkTaskRequest request)
        {
            return Ok(_service.CreateManualTask(request));
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
