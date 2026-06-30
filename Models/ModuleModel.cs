namespace Suvarna.DGMS.Api.Models
{
    public class ModuleModel
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

        public string? DocType { get; set; }

        public int? DisplayOrder { get; set; }

        public string? NewPageUrl { get; set; }

        public string? GridPageUrl { get; set; }

        public string? ParentModuleFormCd { get; set; }

        public string? DocFrom { get; set; }

        public long? ParentModuleId { get; set; }

        public string? ModuleFormCd { get; set; }

        public string? ProjectCd { get; set; }
    }
}
