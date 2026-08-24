namespace Suvarna.DGMS.Api.Models
{
    public class LaundryLogRow
    {
        public int laundryLogId { get; set; }
        public string slipNo { get; set; } = "";
        public int bookingId { get; set; }
        public string bookingNo { get; set; } = "";
        public string roomNo { get; set; } = "";
        public string guestNm { get; set; } = "";
        public string laundryType { get; set; } = "";
        public int itemCount { get; set; }
        public string? itemDetailsJson { get; set; }   // raw JSON text from ITEM_DETAILS
        public DateTime? pickupDt { get; set; }
        public DateTime? deliveryDt { get; set; }
        public string chargePosted { get; set; } = "N";
        public int? chargeId { get; set; }
        public string? remarks { get; set; }
        public DateTime createDt { get; set; }
        public string status { get; set; } = "";   // derived: PENDING / IN_PROGRESS / DELIVERED
    }

    // Reuses the same shape as Guest Folio Billing's booking lookup —
    // active (CHECKEDIN) bookings, since laundry logs against BOOKING_ID.
    public class LaundryBookingLookup
    {
        public int bookingId { get; set; }
        public string bookingRef { get; set; } = "";
        public string guestNm { get; set; } = "";
        public int roomId { get; set; }
        public string roomNo { get; set; } = "";
        public int propertyId { get; set; }
    }

    public class LaundryPickupItem
    {
        public string name { get; set; } = "";
        public int qty { get; set; } = 1;
        public string? special { get; set; }
    }

    public class CreateLaundryLogRequest
    {
        public int sessionId { get; set; }
        public int bookingId { get; set; }
        public int roomId { get; set; }
        public string laundryType { get; set; } = "STANDARD";
        public string? remarks { get; set; }
        public List<LaundryPickupItem> items { get; set; } = new();
    }

    public class LaundryActionResult
    {
        public int laundryLogId { get; set; }
        public string slipNo { get; set; } = "";
        public string status { get; set; } = "";
        public string message { get; set; } = "";
    }

    public class CancelLaundryLogRequest
    {
        public int sessionId { get; set; }
        public int laundryLogId { get; set; }
        public string? reason { get; set; }
    }
}
