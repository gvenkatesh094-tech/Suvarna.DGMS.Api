namespace Suvarna.DGMS.Api.Models
{
    public class Property
    {
        public int propertyId { get; set; }

        public short grpId { get; set; }
        public short orgId { get; set; }
        public short locId { get; set; }

        public string propertyCode { get; set; }
        public string propertyName { get; set; }
        public string propertyType { get; set; }

        public string? city { get; set; }
        public string? state { get; set; }
        public string? pinCode { get; set; }

        public string? gstin { get; set; }
        public string? email { get; set; }
        public string? address { get; set; }

        public int userId { get; set; }
    }
}
