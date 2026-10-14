//namespace DGMS.API.Models.HouseKeeping
//{
//    public class HkTaskRequest
//    {
//        public short GrpId { get; set; }

//        public short OrgId { get; set; }

//        public short LocId { get; set; }

//        public int StaffId { get; set; }

//        public string TaskType { get; set; } = string.Empty;
//    }
//}

// NEW model for "Request New Task" (points discussed re: simplified,
// non-targeted request visible to any supervisor on hk-dashboard).
// Place alongside HkTaskHistoryEntry.cs — same namespace/convention.

namespace Suvarna.DGMS.Api.Models
{
    public class HkTaskRequest
    {
        public int requestId { get; set; }
        public int staffUserId { get; set; }
        public string? staffName { get; set; }
        public DateTime requestDt { get; set; }
    }
}
