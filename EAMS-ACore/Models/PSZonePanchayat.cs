using EAMS_ACore.Models.ElectionType;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EAMS_ACore.Models
{
    public class PSZonePanchayat
    {
        [Key]
        public int PSZonePanchayatMasterId { get; set; }
        public string PSZonePanchayatName { get; set; }
        public int PSZonePanchayatCode { get; set; }

        public string? PSZonePanchayatType { get; set; }

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

        public int PSZonePanchayatBooths { get; set; }
        public string? PSZonePanchayatCategory { get; set; }
        public DateTime? PSZonePanchayatCreatedAt { get; set; }

        public DateTime? PSZonePanchayatUpdatedAt { get; set; }

        public DateTime? PSZonePanchayatDeletedAt { get; set; }
        public bool PSZonePanchayatStatus { get; set; }

        public virtual List<BoothMaster> BoothMaster
        {
            get;
            set;
        }
       
    }
}
