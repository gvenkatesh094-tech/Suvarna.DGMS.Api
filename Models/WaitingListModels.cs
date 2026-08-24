namespace Suvarna.DGMS.Api.Models
{
    public class WaitlistEntryRow
    {
        public int position { get; set; }
        public int waitlistId { get; set; }
        public string guestNm { get; set; } = "";
        public string mobileNo { get; set; } = "";
        public int occCatId { get; set; }
        public string occCatNm { get; set; } = "";
        public int? accmTypeId { get; set; }
        public string accmTypeNm { get; set; } = "";
        public string wardPreference { get; set; } = "";
        public DateTime requestDt { get; set; }
        public string status { get; set; } = "";
        // NEW: populated by the Housekeeping<->Waiting List integration.
        public int? matchedRoomId { get; set; }
        public string? matchedRoomNo { get; set; }
        public DateTime? matchedAt { get; set; }
    }

    public class AddWaitlistRequest
    {
        public int sessionId { get; set; }
        public string guestNm { get; set; } = "";
        public string mobileNo { get; set; } = "";
        public int occCatId { get; set; }
        public int? accmTypeId { get; set; }
        public string? wardPreference { get; set; }
    }

    public class WaitlistActionResult
    {
        public int waitlistId { get; set; }
        public string status { get; set; } = "";
        public string message { get; set; } = "";
    }
}


