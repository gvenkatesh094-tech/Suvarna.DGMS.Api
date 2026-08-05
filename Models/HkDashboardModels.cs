namespace Suvarna.DGMS.Api.Models
{
    // ==========================================
    // READ MODELS
    // ==========================================

    public class HkTask
    {
        public int taskId { get; set; }
        public string taskNo { get; set; } = "";
        public int roomId { get; set; }
        public string roomNo { get; set; } = "";
        public string roomName { get; set; } = "";
        
        public int? bookingId { get; set; }
        public string taskType { get; set; } = "";
        public string priority { get; set; } = "";
        public string status { get; set; } = "";
        public int? assignedUserId { get; set; }
        public string? assignedStaffName { get; set; }
        public int slaMinutes { get; set; }
        public DateTime triggeredDt { get; set; }
        public DateTime? startedAt { get; set; }
        public DateTime? completedAt { get; set; }
        public DateTime? inspectedAt { get; set; }
        public string slaBreachFlag { get; set; } = "N";
        public string inspectionRequired { get; set; } = "Y";
        public int failCount { get; set; }
        public string? remarks { get; set; }
        public int checklistTotal { get; set; }
        public int checklistDone { get; set; }
        public string? photoUrl { get; set; }
        public string? photoNotes { get; set; }
    }

    public class HkChecklistItem
    {
        public int itemId { get; set; }
        public int itemSeq { get; set; }
        public string itemCd { get; set; } = "";
        public string itemNm { get; set; } = "";
        public string isComplete { get; set; } = "N";
        public DateTime? completeDt { get; set; }
        public string? photoUrl { get; set; }
    }

    public class HkRoom
    {
        public int roomId { get; set; }
        public string roomNo { get; set; } = "";
        public string roomName { get; set; } = "";
        public string roomStatus { get; set; } = "";
        public int floorSeq { get; set; }
        public string floorName { get; set; } = "";
        public string accmTypeName { get; set; } = "";
        public string accmTypeCode { get; set; } = "";
    }

    public class HkStaff
    {
        public int userId { get; set; }
        public string staffName { get; set; } = "";
        public string initials { get; set; } = "";
        public string status { get; set; } = "";
        public int tasksAssigned { get; set; }
    }

    // ==========================================
    // REQUEST / RESULT MODELS
    // ==========================================

    public class AssignHkTaskRequest
    {
        public int sessionId { get; set; } 
        public int taskId { get; set; }
        public int staffUserId { get; set; }
        public string? priority { get; set; }
        public string? notes { get; set; }
    }

    public class ToggleHkChecklistItemRequest
    {
        public int sessionId { get; set; }  
        public int taskId { get; set; }
        public string itemCd { get; set; } = "";
        public bool isComplete { get; set; }
    }

    public class CompleteHkTaskRequest
    {
        public int sessionId { get; set; }  
        public int taskId { get; set; }
    }

    public class InspectHkTaskRequest
    {
        public int sessionId { get; set; }  
        public int taskId { get; set; }
        public string result { get; set; } = "P"; // P = Pass, F = Fail
        public string? failRemarks { get; set; }
        public decimal? overallScore { get; set; }
    }

    public class HkOperationResult
    {
        public string status { get; set; } = "";
        public string message { get; set; } = "";
        public string? photoUrl { get; set; }

        public string? photoNotes { get; set; }
    }
    public class GetMyHkTasksRequest
    {
        public int sessionId { get; set; } 
    }
}
