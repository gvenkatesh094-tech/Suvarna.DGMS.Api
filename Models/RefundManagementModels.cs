namespace Suvarna.DGMS.Api.Models
{
    public class RefundEligibleBooking
    {
        public int bookingId { get; set; }
        public string bookingRef { get; set; } = "";
        public string guestNm { get; set; } = "";
        public string roomNo { get; set; } = "";
        public int depositHdrId { get; set; }
        public decimal depositHeld { get; set; }
        public decimal chargedAmt { get; set; }
    }

    public class SubmitRefundRequest
    {
        public int sessionId { get; set; }
        public int bookingId { get; set; }
        public decimal amount { get; set; }
        public string mode { get; set; } = "";
        public string? accountRef { get; set; }
        public string reasonCd { get; set; } = "";
        public string? remarks { get; set; }
    }

    public class SubmitRefundResult
    {
        public int requestId { get; set; }
        public string requestNo { get; set; } = "";
        public bool requiresApproval { get; set; }
        public string status { get; set; } = "";
        public string message { get; set; } = "";
    }

    public class RefundRequestRow
    {
        public int requestId { get; set; }
        public string requestNo { get; set; } = "";
        public int bookingId { get; set; }
        public string bookingNo { get; set; } = "";
        public string guestNm { get; set; } = "";
        public string roomNo { get; set; } = "";
        public decimal refundAmount { get; set; }
        public string refundMode { get; set; } = "";
        public string? accountRef { get; set; }
        public string reasonCd { get; set; } = "";
        public string? remarks { get; set; }
        public bool requiresApproval { get; set; }
        public string status { get; set; } = "";
        public string? approvedByName { get; set; }
        public DateTime? approvedDt { get; set; }
        public string? approvalRemarks { get; set; }
        public string? processedByName { get; set; }
        public DateTime? processedDt { get; set; }
        public DateTime createDt { get; set; }
    }

    public class DecideRefundRequest
    {
        public int sessionId { get; set; }
        public int requestId { get; set; }
        public string decision { get; set; } = ""; // APPROVE / REJECT
        public string? remarks { get; set; }
    }

    public class RefundActionResult
    {
        public string status { get; set; } = "";
        public string message { get; set; } = "";
    }
}
