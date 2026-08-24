namespace Suvarna.DGMS.Api.Models
{
    public class ActiveGuestRow
    {
        public int bookingId { get; set; }
        public string bookingRef { get; set; } = "";
        public string guestNm { get; set; } = "";
        public string mobileNo { get; set; } = "";
        public string roomNo { get; set; } = "";
        public string categoryNm { get; set; } = "";
        public DateTime? checkinDt { get; set; }
        public DateTime? checkoutDt { get; set; }
        public int nights { get; set; }
        public decimal dailyRate { get; set; }
        public decimal gstPct { get; set; }
        public decimal chargedAmt { get; set; }
        public decimal depositAmt { get; set; }
        public string? umrNo { get; set; }
        public string? admnNo { get; set; }
        public string? wardNm { get; set; }
        public string? corpNm { get; set; }
    }
}
