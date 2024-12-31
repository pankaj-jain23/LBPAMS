using System.Text.Json.Serialization;

namespace EAMS_ACore.Models
{
    public class VoterTurnOutPolledDetailViewModel
    {
        public int BoothMasterId { get; set; }
        public int StateMasterId { get; set; }
        public int ElectionTypeMasterId { get; set; }
        public int EventMasterId { get; set; }
        public string EventABBR { get; set; }
        public string EventName { get; set; }
        public int EventSequence { get; set; }
        public bool EventStatus { get; set; }
        public int? TotalVoters { get; set; }
        public int? VotesPolled { get; set; }
        public DateTime? VotesPolledRecivedTime { get; set; }
        public TimeOnly? StartTime { get; set; }
        public TimeOnly? EndTime { get; set; }
        public TimeOnly? LockTime { get; set; }
        public bool IsLastSlot { get; set; }
        public bool IsSlotAvailable { get; set; }
        public string? Message { get; set; }
      
        
    }
}
