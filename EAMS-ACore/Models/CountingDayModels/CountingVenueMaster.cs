using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EAMS_ACore.Models.CountingDayModels
{
    public class CountingVenueMaster
    {
        [Key]
        public int CountingVenueMasterId { get; set; }
        public int LocationMasterId { get; set; }
        [ForeignKey("LocationMasterId")]
        public virtual CountingLocationMaster CountingLocationMaster { get; set; }
        public string CountingVenue { get; set; }
        public bool CountingVenueStatus { get; set; }
    }
}
