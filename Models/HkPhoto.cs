namespace Suvarna.DGMS.Api.Models
{
    public class HkPhoto
    {
        public short grpId { get; set; }

        public short orgId { get; set; }

        public short locId { get; set; }

        public int taskId { get; set; }

        public string? photoPath { get; set; }

        public string? photoName { get; set; }

        public int fileSize { get; set; }

        public string? fileType { get; set; }

        public string? notes { get; set; }

        public int userId { get; set; }
    }
}
