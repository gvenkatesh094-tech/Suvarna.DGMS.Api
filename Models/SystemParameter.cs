using System;

namespace Suvarna.DGMS.Api.Models
{
    public class SystemParameter
    {
        public string? mode { get; set; }

        public int dgmSysParamId { get; set; }

        public int grpId { get; set; }

        public int orgId { get; set; }

        public int locId { get; set; }

        public string? sessionId { get; set; }

        public int userId { get; set; }

        public string? paramCd { get; set; }

        public string? paramNm { get; set; }

        public string? paramValue { get; set; }

        public string? paramCategory { get; set; }

        public string? dataType { get; set; }

        public string? remarks { get; set; }

        public string? recordStatus { get; set; }

        public int createBy { get; set; }

        public DateTime? createDt { get; set; }

        public int modifyBy { get; set; }

        public DateTime? modifyDt { get; set; }
    }
}