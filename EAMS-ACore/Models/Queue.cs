namespace EAMS_ACore.Models
{
    public class Queue
    {

        public int BoothMasterId { get; set; }
        public int? TotalVoters { get; set; }
        public int? VotesPolled { get; set; }
        public DateTime? VotesPolledTime { get; set; }
        public int? RemainingVotes { get; set; }
        public bool? VoteEnabled { get; set; }
        public string? Message { get; set; }

        public int ElectionTypeMasterId { get; set; }
    }
}
