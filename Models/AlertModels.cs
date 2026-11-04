namespace Suvarna.DGMS.Api.Models
{
    // =====================================================
    // Unified user alerts (28-Oct): header bell + Alerts screen
    // =====================================================

    public class AlertListRequest
    {
        public int SessionId { get; set; }

        /// <summary>Screens the user can open (menu rights), e.g. ["/reception","/checkout"]. Decides the shared alerts the user gets.</summary>
        public List<string>? Routes { get; set; }

        /// <summary>ALL / UNREAD / READ</summary>
        public string? Status { get; set; } = "ALL";

        /// <summary>ALL / OPEN (hide resolved)</summary>
        public string? Show { get; set; } = "ALL";

        /// <summary>FRONT_OFFICE / HOUSEKEEPING / BILLING / MAINTENANCE / SECURITY</summary>
        public string? Category { get; set; }

        /// <summary>CRITICAL / WARNING / INFO</summary>
        public string? Severity { get; set; }

        public string? Search { get; set; }

        /// <summary>yyyy-MM-dd (local date)</summary>
        public string? FromDate { get; set; }

        /// <summary>yyyy-MM-dd (local date)</summary>
        public string? ToDate { get; set; }

        public int PageNo { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }

    public class AlertReadRequest
    {
        public int SessionId { get; set; }

        /// <summary>READ / UNREAD / READ_ALL</summary>
        public string Action { get; set; } = "READ";

        public long? AlertId { get; set; }

        /// <summary>READ_ALL only: same screens as the list call</summary>
        public List<string>? Routes { get; set; }

        /// <summary>READ_ALL only: limit to one category</summary>
        public string? Category { get; set; }
    }

    public class UserAlertItem
    {
        public long AlertId { get; set; }
        public string AlertCode { get; set; } = "";
        public string Category { get; set; } = "";
        public string AlertName { get; set; } = "";
        public string Severity { get; set; } = "INFO";
        public string Title { get; set; } = "";
        public string? Message { get; set; }
        public string? RoomNo { get; set; }
        public string? GuestName { get; set; }
        public string? RefType { get; set; }
        public long? RefId { get; set; }
        public string? RefNo { get; set; }
        public int? BookingId { get; set; }
        public string? Route { get; set; }
        public bool IsPersonal { get; set; }
        public bool IsRead { get; set; }
        public DateTime? ReadDate { get; set; }
        public bool IsResolved { get; set; }
        public DateTime? ResolvedDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public string? ActorName { get; set; }
    }

    public class UserAlertSummary
    {
        public int Total { get; set; }
        public int Unread { get; set; }

        /// <summary>Unread and not resolved: the bell badge</summary>
        public int Attention { get; set; }

        public int Critical { get; set; }
        public int Warning { get; set; }
        public int Info { get; set; }

        public Dictionary<string, int> ByCategory { get; set; } = new();

        public DateTime? LatestDate { get; set; }
    }

    public class UserAlertList
    {
        public List<UserAlertItem> Items { get; set; } = new();
        public int TotalRows { get; set; }
        public int PageNo { get; set; }
        public int PageSize { get; set; }
        public UserAlertSummary Summary { get; set; } = new();
    }

    public class AlertReadResult
    {
        public int Updated { get; set; }
    }
}
