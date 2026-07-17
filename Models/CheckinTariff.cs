namespace Suvarna.DGMS.Api.Models
{
    public class CheckinTariff
    {
        public int tariffId { get; set; }

        public string? tariffCode { get; set; }
        public string? tariffName { get; set; }
        public string? tariffType { get; set; }

        public string? billingModel { get; set; }

        public decimal dailyRate { get; set; }
        public decimal hourlyRate { get; set; }
        public decimal monthlyRate { get; set; }

        public decimal packageRate { get; set; }
        public int packageDays { get; set; }

        public int minBillingHours { get; set; }

        public int checkinHour { get; set; }
        public int checkoutHour { get; set; }

        public decimal gstSlabPct { get; set; }

        public string? isGstInclusive { get; set; }

        public string? sacCode { get; set; }

        public decimal cgstPct { get; set; }
        public decimal sgstPct { get; set; }
        public decimal igstPct { get; set; }

        public string? isGstExempt { get; set; }

        public decimal securityDepositAmount { get; set; }

        public string? isDepositRefundable { get; set; }

        public decimal baseDailyAmount { get; set; }
        public decimal gstAmountPerDay { get; set; }
    }
}