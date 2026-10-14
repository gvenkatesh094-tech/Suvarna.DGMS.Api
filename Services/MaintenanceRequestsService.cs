using Suvarna.DGMS.Api.Models;
using Suvarna.DGMS.Api.Repositories;
using System.Text.RegularExpressions;

namespace Suvarna.DGMS.Api.Services
{
    public class MaintenanceRequestsService
    {
        private static readonly string[] ValidPriorities = { "CRITICAL", "HIGH", "MEDIUM", "LOW" };
        private static readonly string[] ValidStatuses = { "OPEN", "ACKNOWLEDGED", "IN_PROGRESS", "RESOLVED", "CLOSED" };
        public static readonly string[] ValidCategories =
            { "PLUMBING", "ELECTRICAL", "FURNITURE", "INTERNET", "CIVIL", "AC", "APPLIANCE", "PEST", "SAFETY", "OTHER" };
        private static readonly string[] ValidRaisedByTypes = { "STAFF", "GUEST", "HK", "FRONT_DESK", "SYSTEM" };

        private readonly MaintenanceRequestsRepository _repo;

        public MaintenanceRequestsService(MaintenanceRequestsRepository repo)
        {
            _repo = repo;
        }

        // ==========================================
        // GRID
        // ==========================================

        public ApiResponse<List<MaintenanceRequestListItem>> GetMaintRequests(
            int sessionId, string? priority, string? status, string? category,
            int? roomId, DateTime? fromDt, DateTime? toDt, int? closedDays)
        {
            if (!string.IsNullOrWhiteSpace(priority) && !ValidPriorities.Contains(priority.Trim().ToUpperInvariant()))
                throw new Exception("Invalid priority filter.");
            if (!string.IsNullOrWhiteSpace(status) && !ValidStatuses.Contains(status.Trim().ToUpperInvariant()))
                throw new Exception("Invalid status filter.");
            if (!string.IsNullOrWhiteSpace(category) && !ValidCategories.Contains(category.Trim().ToUpperInvariant()))
                throw new Exception("Invalid category filter.");
            if (fromDt.HasValue && toDt.HasValue && fromDt.Value.Date > toDt.Value.Date)
                throw new Exception("From date cannot be after To date.");
            if (closedDays.HasValue && (closedDays.Value < 0 || closedDays.Value > 3650))
                throw new Exception("Invalid closed-days window.");

            var data = _repo.GetMaintRequests(sessionId, priority, status, category, roomId, fromDt, toDt, closedDays);

            return ApiResponse<List<MaintenanceRequestListItem>>.Success(
                data.Count == 0 ? "No maintenance requests found." : "Maintenance requests fetched successfully.", data);
        }

        public ApiResponse<List<MaintenanceRoomOption>> GetRoomLookup(int sessionId) =>
            ApiResponse<List<MaintenanceRoomOption>>.Success("Room lookup fetched successfully.", _repo.GetRoomLookup(sessionId));

        // ==========================================
        // RAISE
        // ==========================================

        public ApiResponse<RaiseMaintRequestResult> RaiseRequest(RaiseMaintRequestPayload p)
        {
            if (p == null) throw new Exception("Request payload is required.");
            if (p.roomId <= 0) throw new Exception("Room is required.");

            p.complaintCategory = (p.complaintCategory ?? "").Trim().ToUpperInvariant();
            if (!ValidCategories.Contains(p.complaintCategory)) throw new Exception("Select a valid category.");

            p.description = (p.description ?? "").Trim();
            if (p.description.Length < 5) throw new Exception("Description must be at least 5 characters.");
            if (p.description.Length > 2000) throw new Exception("Description cannot exceed 2000 characters.");

            if (!string.IsNullOrWhiteSpace(p.priority))
            {
                p.priority = p.priority.Trim().ToUpperInvariant();
                if (!ValidPriorities.Contains(p.priority)) throw new Exception("Invalid priority.");
            }

            p.raisedByType = string.IsNullOrWhiteSpace(p.raisedByType) ? "STAFF" : p.raisedByType.Trim().ToUpperInvariant();
            if (!ValidRaisedByTypes.Contains(p.raisedByType)) throw new Exception("Invalid 'raised by' type.");

            if (!string.IsNullOrWhiteSpace(p.raisedByName) && p.raisedByName.Trim().Length > 100)
                throw new Exception("Reporter name cannot exceed 100 characters.");

            p.blockRoom = (p.blockRoom ?? "N").Trim().ToUpperInvariant();
            if (p.blockRoom != "Y" && p.blockRoom != "N") throw new Exception("Block-room flag must be Y or N.");

            var data = _repo.RaiseRequest(p);
            if (data.status != "S")
                throw new Exception(string.IsNullOrWhiteSpace(data.message) ? "Unable to raise maintenance request." : data.message);

            return ApiResponse<RaiseMaintRequestResult>.Success(data.message, data);
        }

        // ==========================================
        // ASSIGN / REASSIGN (NEW)
        // ==========================================

        public ApiResponse<MaintenanceActionResult> AssignRequest(AssignMaintRequestPayload p)
        {
            if (p == null) throw new Exception("Request payload is required.");
            if (p.maintReqId <= 0) throw new Exception("A valid maintenance request is required.");
            if (p.staffId <= 0) throw new Exception("Select a technician.");
            if (!string.IsNullOrWhiteSpace(p.remarks) && p.remarks.Trim().Length > 500)
                throw new Exception("Remarks cannot exceed 500 characters.");

            return Done(_repo.AssignRequest(p), "Unable to assign request.");
        }

        // ==========================================
        // STATUS ACTIONS
        // ==========================================

        public ApiResponse<MaintenanceActionResult> AcknowledgeRequest(AcknowledgeMaintRequestPayload p)
        {
            RequireId(p?.maintReqId);
            return Done(_repo.AcknowledgeRequest(p!.sessionId, p.maintReqId), "Unable to acknowledge request.");
        }

        public ApiResponse<MaintenanceActionResult> StartWork(StartMaintWorkPayload p)
        {
            RequireId(p?.maintReqId);
            return Done(_repo.StartWork(p!), "Unable to start work.");
        }

        public ApiResponse<MaintenanceActionResult> ResolveRequest(ResolveMaintRequestPayload p)
        {
            RequireId(p?.maintReqId);

            p!.resolutionRemarks = (p.resolutionRemarks ?? "").Trim();
            if (p.resolutionRemarks.Length < 5) throw new Exception("Resolution notes must be at least 5 characters.");
            if (p.resolutionRemarks.Length > 4000) throw new Exception("Resolution notes cannot exceed 4000 characters.");
            if (!string.IsNullOrWhiteSpace(p.materialUsed) && p.materialUsed.Trim().Length > 200)
                throw new Exception("Material used cannot exceed 200 characters.");

            if (p.materialCost is < 0 or > 9_999_999) throw new Exception("Material cost must be between 0 and 99,99,999.");
            if (p.labourCost is < 0 or > 9_999_999) throw new Exception("Labour cost must be between 0 and 99,99,999.");
            if (p.labourHrs is < 0 or > 999) throw new Exception("Labour hours must be between 0 and 999.");

            return Done(_repo.ResolveRequest(p), "Unable to resolve request.");
        }

        public ApiResponse<MaintenanceActionResult> CloseRequest(CloseMaintRequestPayload p)
        {
            RequireId(p?.maintReqId);
            return Done(_repo.CloseRequest(p!.sessionId, p.maintReqId), "Unable to close request.");
        }

        public ApiResponse<MaintenanceActionResult> SubmitQc(SubmitMaintQcPayload p)
        {
            RequireId(p?.maintReqId);

            p!.qcResult = (p.qcResult ?? "").Trim().ToUpperInvariant();
            if (p.qcResult != "P" && p.qcResult != "F") throw new Exception("QC result must be Pass or Fail.");
            if (p.qcResult == "F" && (p.qcRemarks ?? "").Trim().Length < 5)
                throw new Exception("Explain what failed (min 5 characters).");
            if (!string.IsNullOrWhiteSpace(p.qcRemarks) && p.qcRemarks.Trim().Length > 1000)
                throw new Exception("QC remarks cannot exceed 1000 characters.");

            return Done(_repo.SubmitQc(p), "Unable to submit QC.");
        }

        // ==========================================
        // WORK LOG + ACTIVITY
        // ==========================================

        public ApiResponse<List<MaintenanceWorkLogItem>> GetWorkLog(int sessionId, int maintReqId)
        {
            RequireId(maintReqId);
            var data = _repo.GetWorkLog(sessionId, maintReqId);
            return ApiResponse<List<MaintenanceWorkLogItem>>.Success(
                data.Count == 0 ? "No work log entries yet." : "Work log fetched successfully.", data);
        }

        public ApiResponse<List<MaintenanceActivityItem>> GetActivityLog(int sessionId, int maintReqId)
        {
            RequireId(maintReqId);
            return ApiResponse<List<MaintenanceActivityItem>>.Success("Activity fetched successfully.",
                _repo.GetActivityLog(sessionId, maintReqId));
        }

        // ==========================================
        // TECHNICIANS (NEW)
        // ==========================================

        public ApiResponse<List<MaintenanceStaffItem>> GetStaff(int sessionId, bool includeInactive) =>
            ApiResponse<List<MaintenanceStaffItem>>.Success("Technicians fetched successfully.",
                _repo.GetStaff(sessionId, includeInactive));

        public ApiResponse<SaveMaintStaffResult> SaveStaff(SaveMaintStaffPayload p)
        {
            if (p == null) throw new Exception("Request payload is required.");

            p.staffName = (p.staffName ?? "").Trim();
            if (p.staffName.Length < 2 || p.staffName.Length > 100)
                throw new Exception("Technician name must be 2–100 characters.");

            if (!string.IsNullOrWhiteSpace(p.mobileNo) && !Regex.IsMatch(p.mobileNo.Trim(), @"^[6-9]\d{9}$"))
                throw new Exception("Enter a valid 10-digit mobile number.");

            if (!string.IsNullOrWhiteSpace(p.skillCats))
            {
                var cats = p.skillCats.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                                      .Select(c => c.ToUpperInvariant()).Distinct().ToList();
                if (cats.Any(c => !ValidCategories.Contains(c))) throw new Exception("Invalid skill category.");
                p.skillCats = string.Join(",", cats);
            }

            var data = _repo.SaveStaff(p);
            if (data.status != "S")
                throw new Exception(string.IsNullOrWhiteSpace(data.message) ? "Unable to save technician." : data.message);

            return ApiResponse<SaveMaintStaffResult>.Success(data.message, data);
        }

        // ==========================================
        // HELPERS
        // ==========================================

        private static void RequireId(int? id)
        {
            if (id is null or <= 0) throw new Exception("A valid maintenance request is required.");
        }

        private static ApiResponse<MaintenanceActionResult> Done(MaintenanceActionResult r, string fallback)
        {
            if (r.status != "S")
                throw new Exception(string.IsNullOrWhiteSpace(r.message) ? fallback : r.message);
            return ApiResponse<MaintenanceActionResult>.Success(r.message, r);
        }
    }
}
