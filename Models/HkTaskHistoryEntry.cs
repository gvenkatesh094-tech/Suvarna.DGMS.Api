// NEW model for the task audit trail (points 2 & 6).
// Place this in the same folder/namespace as your other HK models
// (e.g. Models/HkTaskHistoryEntry.cs, namespace Suvarna.DGMS.Api.Models) —
// matches the lowercase-property convention already used by HkTask,
// HkChecklistItem, etc. in this codebase.

namespace Suvarna.DGMS.Api.Models
{
    public class HkTaskHistoryEntry
    {
        public int historyId { get; set; }
        public string actionType { get; set; } = "";
        public DateTime actionDt { get; set; }
        public string? actionByName { get; set; }
        public string? staffName { get; set; }
        public string? remarks { get; set; }
    }
}
