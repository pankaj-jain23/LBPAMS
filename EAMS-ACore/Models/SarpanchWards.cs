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
    public class SarpanchWards
    {
        [Key]
        public int SarpanchWardsMasterId { get; set; }
        public string SarpanchWardsName { get; set; }
        public int SarpanchWardsCode { get; set; }

        public string? SarpanchWardsType { get; set; }

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

        public int BlockZonePanchayatMasterId
        {
            get;
            set;
        }
        [ForeignKey("BlockZonePanchayatMasterId")]
        public virtual BlockZonePanchayat? BlockZonePanchayat

        {
            get;
            set;
        }
        public string? SarpanchWardsCategory { get; set; }
        public DateTime? SarpanchWardsCreatedAt { get; set; }

        public DateTime? SarpanchWardsUpdatedAt { get; set; }

        public DateTime? SarpanchWardsDeletedAt { get; set; }
        public bool SarpanchWardsStatus { get; set; }


    }
}
