namespace Suvarna.DGMS.Api.Models
{
    public class ModuleDocResponse
    {
        public int ModuleId { get; set; }

        public int ParentModuleId { get; set; }

        public int DocId { get; set; }

        public string DocCd { get; set; }

        public string DocName { get; set; }

        public string ModuleName { get; set; }

        public string PageUrl { get; set; }

        public string NewPageUrl { get; set; }

        public string AccessAdd { get; set; }

        public string AccessMod { get; set; }

        public string AccessDel { get; set; }

        public string AccessQry { get; set; }

        public string AccessApp { get; set; }

        public string AccessExe { get; set; }

        public string RecordStatus { get; set; }

        public int DisplayOrder { get; set; }
    }
}
