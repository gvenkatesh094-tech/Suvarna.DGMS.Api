namespace Suvarna.DGMS.Api.Models
{
    public class LookupModel
    {
        public int id { get; set; }

        public string cd { get; set; } = "";
        public string name { get; set; } = "";

        // NEW: occupant category flags (only filled for the occupant category lookup)
        public string? requirePatientLink { get; set; }
        public string? requireAdmnLink { get; set; }
        public string? isCorporate { get; set; }   // NEW: occupant category IS_CORPORATE
        public string? requireIdVerification { get; set; }
        public int? maxStayDays { get; set; }
        public string? isComplimentary { get; set; }
        public string? securityDepositReqd { get; set; }
        public decimal? discountPct { get; set; }
        public string? gstApplicable { get; set; }
    }
}