namespace Suvarna.DGMS.Api.Models
{
    /// <summary>
    /// 03-Nov: New Reservation form - what the mobile number already has
    /// (last stay details to fill the form + reservations / stays overlapping the dates).
    /// </summary>
    public class ReservationGuestCheck
    {
        public ReturningGuest? lastGuest { get; set; }
        public List<OverlappingBooking> overlapping { get; set; } = new();
    }

    public class ReturningGuest
    {
        public string guestName { get; set; } = "";
        public int? genderId { get; set; }
        public string? genderCd { get; set; }
        public string? email { get; set; }
        public string? address { get; set; }
        public DateTime? lastStayDt { get; set; }
        public int stayCount { get; set; }
    }

    public class OverlappingBooking
    {
        public int bookingId { get; set; }
        public string bookingNo { get; set; } = "";
        public string bookingStatus { get; set; } = "";
        public string guestName { get; set; } = "";
        public string? accmTypeName { get; set; }
        public DateTime? checkinDt { get; set; }
        public DateTime? checkoutDt { get; set; }
    }
}
