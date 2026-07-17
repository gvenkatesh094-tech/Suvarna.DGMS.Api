namespace Suvarna.DGMS.Api.Models
{
    public class CheckinRoom
    {
        public int roomId { get; set; }

        public string? roomNo { get; set; }
        public string? roomName { get; set; }
        public string? roomStatus { get; set; }

        public int propertyId { get; set; }
        public string? propertyName { get; set; }
        public string? propertyCode { get; set; }

        public int? buildingId { get; set; }
        public string? buildingName { get; set; }

        public int? floorId { get; set; }
        public string? floorName { get; set; }
        public int floorSeq { get; set; }

        public int accmTypeId { get; set; }
        public string? accmTypeName { get; set; }
        public string? accmCategory { get; set; }

        public int maxOccupants { get; set; }
        public int totalBeds { get; set; }
        public int availableBeds { get; set; }

        public string? isAc { get; set; }
        public string? isAttachedBath { get; set; }
        public string? isWifi { get; set; }
        public string? isTv { get; set; }
        public string? isAccessible { get; set; }

        public string? wardProximity { get; set; }

        public string? blockedReason { get; set; }
        public DateTime? blockedFromDt { get; set; }
        public DateTime? blockedToDt { get; set; }
    }
}