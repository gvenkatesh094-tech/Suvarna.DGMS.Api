using System;

namespace Suvarna.DGMS.Api.Models
{
    // ==========================================
    // KPI SUMMARY
    // ==========================================
    public class ExecKpiSummary
    {
        public int totalRooms { get; set; }
        public int occupiedRooms { get; set; }
        public int occupancyPct => totalRooms > 0 ? (int)Math.Round(occupiedRooms * 100.0 / totalRooms) : 0;

        public decimal mtdRevenueExclGst { get; set; }
        public decimal gstMtd { get; set; }
        public int activeGuests { get; set; }
        public int hkSlaBreaches { get; set; }
        public int criticalMaintCount { get; set; }
    }

    // ==========================================
    // PROPERTY PERFORMANCE ROW
    // ==========================================
    public class ExecPropertyPerformance
    {
        public int propertyId { get; set; }
        public string propertyName { get; set; } = "";
        public int totalRooms { get; set; }
        public int occupiedRooms { get; set; }
        public decimal occPct { get; set; }
        public decimal adr { get; set; }
        public decimal revPar { get; set; }
        public decimal mtdRevenue { get; set; }
        public decimal gstMtd { get; set; }
        public bool isTotal => propertyId == 0;
    }

    // ==========================================
    // REVENUE TREND ROW
    // ==========================================
    public class ExecRevenueTrendItem
    {
        public DateTime revenueDate { get; set; }
        public string dayLabel { get; set; } = "";
        public decimal subtotalAmount { get; set; }
    }

    // ==========================================
    // GST SLAB SUMMARY ROW
    // ==========================================
    public class ExecGstSlabItem
    {
        public string slabLabel { get; set; } = "";
        public decimal slabPct { get; set; }
        public decimal taxableValue { get; set; }
        public decimal cgst { get; set; }
        public decimal sgst { get; set; }
        public decimal totalGst { get; set; }
        public bool isTotal { get; set; }
    }

    // ==========================================
    // ALERT
    // ==========================================
    public class ExecAlertItem
    {
        public string dotColor { get; set; } = "";
        public string title { get; set; } = "";
        public string subtitle { get; set; } = "";
        public string? route { get; set; }
    }

    // ==========================================
    // OPERATIONAL SNAPSHOT
    // ==========================================
    public class ExecOperationalSnapshot
    {
        public int hkPending { get; set; }
        public int hkInProgress { get; set; }
        public int maintOpen { get; set; }
        public int maintCritical { get; set; }
        public int activeVisitorPasses { get; set; }
        public int blacklistEntries { get; set; }
        public decimal depositBalanceHeld { get; set; }
    }
}
