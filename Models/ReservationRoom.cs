namespace Suvarna.DGMS.Api.Models
{
    public class ReservationRoom
    {
        public int roomId { get; set; }

        public string roomNo { get; set; } = "";

        public string? roomName { get; set; }

        public string? roomStatus { get; set; }

        public int propertyId { get; set; }

        public string? propertyName { get; set; }

        public int? buildingId { get; set; }

        public string? buildingName { get; set; }

        public int? floorId { get; set; }

        public string? floorName { get; set; }

        public int accmTypeId { get; set; }

        public string? accmTypeName { get; set; }

        public int totalBeds { get; set; }

        public int availableBeds { get; set; }
    }
}