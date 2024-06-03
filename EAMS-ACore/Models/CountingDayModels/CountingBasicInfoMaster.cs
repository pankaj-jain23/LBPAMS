using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EAMS_ACore.Models.CountingDayModels
{
    public class CountingBasicInfoMaster
    {
        [Key]
        public int CountingBasicInfoMasterId { get; set; }
        public int ElectionConductedMasterId { get; set; }
        [ForeignKey("ElectionConductedMasterId")]
        public virtual ElectionConductedMaster ElectionConductedMaster { get; set; }
        public int CountingTypeMasterId { get; set; }
        [ForeignKey("CountingTypeMasterId")]
        public virtual CountingTypeMaster CountingTypeMaster { get; set; }
        public int? DistrictMasterId { get; set; }
        [ForeignKey("DistrictMasterId")]
        public virtual DistrictMaster DistrictMaster { get; set; }
        public int? PcMasterId { get; set; }
        [ForeignKey("PcMasterId")]
        public virtual ParliamentConstituencyMaster ParliamentConstituencyMaster { get; set; }
        public int AssemblyMasterId { get; set; }
        [ForeignKey("AssemblyMasterId")]
        public virtual AssemblyMaster AssemblyMaster { get; set; }
        public int NoOfTable { get; set; }
        public int NoOfRound { get; set; }
        public int TotalBooth { get; set; }
        public bool CountingBasicInfoStatus { get; set; }
    }
}
