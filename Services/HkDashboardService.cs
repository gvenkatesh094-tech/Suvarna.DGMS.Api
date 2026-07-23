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
    }
}
