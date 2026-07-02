namespace Suvarna.DGMS.Api.Models
{
    public class Building
    {
        public int buildingId { get; set; }
        public string buildingCode { get; set; }
        public string buildingName { get; set; }
        public int propertyId { get; set; }
        public string propertyCode { get; set; }
        public string propertyName { get; set; }
        public short totalFloors { get; set; }
        public string status { get; set; }
        public string? remarks { get; set; }

        public int userId { get; set; }
        public short grpId { get; set; }
        public short orgId { get; set; }
        public short locId { get; set; }
    }
    public class DeleteBuildingRequest
    {
        public int buildingId { get; set; }
    }
}
