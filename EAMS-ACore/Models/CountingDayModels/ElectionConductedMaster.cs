using EAMS_ACore.Models.ElectionType;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EAMS_ACore.Models.CountingDayModels
{
    public class ElectionConductedMaster
    {
        [Key]
        public int ElectionConductedMasterId { get; set; }
        public int ElectionTypeMasterId { get; set; }
        [ForeignKey("ElectionTypeMasterId")]
        public virtual ElectionTypeMaster ElectionTypeMaster { get; set; }
        public int StateMasterId { get; set; }
        public bool ElectionConductedStatus { get; set; }
    }
}
