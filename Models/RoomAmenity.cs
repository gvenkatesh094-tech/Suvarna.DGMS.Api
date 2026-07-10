namespace Suvarna.DGMS.Api.Models
{
    public class RoomAmenity
    {
        public string? mode { get; set; }

        public int dgmAmenityMastId { get; set; }

        public int grpId { get; set; }

        public int orgId { get; set; }

        public int locId { get; set; }

        public string? sessionId { get; set; }

        public int userId { get; set; }

        public string? amenityCd { get; set; }

        public string? amenityNm { get; set; }

        public string? amenityCategory { get; set; }

        public string? iconCd { get; set; }

        public string? isChargeable { get; set; }

        public decimal chargeAmt { get; set; }

        public short sortOrder { get; set; }

        public string? recordStatus { get; set; }

        public int createBy { get; set; }

        public DateTime? createDt { get; set; }

        public int modifyBy { get; set; }

        public DateTime? modifyDt { get; set; }
        public string? isMandatory { get; set; }
    }
}
