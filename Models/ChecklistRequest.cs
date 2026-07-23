namespace DGMS.API.Models.HouseKeeping
{
    public class ChecklistRequest
    {
        public int TaskId { get; set; }

        public int ChecklistId { get; set; }

        public string IsDone { get; set; } = "N";

        public int UserId { get; set; }
    }
}