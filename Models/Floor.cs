namespace Suvarna.DGMS.Api.Models
{
    public class Floor
    {
        public int floorId { get; set; }    
        public int buildingId { get; set; }
        public int propertyId { get; set; }

        public string floorCode { get; set; }
        public string floorName { get; set; }
        public string buildingName { get; set; }

    
        public string status { get; set; }
        public short floorSeq { get; set; }

        public string? remarks { get; set; }

        public int userId { get; set; }
        public short grpId { get; set; }
        public short orgId { get; set; }
        public short locId { get; set; }
    }
    public class DeleteFloorRequest
    {
        public int floorId { get; set; }
    }
}
