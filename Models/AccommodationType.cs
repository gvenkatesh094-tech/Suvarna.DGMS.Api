namespace Suvarna.DGMS.Api.Models
{
    public class AccommodationType
    {
        public int accmTypeId { get; set; }

        public string? accmTypeCode { get; set; }

        public string? accmTypeName { get; set; }
        public decimal? gst { get; set; }

        public decimal? roomPrice { get; set; }
    }
}