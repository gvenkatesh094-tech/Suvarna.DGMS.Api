namespace Suvarna.DGMS.Api.Models
{
    public class GstConfig
    {
        public int gstConfigId { get; set; }

        public short grpId { get; set; }

        public short orgId { get; set; }

        public short locId { get; set; }

        public int propertyId { get; set; }

        public string sacCode { get; set; }

        public string sacDescription { get; set; }

        public string gstSlabCd { get; set; }

        public decimal gstPct { get; set; }

        public decimal cgstPct { get; set; }

        public decimal sgstPct { get; set; }

        public decimal igstPct { get; set; }

        public string isIntraState { get; set; }

        public string stateCd { get; set; }

        public decimal thresholdAmtFrom { get; set; }

        public decimal thresholdAmtTo { get; set; }

        public DateTime? effectiveFromDt { get; set; }

        public DateTime? effectiveToDt { get; set; }

        // public string remarks { get; set; }

        public string recordStatus { get; set; }

        public int createBy { get; set; }

        public DateTime? createDt { get; set; }
    }
}
