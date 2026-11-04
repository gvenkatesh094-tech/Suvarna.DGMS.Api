namespace Suvarna.DGMS.Api.Models
{
    // NEW (28-Oct): after checkout - did the room go to housekeeping, and what state is it in now.
    // One row per checked-out booking (PR_DGMS_GET_CHECKOUT_HK_STATUS). Times are location time.
    public class CheckoutHkStatus
    {
        public int bookingId { get; set; }
        public string bookingNo { get; set; } = "";
        public string guestName { get; set; } = "";

        public int roomId { get; set; }
        public string roomNo { get; set; } = "";
        public string roomName { get; set; } = "";
        public string roomStatus { get; set; } = "";      // DIRTY / INSPECTING / AVAILABLE / OCCUPIED ...
        public string? bedNo { get; set; }
        public string? bedStatus { get; set; }
        public int otherGuestsInRoom { get; set; }        // shared room: still occupied by others

        public DateTime? checkoutDate { get; set; }

        // housekeeping task raised by the checkout (null = not created)
        public int? hkTaskId { get; set; }
        public string? hkTaskNo { get; set; }
        public string? hkTaskStatus { get; set; }         // PENDING / ASSIGNED / IN_PROGRESS / INSPECTING / READY ...
        public string? hkPriority { get; set; }
        public string? hkStaffName { get; set; }
        public int? slaMinutes { get; set; }
        public DateTime? hkTriggeredDate { get; set; }
        public DateTime? hkSlaDueDate { get; set; }
        public bool slaBreached { get; set; }
    }
}
