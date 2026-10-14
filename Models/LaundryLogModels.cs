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
        public string status { get; set; } = "";   // derived: PENDING / IN_PROGRESS / DELIVERED / CANCELLED
        public string bookingStatus { get; set; } = "";   // CHECKEDIN / CHECKEDOUT
        public DateTime? checkoutDt { get; set; }          // guest's actual checkout time

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
        public string roomName { get; set; } = "";
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

    // NEW — used by LaundryLogController.ReverseCharge / LaundryLogService.ReverseCharge
    // to un-bill a delivered entry's auto-posted laundry charge. Reuses
    // LaundryActionResult as its return type (already defined above),
    // so no separate result class is needed.
    public class ReverseLaundryChargeRequest
    {
        public int sessionId { get; set; }
        public string? reason { get; set; }
    }

    
    public class LaundryReturnPreview
    {
        public int laundryLogId { get; set; }
        public string slipNo { get; set; } = "";
        public string laundryType { get; set; } = "";
        public int itemCount { get; set; }
        public string bookingNo { get; set; } = "";
        public string bookingStatus { get; set; } = "";
        public DateTime? checkoutDt { get; set; }
        public string roomNo { get; set; } = "";
        public string guestNm { get; set; } = "";
        public string billNo { get; set; } = "";
        public bool priceConfigured { get; set; }
        public string serviceNm { get; set; } = "";
        public decimal unitRate { get; set; }
        public decimal gstPct { get; set; }
        public decimal baseAmt { get; set; }
        public decimal cgstAmt { get; set; }
        public decimal sgstAmt { get; set; }
        public decimal grossAmt { get; set; }
    }

    public class LaundryReturnRequest
    {
        public int sessionId { get; set; }
        public int laundryLogId { get; set; }
        public string? paymentMode { get; set; }
        public decimal paymentAmount { get; set; }
        public string? paymentReference { get; set; }
        public string? cardLast4 { get; set; }
        public string? upiVpa { get; set; }
        public string? paymentRemarks { get; set; }
    }

    public class LaundryReturnResult
    {
        public int laundryLogId { get; set; }
        public int chargeId { get; set; }
        public int paymentId { get; set; }
        public decimal grossAmt { get; set; }
        public string billNo { get; set; } = "";
        public string status { get; set; } = "";
        public string message { get; set; } = "";
    }



}