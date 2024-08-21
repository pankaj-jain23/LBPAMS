using EAMS_ACore.Models.ElectionType;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EAMS_ACore.Models
{
    public class BlockZonePanchayat
    {
        [Key]
        public int BlockZonePanchayatMasterId { get; set; }
        public string BlockZonePanchayatName { get; set; }
        public int BlockZonePanchayatCode { get; set; }

        public string? BlockZonePanchayatType { get; set; }

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

        public int BlockZonePanchayatBooths { get; set; }
        public string? BlockZonePanchayatCategory { get; set; }
        public DateTime? BlockZonePanchayatCreatedAt { get; set; }

        public DateTime? BlockZonePanchayatUpdatedAt { get; set; }

        public DateTime? BlockZonePanchayatDeletedAt { get; set; }
        public bool BlockZonePanchayatStatus { get; set; }

        public virtual List<BoothMaster> BoothMaster
        {
            get;
            set;
        }
        public virtual List<SarpanchWards> SarpanchWards
        {
            get;
            set;
        }
    }
}
