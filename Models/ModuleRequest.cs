namespace Suvarna.DGMS.Api.Models
{
    public class ModuleRequest
    {
        public long UserId { get; set; }

        public int SessionId { get; set; }

        public int ParentModuleId { get; set; }
    }
}
