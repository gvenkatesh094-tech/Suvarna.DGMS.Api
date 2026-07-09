namespace Suvarna.DGMS.Api.Models
{
    public class OccupantGuest
    {
        public string? mode { get; set; }

        public int occupantCatId { get; set; }

        public int grpId { get; set; }

        public int orgId { get; set; }

        public int locId { get; set; }

        public string? occupantCatCode { get; set; }

        public string? occupantCatName { get; set; }

        public string? billingModelCode { get; set; }

        public string? requirePatientLink { get; set; }

        public string? requireAdmnLink { get; set; }

        public string? autoCheckoutOnDisch { get; set; }

        public string? allowChargeTransfer { get; set; }

        public string? isCorporate { get; set; }

        public string? isNgoSponsored { get; set; }

        public string? isComplimentary { get; set; }

        public string? requireIdVerification { get; set; }

        public short? maxStayDays { get; set; }

        public short? minStayHours { get; set; }

        public string? securityDepositReqd { get; set; }

        public string? gstApplicable { get; set; }

        public string? visitorPassReqd { get; set; }

        public short sortOrder { get; set; }

        public string? remarks { get; set; }

        public string? status { get; set; }

        public int userId { get; set; }
        public int sessionId { get; set; }
        public int discount { get; set; }
  
        
    }
}
