using System;

namespace Suvarna.DGMS.Api.Models
{
    // ==========================================
    // GRID ROW — one booking currently holding a deposit
    // ==========================================
    public class DepositLedgerBooking
    {
        public int bookingId { get; set; }
        public string bookingRef { get; set; } = "";
        public string guestName { get; set; } = "";
        public string mobileNo { get; set; } = "";

        public string roomNo { get; set; } = "";
        public int propertyId { get; set; }
        public string propertyName { get; set; } = "";

        public string bookingStatus { get; set; } = "";
        public DateTime? checkinDt { get; set; }
        public DateTime? expectedCheckoutDt { get; set; }

        public int depositHdrId { get; set; }
        public string depositNo { get; set; } = "";

        public decimal depositAmount { get; set; }
        public decimal adjustedAmount { get; set; }
        public decimal refundedAmount { get; set; }
        public decimal forfeitedAmount { get; set; }
        public decimal balanceAmount { get; set; }
        public string depositStatus { get; set; } = "";

        // Charges raised on the bill so far, excluding GST — drives the
        // "Charges (excl GST)" column in the grid.
        public decimal chargedAmount { get; set; }

        public DateTime? duesAsOfDt { get; set; }
        public int estStayDays { get; set; }
        public decimal dailyRate { get; set; }
        public decimal estRoomAmount { get; set; }
        public decimal estRoomGst { get; set; }
        public decimal estServiceAmount { get; set; }
        public decimal estServiceGst { get; set; }
        public decimal paidAmount { get; set; }        // payments already made during the stay
        public decimal estNetDues { get; set; }        // what the guest still owes (estimate)
        public decimal refundableAmount { get; set; }  // balance held − estimated dues (never < 0)
    }

    // ==========================================
    // AUDIT TRAIL ROW — one deposit transaction
    // ==========================================
    public class DepositTxnHistory
    {
        public int txnId { get; set; }
        public string txnType { get; set; } = "";       // COLLECT / ADJUSTMENT / REFUND / FORFEIT
        public DateTime txnDt { get; set; }
        public decimal amount { get; set; }

        public string? paymentMode { get; set; }
        public string? paymentRef { get; set; }

        public string? refundMode { get; set; }
        public string? refundRef { get; set; }

        public string remarks { get; set; } = "";
        public int createBy { get; set; }
        public DateTime createDt { get; set; }
    }

    // ==========================================
    // REFUND REQUEST (posted from the ledger's Refund modal)
    // ==========================================
    public class DepositRefundRequest
    {
        public int sessionId { get; set; }
        public int bookingId { get; set; }
        public decimal refundAmount { get; set; }
        public string refundMode { get; set; } = "";     // CASH / CARD / UPI / BANK_TRANSFER / CHEQUE
        public string? refundRef { get; set; }
        public string? remarks { get; set; }
    }

    // ==========================================
    // REFUND RESULT
    // ==========================================
    public class DepositRefundResult
    {
        public int depositTxnId { get; set; }
        public decimal balanceAmount { get; set; }
        public string status { get; set; } = "";
        public string message { get; set; } = "";
    }
}
