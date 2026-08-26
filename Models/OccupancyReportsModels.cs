using System;

namespace Suvarna.DGMS.Api.Models
{
    // ==========================================
    // KPI SUMMARY
    // ==========================================
    public class OccKpiSummary
    {
        public int totalRooms { get; set; }
        public int occupiedRooms { get; set; }
        public int availableRooms { get; set; }
        public decimal? avgStayNights { get; set; }
        public int occupancyPct => totalRooms > 0 ? (int)Math.Round(occupiedRooms * 100.0 / totalRooms) : 0;
    }

    // ==========================================
    // ROOM TYPE ROW
    // ==========================================
    public class OccRoomTypeStat
    {
        public int accmTypeId { get; set; }
        public string accmTypeName { get; set; } = "";
        public int totalRooms { get; set; }
        public int occupiedRooms { get; set; }
        public decimal occPct { get; set; }
        public decimal adr { get; set; }
        public decimal revPar { get; set; }
    }

    // ==========================================
    // CATEGORY BREAKDOWN ROW
    // ==========================================
    public class OccCategoryStat
    {
        public int occCatId { get; set; }
        public string categoryName { get; set; } = "";
        public int bookingCount { get; set; }
        public int roomNights { get; set; }
        public decimal revenue { get; set; }
    }

    // ==========================================
    // TREND ROW
    // ==========================================
    public class OccTrendItem
    {
        public DateTime occDate { get; set; }
        public string dayLabel { get; set; } = "";
        public decimal occPct { get; set; }
    }

    // ==========================================
    // ROOM STATUS GRID ITEM
    // ==========================================
    public class OccRoomStatusItem
    {
        public int roomId { get; set; }
        public string roomNo { get; set; } = "";
        public string roomStatus { get; set; } = "";
    }
}
