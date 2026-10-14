using System;

namespace Suvarna.DGMS.Api.Models
{
    public class CheckoutRequest
    {
        public int sessionId { get; set; } 

        public int bookingId { get; set; }

        public DateTime checkoutDt { get; set; }

        public string? paymentMode { get; set; } = string.Empty;

        public decimal paymentAmount { get; set; }

        public string? paymentReference { get; set; }

        public decimal discountAmount { get; set; }

        public string? discountReason { get; set; }

        public bool roomInspected { get; set; }

        public bool keyReturned { get; set; }

        public bool signatureObtained { get; set; }

        public bool isCompassionate { get; set; }

        public string? remarks { get; set; }
        public decimal penaltyAmount { get; set; }
        public string? penaltyReasonCode { get; set; }
        public string? penaltyRemarks { get; set; }



        public string? refundMode { get; set; }        
        public string? refundReference { get; set; }
        // NEW: mode-specific payment capture
        public string? paymentRemarks { get; set; }   // CASH only
        public string? cardLast4 { get; set; }        // ONLINE, last 4 digits only — never store a full card number
        public string? upiVpa { get; set; }           // ONLINE, UPI VPA
        public bool laundryAcknowledged { get; set; }


    }




}