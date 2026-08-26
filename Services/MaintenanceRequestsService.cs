using Suvarna.DGMS.Api.Models;
using Suvarna.DGMS.Api.Repositories;

namespace Suvarna.DGMS.Api.Services
{
    public class MaintenanceRequestsService
    {
        private static readonly string[] ValidPriorities = { "CRITICAL", "HIGH", "MEDIUM", "LOW" };
        private static readonly string[] ValidStatuses = { "OPEN", "ACKNOWLEDGED", "IN_PROGRESS", "RESOLVED", "CLOSED" };
        private static readonly string[] ValidCategories =
            { "PLUMBING", "ELECTRICAL", "FURNITURE", "INTERNET", "CIVIL", "AC", "APPLIANCE", "PEST", "SAFETY", "OTHER" };
        private static readonly string[] ValidRaisedByTypes = { "STAFF", "GUEST", "HK", "FRONT_DESK", "SYSTEM" };

        private readonly MaintenanceRequestsRepository _repo;

        public MaintenanceRequestsService(MaintenanceRequestsRepository repo)
        {
            _repo = repo;
        }

        // ==========================================
        // GET MAINTENANCE REQUESTS
        // ==========================================

        public ApiResponse<List<MaintenanceRequestListItem>> GetMaintRequests(
            int sessionId,
            string? priority,
            string? status,
            string? category,
            int? roomId,
            DateTime? fromDt,
            DateTime? toDt)
        {
            if (!string.IsNullOrWhiteSpace(priority) && !ValidPriorities.Contains(priority.Trim().ToUpperInvariant()))
                throw new Exception("Invalid priority filter.");

            if (!string.IsNullOrWhiteSpace(status) && !ValidStatuses.Contains(status.Trim().ToUpperInvariant()))
                throw new Exception("Invalid status filter.");

            if (fromDt.HasValue && toDt.HasValue && fromDt.Value.Date > toDt.Value.Date)
                throw new Exception("From date cannot be after To date.");

            var data = _repo.GetMaintRequests(sessionId, priority, status, category, roomId, fromDt, toDt);

            return ApiResponse<List<MaintenanceRequestListItem>>.Success(
                data.Count == 0 ? "No maintenance requests found for the selected filters." : "Maintenance requests fetched successfully.",
                data
            );
        }

        // ==========================================
        // ROOM LOOKUP
        // ==========================================

        public ApiResponse<List<MaintenanceRoomOption>> GetRoomLookup(int sessionId)
        {
            var data = _repo.GetRoomLookup(sessionId);
            return ApiResponse<List<MaintenanceRoomOption>>.Success("Room lookup fetched successfully.", data);
        }

        // ==========================================
        // RAISE REQUEST
        // ==========================================

        public ApiResponse<RaiseMaintRequestResult> RaiseRequest(RaiseMaintRequestPayload payload)
        {
            if (payload == null) throw new Exception("Request payload is required.");
            if (payload.roomId <= 0) throw new Exception("Room is required.");

            if (string.IsNullOrWhiteSpace(payload.complaintCategory))
                throw new Exception("Category is required.");
            if (!ValidCategories.Contains(payload.complaintCategory.Trim().ToUpperInvariant()))
                throw new Exception("Invalid category.");

            var desc = payload.description?.Trim() ?? "";
            if (desc.Length < 5)
                throw new Exception("Description must be at least 5 characters.");
            if (desc.Length > 2000)
                throw new Exception("Description cannot exceed 2000 characters.");
            payload.description = desc;

            if (!string.IsNullOrWhiteSpace(payload.priority) && !ValidPriorities.Contains(payload.priority.Trim().ToUpperInvariant()))
                throw new Exception("Invalid priority.");

            if (!string.IsNullOrWhiteSpace(payload.raisedByType) && !ValidRaisedByTypes.Contains(payload.raisedByType.Trim().ToUpperInvariant()))
                throw new Exception("Invalid 'raised by' type.");

            if (!string.IsNullOrWhiteSpace(payload.blockRoom) &&
                payload.blockRoom.Trim().ToUpperInvariant() != "Y" && payload.blockRoom.Trim().ToUpperInvariant() != "N")
                throw new Exception("Block-room flag must be Y or N.");

            var data = _repo.RaiseRequest(payload);

            if (data.status != "S")
                throw new Exception(string.IsNullOrWhiteSpace(data.message) ? "Unable to raise maintenance request." : data.message);

            return ApiResponse<RaiseMaintRequestResult>.Success(data.message, data);
        }

        // ==========================================
        // ACKNOWLEDGE
        // ==========================================

        public ApiResponse<MaintenanceActionResult> AcknowledgeRequest(AcknowledgeMaintRequestPayload payload)
        {
            if (payload == null) throw new Exception("Request payload is required.");
            if (payload.maintReqId <= 0) throw new Exception("A valid maintenance request is required.");

            var data = _repo.AcknowledgeRequest(payload.sessionId, payload.maintReqId);

            if (data.status != "S")
                throw new Exception(string.IsNullOrWhiteSpace(data.message) ? "Unable to acknowledge request." : data.message);

            return ApiResponse<MaintenanceActionResult>.Success(data.message, data);
        }

        // ==========================================
        // START WORK
        // ==========================================

        public ApiResponse<MaintenanceActionResult> StartWork(StartMaintWorkPayload payload)
        {
            if (payload == null) throw new Exception("Request payload is required.");
            if (payload.maintReqId <= 0) throw new Exception("A valid maintenance request is required.");

            if (!string.IsNullOrWhiteSpace(payload.assignedToName) && payload.assignedToName.Trim().Length > 100)
                throw new Exception("Assigned-to name cannot exceed 100 characters.");

            var data = _repo.StartWork(payload);

            if (data.status != "S")
                throw new Exception(string.IsNullOrWhiteSpace(data.message) ? "Unable to start work." : data.message);

            return ApiResponse<MaintenanceActionResult>.Success(data.message, data);
        }

        // ==========================================
        // RESOLVE
        // ==========================================

        public ApiResponse<MaintenanceActionResult> ResolveRequest(ResolveMaintRequestPayload payload)
        {
            if (payload == null) throw new Exception("Request payload is required.");
            if (payload.maintReqId <= 0) throw new Exception("A valid maintenance request is required.");

            var notes = payload.resolutionRemarks?.Trim() ?? "";
            if (notes.Length < 5)
                throw new Exception("Resolution notes must be at least 5 characters.");
            if (notes.Length > 4000)
                throw new Exception("Resolution notes cannot exceed 4000 characters.");
            payload.resolutionRemarks = notes;

            if (payload.materialCost.HasValue && payload.materialCost.Value < 0)
                throw new Exception("Material cost cannot be negative.");
            if (payload.labourCost.HasValue && payload.labourCost.Value < 0)
                throw new Exception("Labour cost cannot be negative.");
            if (payload.labourHrs.HasValue && (payload.labourHrs.Value < 0 || payload.labourHrs.Value > 999))
                throw new Exception("Labour hours must be between 0 and 999.");

            var data = _repo.ResolveRequest(payload);

            if (data.status != "S")
                throw new Exception(string.IsNullOrWhiteSpace(data.message) ? "Unable to resolve request." : data.message);

            return ApiResponse<MaintenanceActionResult>.Success(data.message, data);
        }

        // ==========================================
        // CLOSE
        // ==========================================

        public ApiResponse<MaintenanceActionResult> CloseRequest(CloseMaintRequestPayload payload)
        {
            if (payload == null) throw new Exception("Request payload is required.");
            if (payload.maintReqId <= 0) throw new Exception("A valid maintenance request is required.");

            var data = _repo.CloseRequest(payload.sessionId, payload.maintReqId);

            if (data.status != "S")
                throw new Exception(string.IsNullOrWhiteSpace(data.message) ? "Unable to close request." : data.message);

            return ApiResponse<MaintenanceActionResult>.Success(data.message, data);
        }

        // ==========================================
        // SUBMIT QC
        // ==========================================

        public ApiResponse<MaintenanceActionResult> SubmitQc(SubmitMaintQcPayload payload)
        {
            if (payload == null) throw new Exception("Request payload is required.");
            if (payload.maintReqId <= 0) throw new Exception("A valid maintenance request is required.");

            var result = payload.qcResult?.Trim().ToUpperInvariant() ?? "";
            if (result != "P" && result != "F")
                throw new Exception("QC result must be Pass or Fail.");
            payload.qcResult = result;

            if (!string.IsNullOrWhiteSpace(payload.qcRemarks) && payload.qcRemarks.Trim().Length > 1000)
                throw new Exception("QC remarks cannot exceed 1000 characters.");

            var data = _repo.SubmitQc(payload);

            if (data.status != "S")
                throw new Exception(string.IsNullOrWhiteSpace(data.message) ? "Unable to submit QC." : data.message);

            return ApiResponse<MaintenanceActionResult>.Success(data.message, data);
        }

        // ==========================================
        // WORK LOG
        // ==========================================

        public ApiResponse<List<MaintenanceWorkLogItem>> GetWorkLog(int sessionId, int maintReqId)
        {
            if (maintReqId <= 0) throw new Exception("A valid maintenance request is required.");

            var data = _repo.GetWorkLog(sessionId, maintReqId);

            return ApiResponse<List<MaintenanceWorkLogItem>>.Success(
                data.Count == 0 ? "No work log entries yet." : "Work log fetched successfully.",
                data
            );
        }
    }
}
