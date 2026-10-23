using System;

namespace Suvarna.DGMS.Api.Models
{
    // ==========================================
    // KPI SUMMARY (MTD)
    // ==========================================
    public class RevenueKpiSummary
    {
        public decimal mtdRevenueExclGst { get; set; }
        public decimal accommodationRevenue { get; set; }
        public decimal serviceRevenue { get; set; }
        public decimal gstCollected { get; set; }
        public decimal grossRevenue { get; set; }
        public int billCount { get; set; }

        // NEW (10_round_e): penalties + cancellation charges, check-out discounts, and the room charge of
        // guests still in-house (accommodation is billed at check-out, so it is not in the MTD figures yet)
        public decimal otherRevenue { get; set; }
        public decimal discountAmount { get; set; }
        public decimal unbilledStayAmount { get; set; }
    }

    // ==========================================
    // DAILY REVENUE ROW
    // ==========================================
    public class RevenueDailyItem
    {
        public DateTime revenueDate { get; set; }
        public string dayLabel { get; set; } = "";
        public decimal accommodationAmount { get; set; }
        public decimal foodAmount { get; set; }
        public decimal laundryAmount { get; set; }
        public decimal otherAmount { get; set; }
        public decimal discountAmount { get; set; }      // NEW: check-out discounts (already deducted from subtotal)
        public decimal subtotalAmount { get; set; }
        public decimal gstAmount { get; set; }
        public decimal grossAmount { get; set; }
    }

    // ==========================================
    // PAYMENT MODE SPLIT (bonus)
    // ==========================================
    public class RevenuePaymentModeItem
    {
        public string paymentMode { get; set; } = "";
        public decimal totalAmount { get; set; }          // received - refunded
        public int txnCount { get; set; }
        public decimal receivedAmount { get; set; }       // NEW
        public decimal refundedAmount { get; set; }       // NEW
    }
}
