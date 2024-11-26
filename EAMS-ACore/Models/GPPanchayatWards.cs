using EAMS_ACore.Models.ElectionType;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EAMS_ACore.Models
{
    public class GPPanchayatWards
    {
        [Key]
        public int GPPanchayatWardsMasterId { get; set; }
        public string GPPanchayatWardsName { get; set; }
        public int GPPanchayatWardsCode { get; set; }

        public string? GPPanchayatWardsType { get; set; }

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

        public int AssemblyMasterId
        {
            get;
            set;
        }
        [ForeignKey("AssemblyMasterId")]
        public virtual AssemblyMaster? AssemblyMaster
        {
            get;
            set;
        }



        public int FourthLevelHMasterId

        {
            get;
            set;
        }


        [ForeignKey("FourthLevelHMasterId")]
        public virtual FourthLevelH? FourthLevelH
        {
            get;
            set;
        }

        
        public string? GPPanchayatWardsCategory { get; set; }
        public DateTime? GPPanchayatWardsCreatedAt { get; set; }

        public DateTime? GPPanchayatWardsUpdatedAt { get; set; }

        public DateTime? GPPanchayatWardsDeletedAt { get; set; }
        public bool GPPanchayatWardsStatus { get; set; }
        public bool IsCC { get; set; }
        public bool IsNN { get; set; }
        public int? Male { get; set; }
        public int? Female { get; set; }
        public int? Transgender { get; set; }
        public int? TotalVoters { get; set; }
    }
}
