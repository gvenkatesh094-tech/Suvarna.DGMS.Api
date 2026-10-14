using DGMS.API.Models.HouseKeeping;
using Microsoft.AspNetCore.Http;
using Suvarna.DGMS.Api.Models;
using Suvarna.DGMS.Api.Repositories;

namespace Suvarna.DGMS.Api.Services
{
    public class HkDashboardService
    {
        private readonly HkDashboardRepository _repo;

        public HkDashboardService(HkDashboardRepository repo)
        {
            _repo = repo;
        }

        // ==========================================
        // GET TASKS
        // ==========================================

        public ApiResponse<List<HkTask>> GetTasks(
            string sessionId,
            int? propertyId,
            string? priority,
            string? status,
            bool slaBreachOnly)
        {
            var data = _repo.GetTasks(sessionId, propertyId, priority, status, slaBreachOnly);

            return ApiResponse<List<HkTask>>.Success(
                $"{data.Count} task(s) fetched successfully.",
                data
            );
        }

        // ==========================================
        // GET CHECKLIST FOR A TASK
        // ==========================================

        public ApiResponse<List<HkChecklistItem>> GetTaskChecklist(string sessionId, int taskId)
        {
            var data = _repo.GetTaskChecklist(sessionId, taskId);

            return ApiResponse<List<HkChecklistItem>>.Success(
                "Checklist fetched successfully.",
                data
            );
        }

        // ==========================================
        // GET ROOM GRID
        // ==========================================

        public ApiResponse<List<HkRoom>> GetRoomGrid(string sessionId, int? propertyId, short? floorSeq)
        {
            var data = _repo.GetRoomGrid(sessionId, propertyId, floorSeq);

            return ApiResponse<List<HkRoom>>.Success(
                "Room grid fetched successfully.",
                data
            );
        }

        // ==========================================
        // GET STAFF ON SHIFT
        // ==========================================

        public ApiResponse<List<HkStaff>> GetStaffOnShift(string sessionId, int? propertyId)
        {
            var data = _repo.GetStaffOnShift(sessionId, propertyId);

            return ApiResponse<List<HkStaff>>.Success(
                "Staff roster fetched successfully.",
                data
            );
        }

        // ==========================================
        // ASSIGN TASK
        // ==========================================

        public ApiResponse<HkOperationResult> AssignTask(AssignHkTaskRequest request)
        {
            var data = _repo.AssignTask(request);

            if (data.status != "S")
            {
                throw new Exception(string.IsNullOrWhiteSpace(data.message) ? "Failed to assign task." : data.message);
            }

            return ApiResponse<HkOperationResult>.Success(data.message, data);
        }

        // ==========================================
        // TOGGLE CHECKLIST ITEM
        // ==========================================

        public ApiResponse<HkOperationResult> ToggleChecklistItem(ToggleHkChecklistItemRequest request)
        {
            var data = _repo.ToggleChecklistItem(request);

            if (data.status != "S")
            {
                throw new Exception(string.IsNullOrWhiteSpace(data.message) ? "Failed to update checklist item." : data.message);
            }

            return ApiResponse<HkOperationResult>.Success(data.message, data);
        }

        // ==========================================
        // COMPLETE TASK
        // ==========================================

        public ApiResponse<HkOperationResult> CompleteTask(CompleteHkTaskRequest request)
        {
            var data = _repo.CompleteTask(request);

            if (data.status != "S")
            {
                throw new Exception(string.IsNullOrWhiteSpace(data.message) ? "Failed to complete task." : data.message);
            }

            return ApiResponse<HkOperationResult>.Success(data.message, data);
        }

        // ==========================================
        // INSPECT TASK
        // ==========================================

        public ApiResponse<HkOperationResult> InspectTask(InspectHkTaskRequest request)
        {
            var data = _repo.InspectTask(request);

            if (data.status != "S")
            {
                throw new Exception(string.IsNullOrWhiteSpace(data.message) ? "Failed to record inspection." : data.message);
            }

            return ApiResponse<HkOperationResult>.Success(data.message, data);
        }
        // ==========================================
        // GET MY TASKS
        // ==========================================

        public ApiResponse<List<HkTask>> GetMyTasks(string sessionId)
        {
            var data = _repo.GetMyTasks(sessionId);

            return ApiResponse<List<HkTask>>.Success(
                $"{data.Count} task(s) fetched successfully.",
                data
            );
        }
        public ApiResponse<HkOperationResult> UploadPhoto(string sessionId, int taskId, string? notes, IFormFile photo)
        {
            var data = _repo.UploadPhoto(
                sessionId,
                taskId,
                notes,
                photo
            );

            if (data.status != "S")
            {
                throw new Exception(data.message);
            }

            return ApiResponse<HkOperationResult>.Success(
                data.message,
                data
            );
        }
        // ==========================================
        // NEW: UPLOAD CHECKLIST ITEM PHOTO
        // ==========================================

        public ApiResponse<HkOperationResult> UploadChecklistItemPhoto(string sessionId, int taskId, string itemCd, IFormFile photo)
        {
            var data = _repo.UploadChecklistItemPhoto(
                sessionId,
                taskId,
                itemCd,
                photo
            );

            if (data.status != "S")
            {
                throw new Exception(string.IsNullOrWhiteSpace(data.message) ? "Failed to upload checklist item photo." : data.message);
            }

            return ApiResponse<HkOperationResult>.Success(
                data.message,
                data
            );
        }

        // ==========================================
        // NEW: TASK AUDIT TRAIL
        // ==========================================

        public ApiResponse<List<HkTaskHistoryEntry>> GetTaskHistory(string sessionId, int taskId)
        {
            var data = _repo.GetTaskHistory(sessionId, taskId);

            return ApiResponse<List<HkTaskHistoryEntry>>.Success(
                $"{data.Count} history record(s) fetched successfully.",
                data
            );
        }

        // ==========================================
        // NEW: REQUEST NEW TASK
        // ==========================================

        public ApiResponse<HkOperationResult> RequestNewTask(string sessionId)
        {
            var data = _repo.RequestNewTask(sessionId);

            if (data.status != "S")
            {
                throw new Exception(string.IsNullOrWhiteSpace(data.message) ? "Failed to submit request." : data.message);
            }

            return ApiResponse<HkOperationResult>.Success(data.message, data);
        }

        public ApiResponse<List<HkTaskRequest>> GetTaskRequests(string sessionId)
        {
            var data = _repo.GetTaskRequests(sessionId);

            return ApiResponse<List<HkTaskRequest>>.Success(
                $"{data.Count} request(s) fetched successfully.",
                data
            );
        }

        public ApiResponse<HkOperationResult> DismissTaskRequest(string sessionId, int requestId)
        {
            var data = _repo.DismissTaskRequest(sessionId, requestId);

            if (data.status != "S")
            {
                throw new Exception(string.IsNullOrWhiteSpace(data.message) ? "Failed to dismiss request." : data.message);
            }

            return ApiResponse<HkOperationResult>.Success(data.message, data);
        }

        // ==========================================
        // NEW: CREATE MANUAL TASK
        // ==========================================

        public ApiResponse<HkOperationResult> CreateManualTask(CreateManualHkTaskRequest request)
        {
            var data = _repo.CreateManualTask(request);

            if (data.status != "S")
            {
                throw new Exception(string.IsNullOrWhiteSpace(data.message) ? "Failed to create task." : data.message);
            }

            return ApiResponse<HkOperationResult>.Success(data.message, data);
        }

        public ApiResponse<HkQuickStats> GetQuickStats(int sessionId, int locationId)
        {
            var data = _repo.GetQuickStats(sessionId, locationId);

            return ApiResponse<HkQuickStats>.Success(
                "Quick stats fetched successfully.",
                data
            );
        }




        public ApiResponse<List<HkEscalation>> GetEscalations(int sessionId)
        {
            var data = _repo.GetEscalations(sessionId);
            return ApiResponse<List<HkEscalation>>.Success(
                $"{data.Count} escalation(s) fetched successfully.", data);
        }

        public ApiResponse<List<HkNotification>> GetMyNotifications(int sessionId)
        {
            var data = _repo.GetMyNotifications(sessionId);
            return ApiResponse<List<HkNotification>>.Success(
                $"{data.Count} notification(s) fetched successfully.", data);
        }

        public ApiResponse<HkOperationResult> MarkNotificationRead(int sessionId, int? notificationId)
        {
            var data = _repo.MarkNotificationRead(sessionId, notificationId);

            if (data.status != "S")
            {
                throw new Exception(string.IsNullOrWhiteSpace(data.message) ? "Failed to mark notification read." : data.message);
            }

            return ApiResponse<HkOperationResult>.Success(data.message, data);
        }

    }
}
