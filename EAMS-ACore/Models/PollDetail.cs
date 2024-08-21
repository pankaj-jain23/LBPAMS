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
        public int EventMasterId { get; set; }
        public int VotesPolled { get; set; }
        public int AddedBy { get; set; }
        public DateTime? VotesPolledRecivedTime { get; set; }
        public int? Male { get; set; }
        public int? Female { get; set; }
        public int? Transgender { get; set; }
        public int? PCMasterId { get; set; }
        public string UserType { get; set; }
        public int ElectionTypeMasterId { get; set; }

    }
}
