namespace DGMS.API.Models.HouseKeeping
{
    public class HkTaskResponse
    {
        public int TaskId { get; set; }

        public string TaskNo { get; set; } = string.Empty;

        public int BookingId { get; set; }

        public int RoomId { get; set; }

        public string RoomNo { get; set; } = string.Empty;

        public string TaskType { get; set; } = string.Empty;

        public string Priority { get; set; } = string.Empty;

        public string Status { get; set; } = string.Empty;

        public int Triggered { get; set; }

        public int SLA { get; set; }

        public int TotalItems { get; set; }

        public int CompletedItems { get; set; }

        public int StaffId { get; set; }
    }
}