using System.ComponentModel.DataAnnotations;

namespace EAMS_ACore.Models
{
    public class PollDetail
    {
        [Key]
        public int PollDetailMasterId { get; set; }
        public int? SlotManagementId { get; set; }
        public int StateMasterId { get; set; }
        public int DistrictMasterId { get; set; }
        public int AssemblyMasterId { get; set; }
        public int BoothMasterId { get; set; }
        public int? VotesPolled { get; set; }
        public DateTime? VotesPolledRecivedTime { get; set; }
        public int ElectionTypeMasterId { get; set; } 
        public int EventMasterId { get; set; }
        public string EventABBR { get; set; }
        public string EventName { get; set; }
        public int EventSequence { get; set; }
        public bool EventStatus { get; set; }
    }
}
