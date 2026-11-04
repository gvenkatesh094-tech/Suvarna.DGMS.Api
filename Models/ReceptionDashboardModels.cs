using System.Collections.Generic;
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
        public decimal netDueAmount { get; set; }

        // 03-Nov
        public int roomId { get; set; }
        public string? bedNo { get; set; }
        public int totalBeds { get; set; }
        public bool isOverdue { get; set; }
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
        public string? buildingName { get; set; }   // 23-Oct: tells apart rooms with the same name
        public string? floorName { get; set; }

        // 03-Nov: room tile + popup (null / 0 until the 03-Nov SQL script is run)
        public int floorId { get; set; }
        public int floorSeq { get; set; }
        public int accmTypeId { get; set; }
        public string? accmTypeName { get; set; }
        public int totalBeds { get; set; }
        public int maxOccupants { get; set; }
        public string? genderRestrict { get; set; }      // M / F / null = any
        public int occupiedBeds { get; set; }
        public int guestCount { get; set; }
        public string? guestName { get; set; }
        public int bookingId { get; set; }
        public DateTime? expectedCheckoutDt { get; set; }
        public bool isOverdue { get; set; }
        public bool hasDue { get; set; }
        public decimal dueAmount { get; set; }
        public int nextResBookingId { get; set; }
        public string? nextResBookingNo { get; set; }
        public string? nextResGuestName { get; set; }
        public DateTime? nextResCheckinDt { get; set; }
        public int hkTaskId { get; set; }
        public string? hkStatus { get; set; }
        public string? hkPriority { get; set; }
        public string? hkStaffName { get; set; }
        public DateTime? hkTriggeredDt { get; set; }
        public DateTime? hkAssignedDt { get; set; }
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

    // ==========================================
    // 03-Nov: ROOM POPUP
    // ==========================================
    public class ReceptionRoomDetail
    {
        public List<ReceptionRoomBed> beds { get; set; } = new();
        public List<ReceptionRoomGuest> guests { get; set; } = new();
        public List<ReceptionRoomReservation> reservations { get; set; } = new();
    }

    public class ReceptionRoomBed
    {
        public int bedId { get; set; }
        public string bedNo { get; set; } = "";
        public string? bedName { get; set; }
        public string? bedStatus { get; set; }
    }

    public class ReceptionRoomGuest
    {
        public int bookingId { get; set; }
        public string bookingNo { get; set; } = "";
        public int bedId { get; set; }
        public string? guestName { get; set; }
        public string? mobileNo { get; set; }
        public string? genderCd { get; set; }
        public DateTime? checkinDt { get; set; }
        public DateTime? expectedCheckoutDt { get; set; }
        public bool isOverdue { get; set; }
        public string? corpName { get; set; }
        public string? occCatName { get; set; }
        public decimal netDueAmount { get; set; }
    }

    public class ReceptionRoomReservation
    {
        public int bookingId { get; set; }
        public string bookingNo { get; set; } = "";
        public int bedId { get; set; }
        public string? guestName { get; set; }
        public string? mobileNo { get; set; }
        public DateTime? expectedCheckinDt { get; set; }
        public DateTime? expectedCheckoutDt { get; set; }
        public int guestCount { get; set; }
    }

    // ==========================================
    // 03-Nov: TODAY (arrivals / departures)
    // ==========================================
    public class ReceptionTodayItem
    {
        public string listType { get; set; } = "";      // ARR / DEP
        public int bookingId { get; set; }
        public string bookingNo { get; set; } = "";
        public string? guestName { get; set; }
        public string? mobileNo { get; set; }
        public string? genderCd { get; set; }
        public int roomId { get; set; }
        public string? roomNo { get; set; }
        public string? bedNo { get; set; }
        public int accmTypeId { get; set; }
        public string? accmTypeName { get; set; }
        public DateTime? expectedDt { get; set; }
        public bool isLate { get; set; }
        public int guestCount { get; set; }
        public decimal netDueAmount { get; set; }
    }

    // ==========================================
    // 03-Nov: GLOBAL SEARCH
    // ==========================================
    public class ReceptionSearchItem
    {
        public int bookingId { get; set; }
        public string bookingNo { get; set; } = "";
        public string bookingStatus { get; set; } = "";
        public string? guestName { get; set; }
        public string? mobileNo { get; set; }
        public string? umrNo { get; set; }
        public int roomId { get; set; }
        public string? roomNo { get; set; }
        public string? bedNo { get; set; }
        public DateTime? expectedCheckinDt { get; set; }
        public DateTime? expectedCheckoutDt { get; set; }
    }

    // ==========================================
    // 03-Nov: BEST ROOM FOR AN ARRIVAL
    // ==========================================
    public class ReceptionRoomSuggestion
    {
        public int roomId { get; set; }
        public string roomNo { get; set; } = "";
        public string? floorName { get; set; }
        public string? accmTypeName { get; set; }
        public string? roomStatus { get; set; }
        public int totalBeds { get; set; }
        public int maxOccupants { get; set; }
        public int freeBeds { get; set; }
        public int bedId { get; set; }
        public string? bedNo { get; set; }
        public string? genderRestrict { get; set; }
        public DateTime? freeUntilDt { get; set; }
        public int fit { get; set; }                      // 1 best, 0 fits, -1 no
        public string? reason { get; set; }
        public int guestCount { get; set; }
    }

    public class ReceptionRoomActionRequest
    {
        public int sessionId { get; set; }
        public int roomId { get; set; }
    }

    // ==========================================
    // 03-Nov: SHIFT HANDOVER
    // ==========================================
    public class ShiftHandover
    {
        public string shiftName { get; set; } = "";
        public DateTime? shiftStartDt { get; set; }
        public DateTime? shiftEndDt { get; set; }
        public string? userName { get; set; }
        public int checkinCount { get; set; }
        public int checkoutCount { get; set; }
        public int reservationCount { get; set; }
        public decimal cashAmount { get; set; }
        public decimal upiAmount { get; set; }
        public decimal cardAmount { get; set; }
        public decimal otherAmount { get; set; }
        public decimal totalAmount { get; set; }
        public int arrivalsPending { get; set; }
        public int dirtyRooms { get; set; }
        public int maintenanceRooms { get; set; }
        public List<ShiftOverdueGuest> overdueGuests { get; set; } = new();
        public ShiftPreviousNote? previousNote { get; set; }
    }

    public class ShiftOverdueGuest
    {
        public int bookingId { get; set; }
        public string bookingNo { get; set; } = "";
        public string? roomNo { get; set; }
        public string? guestName { get; set; }
        public DateTime? expectedCheckoutDt { get; set; }
        public decimal netDueAmount { get; set; }
    }

    public class ShiftPreviousNote
    {
        public string? shiftName { get; set; }
        public string? notes { get; set; }
        public string? handedTo { get; set; }
        public DateTime? createDt { get; set; }
        public string? createdByName { get; set; }
    }

    public class ShiftHandoverSaveRequest
    {
        public int sessionId { get; set; }
        public string? notes { get; set; }
        public string? handedTo { get; set; }
    }
}
