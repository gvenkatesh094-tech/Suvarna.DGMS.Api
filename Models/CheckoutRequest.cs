using System;

namespace Suvarna.DGMS.Api.Models
{
    public class CheckoutRequest
    {
        public string sessionId { get; set; } = string.Empty;

        public int bookingId { get; set; }

        public DateTime checkoutDt { get; set; }

        public string paymentMode { get; set; } = string.Empty;

        public decimal paymentAmount { get; set; }

        public string? paymentReference { get; set; }

        public decimal discountAmount { get; set; }

        public string? discountReason { get; set; }

        public bool roomInspected { get; set; }

        public bool keyReturned { get; set; }

        public bool signatureObtained { get; set; }

        public bool isCompassionate { get; set; }

        public string? remarks { get; set; }
    }
}