namespace Suvarna.DGMS.Api.Models
{
    public class Tariff
    {
        public string? mode { get; set; }

        public int tariffId { get; set; }

        public short grpId { get; set; }
        public short orgId { get; set; }
        public short locId { get; set; }

        public string? tariffCode { get; set; }
        public string? tariffName { get; set; }
        public string? tariffType { get; set; }

        public int? propertyId { get; set; }
        public int? accmTypeId { get; set; }
        public int? occCatId { get; set; }

        public string? billingModel { get; set; }

        public decimal rate { get; set; }

        public decimal gst { get; set; }

        public string? sacCode { get; set; }

        public decimal deposit { get; set; }

        public string? status { get; set; }

        public int userId { get; set; }

        public int sessionId { get; set; }
    }
}