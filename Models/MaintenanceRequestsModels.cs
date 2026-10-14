using System;

namespace Suvarna.DGMS.Api.Models
{
    // ==========================================
    // GRID ROW
    // ==========================================
    public class MaintenanceRequestListItem
    {
        public int maintReqId { get; set; }
        public string maintNo { get; set; } = "";
        public int roomId { get; set; }
        public string roomNo { get; set; } = "";
        public string? roomName { get; set; }
        public string? roomStatus { get; set; }
        public int? bookingId { get; set; }

        public string category { get; set; } = "";
        public string description { get; set; } = "";
        public string priority { get; set; } = "";       // CRITICAL / HIGH / MEDIUM / LOW
        public string maintStatus { get; set; } = "";    // OPEN / ACKNOWLEDGED / IN_PROGRESS / RESOLVED / CLOSED

        public string raisedByType { get; set; } = "";
        public string? raisedByGuestName { get; set; }

        public int? assignedStaffId { get; set; }
        public string? assignedToName { get; set; }
        public string? assignedToMobile { get; set; }

        public string roomBlockRequired { get; set; } = "";
        public string roomBlocked { get; set; } = "";

        public DateTime raisedDt { get; set; }
        public DateTime? acknowledgedDt { get; set; }
        public DateTime? workStartedDt { get; set; }
        public DateTime? workCompletedDt { get; set; }
        public DateTime? closedDt { get; set; }
        public DateTime? slaTargetDt { get; set; }

        public string? qcResult { get; set; }
        public string? qcRemarks { get; set; }
        public string? resolutionRemarks { get; set; }
        public decimal costAmount { get; set; }
        public int reworkCount { get; set; }

        public bool isOverdue { get; set; }
    }

    // ==========================================
    // WORK LOG LINE (material/labour)
    // ==========================================
    public class MaintenanceWorkLogItem
    {
        public int detId { get; set; }
        public int lineNo { get; set; }
        public string workDesc { get; set; } = "";
        public string? materialUsed { get; set; }
        public decimal materialCostAmount { get; set; }
        public decimal? labourHrs { get; set; }
        public decimal labourCostAmount { get; set; }
        public decimal lineTotalAmount { get; set; }
        public string? photoUrl { get; set; }
        public string? remarks { get; set; }
        public DateTime createDt { get; set; }
    }

    // ==========================================
    // ACTIVITY LOG (NEW)
    // ==========================================
    public class MaintenanceActivityItem
    {
        public long logId { get; set; }
        public string actionCd { get; set; } = "";
        public string? fromStatus { get; set; }
        public string? toStatus { get; set; }
        public string? staffName { get; set; }
        public string? remarks { get; set; }
        public int actionBy { get; set; }
        public DateTime actionDt { get; set; }
    }

    // ==========================================
    // TECHNICIAN (NEW)
    // ==========================================
    public class MaintenanceStaffItem
    {
        public int staffId { get; set; }
        public string staffName { get; set; } = "";
        public string? mobileNo { get; set; }
        public string? skillCats { get; set; }
        public bool isActive { get; set; }
        public int openJobs { get; set; }
    }

    // ==========================================
    // ROOM LOOKUP ROW
    // ==========================================
    public class MaintenanceRoomOption
    {
        public int roomId { get; set; }
        public string roomNo { get; set; } = "";
        public string? roomName { get; set; }
        public string? roomStatus { get; set; }
        public string? accmTypeName { get; set; }
    }

    // ==========================================
    // RESULTS
    // ==========================================
    public class MaintenanceActionResult
    {
        public string status { get; set; } = "";
        public string message { get; set; } = "";
    }

    public class RaiseMaintRequestResult
    {
        public int maintReqId { get; set; }
        public string maintNo { get; set; } = "";
        public string status { get; set; } = "";
        public string message { get; set; } = "";
    }

    public class SaveMaintStaffResult
    {
        public int staffId { get; set; }
        public string status { get; set; } = "";
        public string message { get; set; } = "";
    }

    // ==========================================
    // REQUEST DTOs
    // ==========================================
    public class RaiseMaintRequestPayload
    {
        public int sessionId { get; set; }
        public int roomId { get; set; }
        public int? bookingId { get; set; }
        public string complaintCategory { get; set; } = "";
        public string description { get; set; } = "";
        public string? priority { get; set; }
        public string raisedByType { get; set; } = "STAFF";
        public string? raisedByName { get; set; }        // NEW
        public string blockRoom { get; set; } = "N";
    }

    public class AssignMaintRequestPayload             // NEW
    {
        public int sessionId { get; set; }
        public int maintReqId { get; set; }
        public int staffId { get; set; }
        public string? remarks { get; set; }
    }

    public class AcknowledgeMaintRequestPayload
    {
        public int sessionId { get; set; }
        public int maintReqId { get; set; }
    }

    public class StartMaintWorkPayload
    {
        public int sessionId { get; set; }
        public int maintReqId { get; set; }
        public int? assignedToUserId { get; set; }
        public string? assignedToName { get; set; }
    }

    public class ResolveMaintRequestPayload
    {
        public int sessionId { get; set; }
        public int maintReqId { get; set; }
        public string resolutionRemarks { get; set; } = "";
        public string? materialUsed { get; set; }
        public decimal? materialCost { get; set; }
        public decimal? labourHrs { get; set; }
        public decimal? labourCost { get; set; }
    }

    public class CloseMaintRequestPayload
    {
        public int sessionId { get; set; }
        public int maintReqId { get; set; }
    }

    public class SubmitMaintQcPayload
    {
        public int sessionId { get; set; }
        public int maintReqId { get; set; }
        public string qcResult { get; set; } = "";   // P / F
        public string? qcRemarks { get; set; }
    }

    public class SaveMaintStaffPayload                 // NEW
    {
        public int sessionId { get; set; }
        public int? staffId { get; set; }
        public string staffName { get; set; } = "";
        public string? mobileNo { get; set; }
        public string? skillCats { get; set; }
        public bool isActive { get; set; } = true;
    }
}
