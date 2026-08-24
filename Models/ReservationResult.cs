namespace Suvarna.DGMS.Api.Models
{
    public class ReservationResult
    {
        public int bookingId { get; set; }

        public string bookingNo { get; set; } = "";

        public string status { get; set; } = "";

        public string message { get; set; } = "";
    }
}