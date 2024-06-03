using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EAMS_ACore.Models.CountingDayModels
{
    public class CountingLocationMaster
    {
        [Key]
        public int LocationMasterId { get; set; }
        public int StateMasterId { get; set; }
        [ForeignKey("StateMasterId")]
        public virtual StateMaster StateMaster { get; set; }
        public int? DistrictMasterId { get; set; }
        [ForeignKey("DistrictMasterId")]
        public virtual DistrictMaster DistrictMaster { get; set; }

        public int? PcMasterId { get; set; }
        [ForeignKey("PcMasterId")]
        public virtual ParliamentConstituencyMaster ParliamentConstituencyMaster { get; set; }
        public string UserId { get; set; }
        public int LocationName { get; set; }
        public bool CountingLocationStatus { get; set; }
    }
}
