using System;

namespace Suvarna.DGMS.Api.Models
{
    // ==========================================
    // DASHBOARD SUMMARY
    // ==========================================
    public class ReceptionDashboardSummary
    {
        public int totalRooms { get; set; }
        public int occupiedRooms { get; set; }
        public int availableRooms { get; set; }
        public int pendingCheckin { get; set; }
        public int overdueCheckin { get; set; }
        public int dueCheckout { get; set; }
        public int overdueCheckout { get; set; }
        public int reservations { get; set; }
        public int arrivingTomorrow { get; set; }
        public decimal occupancyRate { get; set; }
        public decimal todayRevenue { get; set; }
        public decimal mtdRevenue { get; set; }
    }

    // ==========================================
    // ACTIVE BOOKING ROW
    // ==========================================
    public class ReceptionActiveBooking
    {
        public int bookingId { get; set; }
        public string bookingRef { get; set; } = "";
        public string guestName { get; set; } = "";
        public string mobileNo { get; set; } = "";
        public string roomNo { get; set; } = "";
        public string categoryName { get; set; } = "";
        public DateTime? checkinDt { get; set; }
        public DateTime? expectedCheckoutDt { get; set; }
        public decimal chargedAmount { get; set; }
        public decimal gstPercent { get; set; }
        public decimal depositAmount { get; set; }
    }

    // ==========================================
    // ROOM STATUS ITEM
    // ==========================================
    public class ReceptionRoomStatus
    {
        public int roomId { get; set; }
        public string roomNo { get; set; } = "";
        public string? roomName { get; set; }
        public string roomStatus { get; set; } = "";
    }

    //// ==========================================
    //// ALERT
    //// ==========================================
    //public class ReceptionAlert
    //{
    //    public int alertId { get; set; }
    //    public string alertColor { get; set; } = "";
    //    public string alertTitle { get; set; } = "";
    //    public string alertSubtitle { get; set; } = "";
    //    public string actionText { get; set; } = "";
    //}

    // ==========================================
    // REVENUE (7-day trend)
    // ==========================================
    public class ReceptionRevenueItem
    {
        public string dayName { get; set; } = "";
        public decimal amount { get; set; }
    }
}
