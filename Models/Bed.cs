namespace Suvarna.DGMS.Api.Models
{
    public class Bed
    {
        public int bedId { get; set; }

        public short grpId { get; set; }
        public short orgId { get; set; }
        public short locId { get; set; }

        public int roomId { get; set; }
        public int propertyId { get; set; }

        public string bedNo { get; set; }

        public string bedType { get; set; }

        public string bedStatus { get; set; }

        public short sortOrder { get; set; }

        public int userId { get; set; }

        public string sessionId { get; set; }
    }
}
