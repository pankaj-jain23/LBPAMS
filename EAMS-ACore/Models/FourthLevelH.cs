﻿using EAMS_ACore.Models.ElectionType;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EAMS_ACore.Models
{
    public class FourthLevelH
    {
        [Key]
        public int FourthLevelHMasterId { get; set; }
        public string? HierarchyName { get; set; }
        public int HierarchyCode { get; set; }

        public string? HierarchyType { get; set; }

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

        public DateTime? HierarchyCreatedAt { get; set; }

        public DateTime? HierarchyUpdatedAt { get; set; }

        public DateTime? HierarchyDeletedAt { get; set; }
        public bool HierarchyStatus { get; set; }
        public string? SecondLanguage { get; set; }
        public string? AssignedTo { get; set; }
        public string? AssignedBy { get; set; }
        public bool IsAssigned { get; set; }


        public virtual List<BoothMaster> BoothMaster
        {
            get;
            set;
        }

        public virtual List<PSZonePanchayat> PSZonePanchayat

        {
            get;
            set;
        }
        public virtual List<GPPanchayatWards> GPPanchayatWards
        {
            get;
            set;
        }
    }
}
