namespace DGMS.API.Models.HouseKeeping
{
    public class PhotoUploadRequest
    {
        public short GrpId { get; set; }

        public short OrgId { get; set; }

        public short LocId { get; set; }

        public int TaskId { get; set; }

        public string PhotoPath { get; set; } = string.Empty;

        public string PhotoName { get; set; } = string.Empty;

        public int FileSize { get; set; }

        public string FileType { get; set; } = string.Empty;

        public string Notes { get; set; } = string.Empty;

        public int UserId { get; set; }
    }
}