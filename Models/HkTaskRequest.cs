namespace DGMS.API.Models.HouseKeeping
{
    public class HkTaskRequest
    {
        public short GrpId { get; set; }

        public short OrgId { get; set; }

        public short LocId { get; set; }

        public int StaffId { get; set; }

        public string TaskType { get; set; } = string.Empty;
    }
}