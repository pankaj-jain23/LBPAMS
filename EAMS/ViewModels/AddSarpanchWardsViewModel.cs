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
      
        public int BoothMasterId
        {
            get;
            set;
        }
        
        public string? SarpanchWardsCategory { get; set; }
        public DateTime? SarpanchWardsCreatedAt { get; set; }

        public DateTime? SarpanchWardsUpdatedAt { get; set; }

        public DateTime? SarpanchWardsDeletedAt { get; set; }
        public bool IsStatus { get; set; }
      
    }
    public class UpdateSarpanchWardsViewModel
    {
        public int SarpanchWardsMasterId { get; set; }
        public int MyProperty { get; set; }
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

        public int BoothMasterId
        {
            get;
            set;
        }

        public string? SarpanchWardsCategory { get; set; }
        public DateTime? SarpanchWardsCreatedAt { get; set; }

        public DateTime? SarpanchWardsUpdatedAt { get; set; }

        public DateTime? SarpanchWardsDeletedAt { get; set; }
        public bool IsStatus { get; set; }
       
    }
}
