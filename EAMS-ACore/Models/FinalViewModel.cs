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
        public int? TotalAvailableMale { get; set; }
        public int? TotalAvailableFemale { get; set; }
        public int? TotalAvailableTransgender { get; set; } 
        public int? Male { get; set; }
        public int? Female { get; set; }
        public int? Transgender { get; set; }
        public int? edc { get; set; }
        public int ElectionTypeMasterId { get; set; }


    }
}
