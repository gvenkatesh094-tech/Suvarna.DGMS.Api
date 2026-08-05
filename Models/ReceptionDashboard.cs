namespace Suvarna.DGMS.Api.Models
{
    public class ReceptionDashboardSummary
    {
        public int TotalRooms { get; set; }
        public int OccupiedRooms { get; set; }
        public int AvailableRooms { get; set; }
        public int PendingCheckIn { get; set; }
        public int DueCheckOut { get; set; }
        public int Reservations { get; set; }
        public decimal OccupancyRate { get; set; }
    }

    public class ReceptionBooking
    {
        public int BookingId { get; set; }
        public string BookingRef { get; set; }
        public string GuestName { get; set; }
        public string MobileNo { get; set; }
        public string RoomNo { get; set; }
        public string Category { get; set; }

        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }

        public decimal ChargedAmount { get; set; }
        public decimal GstPercent { get; set; }
        public decimal DepositAmount { get; set; }
    }

    public class ReceptionRoomStatus
    {
        public int RoomId { get; set; }

        public string RoomNo { get; set; }

        public string RoomName { get; set; }

        public string RoomStatus { get; set; }
    }

    public class ReceptionAlert
    {
        public string AlertColor { get; set; }

        public string AlertTitle { get; set; }

        public string AlertDescription { get; set; }

        public string ActionText { get; set; }
    }

    public class ReceptionRevenue
    {
        public string DayName { get; set; }

        public decimal Amount { get; set; }
    }
}