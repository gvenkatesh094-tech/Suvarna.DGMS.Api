namespace Suvarna.DGMS.Api.Models
{
    public class HkEscalation
    {
        public int taskId { get; set; }
        public string taskNo { get; set; } = "";
        public string roomNo { get; set; } = "";
        public string roomName { get; set; } = "";
        public string taskType { get; set; } = "";
        public string status { get; set; } = "";
        public int? assignedUserId { get; set; }
        public string? assignedStaffName { get; set; }
        public int slaMinutes { get; set; }
        public DateTime triggeredDt { get; set; }
        public DateTime? escalatedDt { get; set; }
    }

    public class HkNotification
    {
        public int notificationId { get; set; }
        public int? taskId { get; set; }
        public string notificationType { get; set; } = "";
        public string message { get; set; } = "";
        public string isRead { get; set; } = "N";
        public DateTime createdDt { get; set; }
    }

    // NOTE: HkOperationResult already exists in your Models.HouseKeeping
    // namespace (used by HkDashboardRepository's RunWithOutputParams) —
    // NOT redeclared here to avoid a duplicate-class conflict. Reuse the
    // existing one for the new repository methods below.
}

