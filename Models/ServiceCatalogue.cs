using System;

namespace Suvarna.DGMS.Api.Models
{
    public class ServiceCatalogue
    {
        public string? mode { get; set; }

        public int dgmServiceMastId { get; set; }

        public int grpId { get; set; }

        public int orgId { get; set; }

        public int locId { get; set; }

        public string? sessionId { get; set; }

        public int userId { get; set; }

        public string? serviceCd { get; set; }

        public string? serviceNm { get; set; }

        public string? serviceCategory { get; set; }

        public int propertyId { get; set; }

        public string? unitOfMeasure { get; set; }

        public decimal unitRateAmt { get; set; }

        public decimal gstPct { get; set; }

        public string? sacCode { get; set; }

        public string? isGstInclusive { get; set; }

        public string? isOptional { get; set; }

        public string? isAutoPost { get; set; }

        public int deptId { get; set; }

        public string? costCenterCd { get; set; }

        public short sortOrder { get; set; }

        public string? remarks { get; set; }

        public string? recordStatus { get; set; }

        public int createBy { get; set; }

        public DateTime? createDt { get; set; }

        public int modifyBy { get; set; }

        public DateTime? modifyDt { get; set; }
    }
}