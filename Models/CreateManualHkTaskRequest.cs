// NEW model for manual task creation from hk-dashboard (Room + Type +
// Priority, not tied to a checkout). Same lowercase-property convention
// as AssignHkTaskRequest etc. in this codebase.

namespace Suvarna.DGMS.Api.Models
{
    public class CreateManualHkTaskRequest
    {
        public int sessionId { get; set; }
        public int roomId { get; set; }
        public string taskType { get; set; } = "";
        public string? priority { get; set; }
    }
}
