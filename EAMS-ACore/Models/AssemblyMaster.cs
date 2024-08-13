using EAMS_ACore.Models;
using EAMS_ACore.Models.ElectionType;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace EAMS_ACore
{
    public class AssemblyMaster
    {
        [Key]
        public int AssemblyMasterId { get; set; }
        public string AssemblyName { get; set; }
        public int AssemblyCode { get; set; }

        public string? AssemblyType { get; set; }

        public int ElectionTypeMasterId
        {
            get;
            set;
        }
        [ForeignKey("ElectionTypeMasterId")]
        public virtual ElectionTypeMaster? ElectionTypeMaster
        {
            get;
            set;
        }

        public int StateMasterId
        {
            get;
            set;
        }
        [ForeignKey("StateMasterId")]
        public virtual StateMaster? StateMaster
        {
            get;
            set;
        }

        public int DistrictMasterId
        {
            get;
            set;
        }
        [ForeignKey("DistrictMasterId")]
        public virtual DistrictMaster? DistrictMaster
        {
            get;
            set;
        }
        public int? PCMasterId
        {
            get;
            set;
        }
        [ForeignKey("PCMasterId")]
        public virtual ParliamentConstituencyMaster? ParliamentConstituencyMaster
        {
            get;
            set;
        }

        public int? TotalBooths { get; set; }
        public DateTime? AssemblyCreatedAt { get; set; }

        public DateTime? AssemblyUpdatedAt { get; set; }

        public DateTime? AssemblyDeletedAt { get; set; }
        public bool AssemblyStatus { get; set; }
        public string? SecondLanguage { get; set; }
        public virtual List<BoothMaster> BoothMaster
        {
            get;
            set;
        }
        public virtual List<FourthLevelH> FourthLevelH
        {
            get;
            set;
        }

        public virtual List<BlockZonePanchayat> BlockPanchayat
        {
            get;
            set;
        }
 
    }
}
