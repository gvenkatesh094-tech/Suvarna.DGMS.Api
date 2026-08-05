namespace DGMS.API.Models.HouseKeeping
{
    public class HkDashboardResponse
    {
        public int Pending { get; set; }

        public int InProgress { get; set; }

        public int Ready { get; set; }

        public int Critical { get; set; }
    }
    
}