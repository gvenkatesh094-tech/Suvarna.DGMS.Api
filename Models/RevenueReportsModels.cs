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
        public decimal totalAmount { get; set; }
        public int txnCount { get; set; }
    }
}
