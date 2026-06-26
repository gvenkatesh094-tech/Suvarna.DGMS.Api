namespace Suvarna.DGMS.Api.Models
{
    public class Building
    {
        public int buildingId { get; set; }

        public short grpId { get; set; }
        public short orgId { get; set; }
        public short locId { get; set; }

        public int propertyId { get; set; }

        public string buildingCode { get; set; }
        public string buildingName { get; set; }

        public short totalFloors { get; set; }

        public string? remarks { get; set; }

        public int userId { get; set; }
    }
}
