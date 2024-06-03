namespace EAMS_ACore.Models
{
    public class VotesPolledPercentage
    {
        public int? TotalVoters { get; set; }
        public int? VotesPolled { get; set; }
        public string? PollPercentage { get; set; }
        public int? FinalVote { get; set; }
        public string? FinalVotePercentage { get; set; }
    }
}
