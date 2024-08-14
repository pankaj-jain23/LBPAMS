using EAMS_ACore.Models.ElectionType;
using EAMS_ACore;
using System.ComponentModel.DataAnnotations.Schema;
using EAMS_ACore.Models;

namespace LBPAMS.ViewModels
{
    public class AddBlockPanchayatViewModel
    {

        public string BlockZonePanchayatName { get; set; }
        public int BlockZonePanchayatCode { get; set; }

        public string? BlockZonePanchayatType { get; set; }

        public int ElectionTypeMasterId
        {
            get;
            set;
        }


        public int StateMasterId
        {
            get;
            set;
        }


        public int DistrictMasterId
        {
            get;
            set;
        }


        public int AssemblyMasterId
        {
            get;
            set;
        }

        public int FourthLevelHMasterId
        {
            get;
            set;
        }


        public int BlockZonePanchayatBooths { get; set; }
        public string? BlockZonePanchayatCategory { get; set; }
        public DateTime? BlockZonePanchayatCreatedAt { get; set; }

        public DateTime? BlockZonePanchayatUpdatedAt { get; set; }

        public DateTime? BlockZonePanchayatDeletedAt { get; set; }

        public bool IsStatus { get; set; }


    }
    public class UpdateBlockPanchayatViewModel
    {
        public int BlockZonePanchayatMasterId { get; set; }
        public string BlockZonePanchayatName { get; set; }
        public int BlockZonePanchayatCode { get; set; }

        public string? BlockZonePanchayatType { get; set; }

        public int ElectionTypeMasterId
        {
            get;
            set;
        }


        public int StateMasterId
        {
            get;
            set;
        }


        public int DistrictMasterId
        {
            get;
            set;
        }


        public int AssemblyMasterId
        {
            get;
            set;
        }

        public int FourthLevelHMasterId
        {
            get;
            set;
        }


        public int BlockZonePanchayatBooths { get; set; }
        public string? BlockZonePanchayatCategory { get; set; }
        public DateTime? BlockZonePanchayatCreatedAt { get; set; }

        public DateTime? BlockZonePanchayatUpdatedAt { get; set; }

        public DateTime? BlockZonePanchayatDeletedAt { get; set; }

        public bool IsStatus { get; set; }

    }

    public class ListBlockPanchayatViewModel
    {
        public int BlockZonePanchayatMasterId { get; set; }
        public string BlockZonePanchayatName { get; set; }
        public int BlockZonePanchayatCode { get; set; }

        public string? BlockZonePanchayatType { get; set; }

        public int ElectionTypeMasterId
        {
            get;
            set;
        }
        public string ElectionTypeName { get; set; }

        public int StateMasterId
        {
            get;
            set;
        }
        public string StateName { get; set; }

        public int DistrictMasterId
        {
            get;
            set;
        }

        public string DistrictName { get; set; }
        public int AssemblyMasterId
        {
            get;
            set;
        }
        public string AssemblyName { get; set; }

        public int FourthLevelHMasterId
        {
            get;
            set;
        }
        public string FourthLevelHName { get; set; }

        public int BlockZonePanchayatBooths { get; set; }
        public string? BlockZonePanchayatCategory { get; set; }
        
        public bool IsStatus { get; set; }
    }
}
