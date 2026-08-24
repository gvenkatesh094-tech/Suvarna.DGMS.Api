namespace Suvarna.DGMS.Api.Models
{
    public class TransferBookingLookup
    {
        public int bookingId { get; set; }
        public string bookingRef { get; set; } = "";
        public string guestNm { get; set; } = "";
        public string roomNo { get; set; } = "";
        public int propertyId { get; set; }
    }

    public class TransferAvailableRoom
    {
        public int roomId { get; set; }
        public string roomNo { get; set; } = "";
        public string roomType { get; set; } = "";
        public int propertyId { get; set; }
        public decimal dailyRate { get; set; }
        public decimal gstPct { get; set; }
    }

    public class ConfirmTransferRequest
    {
        public int sessionId { get; set; }
        public int bookingId { get; set; }
        public int targetRoomId { get; set; }
        public DateTime transferDate { get; set; }
        public string? reason { get; set; }
    }

    //public class TransferResult
    //{
    //    public string status { get; set; } = "";
    //    public string message { get; set; } = "";
    //}
}
