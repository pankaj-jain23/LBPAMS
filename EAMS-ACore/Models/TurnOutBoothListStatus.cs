namespace EAMS_ACore.Models
{
    public class TurnOutBoothListStatus
    {
        public int StateMasterId { get; set; }
        public int DistrictMasterId { get; set; }
        public int AssemblyMasterId { get; set; }
        public int BoothMasterId { get; set; }
        public string BoothCode { get; set; }
        public int EventMasterId { get; set; }
        public string EventName { get; set; }
        public string BoothName { get; set; }
        public string? Color { get; set; }
        public int? SortColor { get; set; }
        public bool? UpdateStatus { get; set; }
    }
}