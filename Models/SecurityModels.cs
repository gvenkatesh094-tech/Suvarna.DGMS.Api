namespace Suvarna.DGMS.Api.Models
{
    // ---- lookups / dashboard ----
    public class SecLookupItem { public string category { get; set; } = ""; public string code { get; set; } = ""; public string name { get; set; } = ""; public int? extraInt { get; set; } public int sortOrder { get; set; } }
    public class SecHost { public int bookingId { get; set; } public string? bookingNo { get; set; } public int propertyId { get; set; } public int roomId { get; set; } public string? roomNo { get; set; } public string? guestName { get; set; } public string? mobileNo { get; set; } }
    public class SecKpis
    {
        public int activeOccupants { get; set; } public int activePasses { get; set; } public int expiredPasses { get; set; } public int insideNow { get; set; }
        public int blacklistCount { get; set; } public int entriesToday { get; set; } public int exitsToday { get; set; } public int deniedToday { get; set; } public int openIncidents { get; set; }
    }

    // ---- visitor pass ----
    public class SecBlacklistCheckRequest { public string idType { get; set; } = ""; public string idNo { get; set; } = ""; public string? name { get; set; } }
    public class SecBlacklistCheckResult { public bool found { get; set; } public string? matchType { get; set; } public string? fullName { get; set; } public string? reasonCode { get; set; } public string? blacklistNo { get; set; } }
    public class SecIssuePassRequest
    {
        public string passType { get; set; } = ""; public int? bookingId { get; set; } public string visitorName { get; set; } = ""; public string? gender { get; set; }
        public string idType { get; set; } = ""; public string idNo { get; set; } = ""; public string? mobile { get; set; }
        public DateTime validFromUtc { get; set; } public DateTime validToUtc { get; set; }
        public bool idDocStored { get; set; } public bool photoStored { get; set; } public string? photoUrl { get; set; }
        public bool consentId { get; set; } public bool consentVerify { get; set; } public string? remarks { get; set; }
    }
    public class SecIssuePassResult { public int passId { get; set; } public string? passNo { get; set; } public string? token { get; set; } public string? qrPayload { get; set; } }
    public class SecPass
    {
        public int passId { get; set; } public string passNo { get; set; } = ""; public string? passType { get; set; } public string? visitorName { get; set; } public string? mobile { get; set; }
        public string? idType { get; set; } public string? idMasked { get; set; } public int? bookingId { get; set; } public int? roomId { get; set; } public string? roomNo { get; set; }
        public DateTime? validFrom { get; set; } public DateTime? validTo { get; set; } public string? status { get; set; } public bool isInside { get; set; }
        public string? revokeReason { get; set; } public string? photoUrl { get; set; } public string? qrPayload { get; set; } public DateTime? createdOn { get; set; }
    }
    public class SecRevokeRequest { public int passId { get; set; } public string reason { get; set; } = ""; }

    // ---- scan / entry-exit log ----
    public class SecScanRequest { public string passNo { get; set; } = ""; public string? token { get; set; } public string gate { get; set; } = ""; public string? action { get; set; } public string? deviceId { get; set; } }
    public class SecScanResult { public bool allowed { get; set; } public string? resultCode { get; set; } public string? actionCode { get; set; } public string? message { get; set; } public string? passNo { get; set; } public string? visitorName { get; set; } public string? roomNo { get; set; } public DateTime? validTo { get; set; } }
    public class SecLogRow { public long logId { get; set; } public DateTime? logDt { get; set; } public string? passNo { get; set; } public string? visitorName { get; set; } public string? roomNo { get; set; } public string? gateCode { get; set; } public string? gateName { get; set; } public string? actionCode { get; set; } public string? scanResult { get; set; } public string? denyReason { get; set; } public string? deviceId { get; set; } }
    public class SecLogSummary { public int entries { get; set; } public int exits { get; set; } public int denied { get; set; } }
    public class SecLogPage { public SecLogSummary summary { get; set; } = new(); public List<SecLogRow> rows { get; set; } = new(); public int total { get; set; } public int page { get; set; } public int pageSize { get; set; } }

    // ---- blacklist ----
    public class SecBlacklistEntry
    {
        public int blacklistId { get; set; } public string? blacklistNo { get; set; } public string fullName { get; set; } = ""; public string? idType { get; set; } public string? idMasked { get; set; }
        public string? reasonCode { get; set; } public string? reasonName { get; set; } public string? reasonDesc { get; set; } public DateTime? effectiveFrom { get; set; } public DateTime? effectiveTo { get; set; }
        public bool isLegalHold { get; set; } public DateTime? lastReviewed { get; set; } public string? listStatus { get; set; }
    }
    public class SecBlacklistSaveRequest
    {
        public string mode { get; set; } = "I"; public int? id { get; set; } public string? fullName { get; set; } public string? idType { get; set; } public string? idNo { get; set; }
        public string? reasonCode { get; set; } public string? reasonDesc { get; set; } public DateTime? fromUtc { get; set; } public DateTime? toUtc { get; set; }
    }
    public class SecSaveResult { public int id { get; set; } public string? number { get; set; } }

    // ---- incidents ----
    public class SecIncident
    {
        public int incidentId { get; set; } public string incidentNo { get; set; } = ""; public string? incidentType { get; set; } public string? incidentTypeName { get; set; } public DateTime? incidentDt { get; set; }
        public int? roomId { get; set; } public string? roomNo { get; set; } public int? passId { get; set; } public string? gate { get; set; } public string? description { get; set; }
        public string? actionTaken { get; set; } public string? resolution { get; set; } public string? status { get; set; } public DateTime? closedDt { get; set; } public DateTime? createdOn { get; set; }
    }
    public class SecIncidentSaveRequest
    {
        public int? id { get; set; } public string type { get; set; } = ""; public DateTime? incidentUtc { get; set; } public int? roomId { get; set; } public string? gate { get; set; }
        public string description { get; set; } = ""; public string? actionTaken { get; set; } public string status { get; set; } = "OPEN"; public string? resolution { get; set; }
    }
}
