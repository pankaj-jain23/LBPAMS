using EAMS_ACore.Models.ElectionType;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EAMS_ACore.Models
{
    public class BlockPanchayat
    {
        [Key]
        public int BlockPanchayatMasterId { get; set; }
        public string BlockPanchayatName { get; set; }
        public int BlockPanchayatCode { get; set; }

        public string? BlockPanchayatType { get; set; }

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
        public int BlockPanchayatBooths { get; set; }
        public string? BlockPanchayatCategory { get; set; }
        public DateTime? BlockPanchayatCreatedAt { get; set; }

        public DateTime? BlockPanchayatUpdatedAt { get; set; }

        public DateTime? BlockPanchayatDeletedAt { get; set; }
        public bool BlockPanchayatStatus { get; set; }

        public virtual List<BoothMaster> BoothMaster
        {
            get;
            set;
        }
    }
}
