namespace Suvarna.DGMS.Api.Models
{
    public class Room
    {
        public int roomId { get; set; }

        public string? mode { get; set; }

        public int propertyId { get; set; }

        public int buildingId { get; set; }

        public int floorId { get; set; }

        public int accmTypeId { get; set; }

        public string roomNo { get; set; }

        public string? roomName { get; set; }

        public short totalBeds { get; set; }

        public decimal? floorAreaSqft { get; set; }

        public string isAttachedBath { get; set; } = "Y";

        public string isAc { get; set; } = "N";

        public string isTv { get; set; } = "N";

        public string isWifi { get; set; } = "N";

        public string isAccessible { get; set; } = "N";

        public string? wardProximity { get; set; }

        public string? phoneExtension { get; set; }

        public short lockerCount { get; set; }

        public string? remarks { get; set; }

        public int sessionId { get; set; }

        public int userId { get; set; }

        public string? roomType { get; set; }

        public decimal? roomPrice { get; set; }

        public decimal? gst { get; set; }

        public int? maxOccupants { get; set; }

        public string? roomStatus { get; set; }
    }
}
