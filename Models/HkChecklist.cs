namespace Suvarna.DGMS.Api.Models
{
    public class HkChecklist
    {
        public int TaskId { get; set; }

        public int ChecklistId { get; set; }

        public string IsDone { get; set; } = "N";

        public int UserId { get; set; }
    }
}
