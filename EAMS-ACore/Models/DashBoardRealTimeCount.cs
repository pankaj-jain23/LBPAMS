namespace EAMS_ACore.Models
{
    public class DashBoardRealTimeCount
    {
        public int Total { get; set; }
        public List<EventCount> Events { get; set; }
    }

    public class EventCount
    {
        public string EventName { get; set; }
        public string EventAbbrName { get; set; }
        
        public int? VotesPolledCount { get; set; }
        public int? VotesInQueueCount { get; set; }
        public int? TotalVotersCount { get; set; }
        public decimal? VotesPolledPercentage { get; set; }
        public int? FinalVotesCount { get; set; }
        public decimal? FinalVotesPercentage { get; set; }

        public int? Count { get; set; }
        public int? TotalWinnerCandidate {  get; set; }
        public int? TotalUnOpposedCandidate { get;set; }
        public int? TotalFourthLevel { get;set; }
    }

    public class DahboardMastersId
    {
        public string? ElectionMasterId { get; set; }
        public string? StateMasterId { get; set; }
        public string? DistrictMasterId { get; set; }
        public string? PCMasterId { get; set; }
        public string? AssemblyMasterId { get; set; }
        public string? BoothMasterId { get; set; }
    }
}
