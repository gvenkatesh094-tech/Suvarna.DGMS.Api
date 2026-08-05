namespace Suvarna.DGMS.Api.Models
{
    public class HkQuickStats
    {
        public string avgTaskCompletionTime { get; set; } = "";
        public int roomsCleanedToday { get; set; }
        public int vipCleanings { get; set; }
        public int complaints { get; set; }
    }
}
