using System;

namespace Suvarna.DGMS.Api.Models
{
    public class CheckoutResult
    {
        //=========================================
        // Status
        //=========================================

        public string status { get; set; } = string.Empty;

        public string message { get; set; } = string.Empty;

        //=========================================
        // Booking
        //=========================================

        public int bookingId { get; set; }

        public string bookingNo { get; set; } = string.Empty;

        public DateTime checkoutDate { get; set; }

        //=========================================
        // Bill
        //=========================================

        public int billId { get; set; }

        public string billNo { get; set; } = string.Empty;

        public decimal grossAmount { get; set; }

        public decimal discountAmount { get; set; }

        public decimal gstAmount { get; set; }

        public decimal netPayable { get; set; }

        //=========================================
        // Deposit
        //=========================================

        public decimal depositAdjusted { get; set; }

        public decimal depositRefund { get; set; }

        //=========================================
        // Payment
        //=========================================

        public decimal paymentReceived { get; set; }

        public string paymentMode { get; set; } = string.Empty;

        public string? paymentReference { get; set; }

        //=========================================
        // Receipt
        //=========================================

        public int paymentTxnId { get; set; }

        public string receiptNo { get; set; } = string.Empty;

        //=========================================
        // Room
        //=========================================

        public int roomId { get; set; }

        public string roomNo { get; set; } = string.Empty;

        public int? bedId { get; set; }

        public string? bedNo { get; set; }

        //=========================================
        // Flags
        //=========================================

        public bool roomReleased { get; set; }

        public bool bedReleased { get; set; }

        public bool checkoutCompleted { get; set; }
        public decimal penaltyAmount { get; set; }
    }
}