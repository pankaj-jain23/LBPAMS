namespace EAMS_ACore.Models
{
    public class FinalViewModel
    {
        public int BoothMasterId { get; set; }
        public int? TotalVoters { get; set; }
        public int? LastVotesPolled { get; set; }
        public int? LastFinalVotesPolled { get; set; }
        public DateTime? VotesFinalPolledTime { get; set; }
        public bool? VoteEnabled { get; set; }
        public string? Message { get; set; }
        public string? TotalAvailableMale { get; set; }
        public string? TotalAvailableFemale { get; set; }
        public string? TotalAvailableTransgender { get; set; } 
        public string? Male { get; set; }
        public string? Female { get; set; }
        public string? Transgender { get; set; }
        public string? edc { get; set; }
        public int ElectionTypeMasterId { get; set; }


    }
}
