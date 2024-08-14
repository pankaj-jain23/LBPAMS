using EAMS_ACore.Models.ElectionType;
using EAMS_ACore;
using System.ComponentModel.DataAnnotations.Schema;

namespace LBPAMS.ViewModels
{
    public class AddFourthLevelHViewModel
    {
        public string HierarchyName { get; set; }
        public int HierarchyCode { get; set; }

        public string? HierarchyType { get; set; }

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


        public string? HierarchyCategory { get; set; }
        public DateTime? HierarchyCreatedAt { get; set; }

        public DateTime? HierarchyUpdatedAt { get; set; }

        public DateTime? HierarchyDeletedAt { get; set; }
        public bool IsStatus { get; set; }

    }

    public class UpdateFourthLevelHViewModel
    {
        public int FourthLevelHMasterId { get; set; }
        public string HierarchyName { get; set; }
        public int HierarchyCode { get; set; }

        public string? HierarchyType { get; set; }

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


        public string? HierarchyCategory { get; set; }
        public DateTime? HierarchyCreatedAt { get; set; }

        public DateTime? HierarchyUpdatedAt { get; set; }

        public DateTime? HierarchyDeletedAt { get; set; }
        public bool IsStatus { get; set; }

    }

    public class ListFourthLevelHViewModel
    {
        public int FourthLevelHMasterId { get; set; }
        public string HierarchyName { get; set; }
        public int HierarchyCode { get; set; }

        public string? HierarchyType { get; set; }

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
        


        public string? HierarchyCategory { get; set; }
       
        public bool IsStatus { get; set; }
    }
}
