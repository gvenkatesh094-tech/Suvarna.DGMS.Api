namespace Suvarna.DGMS.Api.Models
{
    public class Bed
    {
        public int bedId { get; set; }

        public string mode { get; set; }

        public short grpId { get; set; }

        public short orgId { get; set; }

        public short locId { get; set; }

        // Property
        public int propertyId { get; set; }

        // Building
        public int buildingId { get; set; }

        // Floor
        public int floorId { get; set; }

        // Room
        public int roomId { get; set; }

        public string roomName { get; set; }

        public int buildingId { get; set; }

        public int floorId { get; set; }

        // Bed
        public string bedNo { get; set; }

        public string bedType { get; set; }

        public string bedStatus { get; set; }

        public short sortOrder { get; set; }

        public int userId { get; set; }

        public int sessionId { get; set; }

        // Optional (if you want to display current occupant)
        public string? occupant { get; set; }
    }
}