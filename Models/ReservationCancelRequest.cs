namespace Suvarna.DGMS.Api.Models
{
    public class ReservationCancelRequest
    {
        public int bookingId { get; set; }

        public int sessionId { get; set; }

        public int userId { get; set; }

        public string? cancelRemarks { get; set; }

        // NEW: cancellation charge kept from the advance, the rest is refunded
        public decimal cancelChargeAmount { get; set; }
        public string? refundMode { get; set; }
        public string? refundRef { get; set; }
    }

    // NEW: result of PR_DGMS_CANCEL_RESERVATION
    public class ReservationCancelResult
    {
        public int bookingId { get; set; }
        public decimal refundAmount { get; set; }
        public decimal forfeitedAmount { get; set; }
        public string status { get; set; } = "";
        public string message { get; set; } = "";
    }
}