namespace Suvarna.DGMS.Api.Models
{
    public class CorpAgreement
    {
        public int CorpAgreementMastId { get; set; }

        public short GrpId { get; set; }

        public short OrgId { get; set; }

        public short LocId { get; set; }
        public int sessionId { get; set; }

        public int propertyId { get; set; }

        public string propertyName { get; set; }

        public int roomAllocate { get; set; }

        public string AgreementNo { get; set; }

        public string CorpNm { get; set; }

        public string CorpGstin { get; set; }

        public string CorpAddress { get; set; }

        public string ContactNm { get; set; }

        public string ContactEmail { get; set; }

        public string ContactPhone { get; set; }

        public string AgreementType { get; set; }

        public string BillingCycle { get; set; }

        public decimal CreditLimitAmt { get; set; }

        public short CreditDays { get; set; }

        public decimal DiscountPct { get; set; }

        public int TariffId { get; set; }

        public string IsGstRegistered { get; set; }

        public DateTime EffectiveFromDt { get; set; }

        public DateTime EffectiveToDt { get; set; }

        public string Remarks { get; set; }

        public string RecordStatus { get; set; }

        public int CreateBy { get; set; }

        public DateTime? CreateDt { get; set; }

        public int? ModifyBy { get; set; }

        public DateTime? ModifyDt { get; set; }

        public int? ApproveBy { get; set; }

        public DateTime? ApproveDt { get; set; }
    }
}
