using EAMS_ACore.Models.ElectionType;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAMS_ACore.Models
{
    public class PSZone
    {
        [Key]
        public int PSZoneMasterId { get; set; }
        public string PSZoneName { get; set; }
        public int PSZoneCode { get; set; }

        public string? PSZoneType { get; set; }

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
        public int PSZoneBooths { get; set; }
        public string? PSZoneCategory { get; set; }
        public DateTime? PSZoneCreatedAt { get; set; }

        public DateTime? PSZoneUpdatedAt { get; set; }

        public DateTime? PSZoneDeletedAt { get; set; }
        public bool PSZoneStatus { get; set; }
    
        public virtual List<BoothMaster> BoothMaster
        {
            get;
            set;
        }
    }
}
