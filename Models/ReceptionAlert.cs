namespace Suvarna.DGMS.Api.Models
{
    public class ReceptionAlert
    {
        public int alertId { get; set; }

        // NEW: stable key used by the UI for routing (e.g. CHECKOUT_DUE)
        public string alertCode { get; set; } = string.Empty;

        // NEW: CRITICAL / WARNING / INFO
        public string severity { get; set; } = string.Empty;

        // NEW: number of records behind the alert
        public int alertCount { get; set; }

        public string alertColor { get; set; } = string.Empty;
        public string alertTitle { get; set; } = string.Empty;
        public string alertSubtitle { get; set; } = string.Empty;
        public string actionText { get; set; } = string.Empty;
    }
}
