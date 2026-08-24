namespace Suvarna.DGMS.Api.Models
{
    public class ReservationCancelRequest
    {
        public int bookingId { get; set; }

        public int sessionId { get; set; }

        public int userId { get; set; }

        public string? cancelRemarks { get; set; }
    }
}