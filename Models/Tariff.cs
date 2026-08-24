namespace Suvarna.DGMS.Api.Models
{
    public class Tariff
    {          
        public string? mode { get; set; }        
        public int tariffId { get; set; }

        public short grpId { get; set; }
        public short orgId { get; set; }
        public short locationId { get; set; }

        public string? tariffCode { get; set; }
        public string? tariffName { get; set; }
        public string? tariffType { get; set; }

        public int? propertyId { get; set; }
        public int?  billingModelId { get; set; }
        public int? accmTypeId { get; set; }
        public int? occCatId { get; set; }

        public string? billingModelCode { get; set; }

        public string? billingModelName { get; set; }

        public decimal rate { get; set; }

        public decimal gst { get; set; }

        public string? sacCode { get; set; }

        public decimal deposit { get; set; }

        public string? status { get; set; }

        public int userId { get; set; }

        public int sessionId { get; set; }
        public decimal dailyRate { get; set; }

        public decimal hourlyRate { get; set; }

        public decimal monthlyRate { get; set; }

        public decimal packageRate { get; set; }

        public short packageDays { get; set; }

        public short minimumBillingHours { get; set; }

        public short checkinHour { get; set; }

        public short checkoutHour { get; set; }

        

        public string? isGstInclusive { get; set; }

        

        public decimal cgst { get; set; }

        public decimal sgst { get; set; }

        public decimal igst { get; set; }

        public string? isGstExempt { get; set; }
        public string? isDepositRefundable { get; set; }

        public DateTime? effectiveFrom { get; set; }

        public DateTime? effectiveTo { get; set; }

        public string? isDynamicPricing { get; set; }

        public decimal peakMultiplier { get; set; }

        public decimal offPeakMultiplier { get; set; }

        public string? isDefault { get; set; }

        public string? remarks { get; set; }
        public string? accommodationName { get; set; }
        public string? propertyName { get; set; }
        public int? accCatId { get; set; }



    }
    public class TariffSaveRequest
    {
        public string? mode { get; set; }

        public int tariffId { get; set; }

        public short grpId { get; set; }

        public short orgId { get; set; }

        public short locationId { get; set; }

        public string? tariffCode { get; set; }

        public string? tariffName { get; set; }

        public string? tariffType { get; set; }

        public int? propertyId { get; set; }

        public int? accmTypeId { get; set; }

        public int? occCatId { get; set; }

        public int? billingModelId { get; set; }

        public string? billingModelCode { get; set; }

        public string? billingModelName { get; set; }

        public decimal dailyRate { get; set; }

        public decimal hourlyRate { get; set; }

        public decimal monthlyRate { get; set; }

        public decimal packageRate { get; set; }

        public short packageDays { get; set; }

        public short minimumBillingHours { get; set; }

        public short checkinHour { get; set; }

        public short checkoutHour { get; set; }

        public decimal gst { get; set; }

        public string? isGstInclusive { get; set; }

        public string? sacCode { get; set; }

        public decimal cgst { get; set; }

        public decimal sgst { get; set; }

        public decimal igst { get; set; }

        public string? isGstExempt { get; set; }

        public decimal deposit { get; set; }

        public string? isDepositRefundable { get; set; }

        public DateTime? effectiveFrom { get; set; }

        public DateTime? effectiveTo { get; set; }

        public string? isDynamicPricing { get; set; }

        public decimal peakMultiplier { get; set; }

        public decimal offPeakMultiplier { get; set; }

        public string? isDefault { get; set; }

        public string? remarks { get; set; }

        public string? status { get; set; }

        public int userId { get; set; }

        public int sessionId { get; set; }
        public int? accCatId { get; set; }
    }
    public class BillingModelMaster
    {
        public int billingModelId { get; set; }
        public string billingModelCode { get; set; } = string.Empty;
        public string billingModelName { get; set; } = string.Empty;
        public string description { get; set; } = string.Empty;
        public int displayOrder { get; set; }
        public string status { get; set; } = string.Empty;
        public int createdBy { get; set; }
        public DateTime? createdDate { get; set; }
        public int modifiedBy { get; set; }
        public DateTime? modifiedDate { get; set; }
    }
}