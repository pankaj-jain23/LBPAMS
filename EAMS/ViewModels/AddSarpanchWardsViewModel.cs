using EAMS_ACore.Models.ElectionType;
using EAMS_ACore;
using System.ComponentModel.DataAnnotations.Schema;

namespace LBPAMS.ViewModels
{
    public class AddSarpanchWardsViewModel
    {
        public string SarpanchWardsName { get; set; }
        public int SarpanchWardsCode { get; set; }

        public string? SarpanchWardsType { get; set; }

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
        public int? FourthLevelHMasterId
        {
            get;
            set;
        }
        public int? BlockZonePanchayatMasterId 
        { 
            get;
            set; 
        }

        public string? SarpanchWardsCategory { get; set; }
       
        public bool IsStatus { get; set; }
      
    }
    public class UpdateSarpanchWardsViewModel
    {
        public int SarpanchWardsMasterId { get; set; }
     
        public string SarpanchWardsName { get; set; }
        public int SarpanchWardsCode { get; set; }

        public string? SarpanchWardsType { get; set; }

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


        public int? FourthLevelHMasterId
        {
            get;
            set;
        }
        public int? BlockZonePanchayatMasterId
        {
            get;
            set;
        }
        public string? SarpanchWardsCategory { get; set; }
       
        public bool IsStatus { get; set; }
       
    }

    public class ListSarpanchWardsViewModel {
        public int SarpanchWardsMasterId { get; set; }
        public string SarpanchWardsName { get; set; }
        public int SarpanchWardsCode { get; set; }

        public string? SarpanchWardsType { get; set; }

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
        public int? FourthLevelHMasterId
        {
            get;
            set;
        }
        public string FourthLevelHName { get; set; }
        public int? BlockZonePanchayatMasterId
        {
            get;
            set;
        }
        public string BlockZonePanchayatName { get; set; }
        public string? SarpanchWardsCategory { get; set; }

        public bool IsStatus { get; set; }
    }
}
