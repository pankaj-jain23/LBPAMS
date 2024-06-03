namespace EAMS_ACore.Models
{
    public class EventWiseBoothStatus
    {
        public int EventMasterId { get; set; }
        public string EventName { get; set; }
        public int TotalBooths { get; set; }
        public int Completed { get; set; }
        public int Pending { get; set; }
    }
}
