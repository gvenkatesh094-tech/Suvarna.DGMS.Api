namespace Suvarna.DGMS.Api.Models
{
    public class ReservationAvailability
    {
        public int totalRooms { get; set; }

        public int blockedRooms { get; set; }

        public int availableRooms { get; set; }

        public bool isAvailable { get; set; }
    }
}