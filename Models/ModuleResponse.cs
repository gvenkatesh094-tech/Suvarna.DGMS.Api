namespace Suvarna.DGMS.Api.Models
{
    public class ModuleResponse
    {
        public long UserId { get; set; }

        public string? DocIdType { get; set; }

        public long? DocId { get; set; }

        public string? DocDesc { get; set; }

        public string? PageUrl { get; set; }

        public string? ParentDocIdType { get; set; }

        public long? ParentDocId { get; set; }

        public string? ParentDocDesc { get; set; }

        public long? LocId { get; set; }

        public int? Priority { get; set; }

        public string? IsSecurityCd { get; set; }

        public string? DocType { get; set; }

        public int? DisplayOrder { get; set; }

        public string? DocRptFormat { get; set; }

        public string? ReportType { get; set; }

        public string? NewPageUrl { get; set; }

        public string? GridPageUrl { get; set; }

        public string? IsMsgStation { get; set; }

        public string? ParentModuleFormCd { get; set; }

        public string? DocFrom { get; set; }

        public long? ParentModuleId { get; set; }

        public string? IsTokenSystem { get; set; }

        public string? IsMakerCheckr { get; set; }

        public string? IsShiftLogRequired { get; set; }

        public string? IsPaymentGatewayAllowed { get; set; }

        public int? DocOrder { get; set; }

        public int? DisOrder { get; set; }

        public long? ParDocId { get; set; }

        public string? ParDocCd { get; set; }

        public string? ParDocName { get; set; }

        public string? DocFormCd { get; set; }

        public string? ModuleFormCd { get; set; }

        public string? ParDocFormCd { get; set; }

        public string? ProjectCd { get; set; }
    }
}
