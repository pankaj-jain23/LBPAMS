using EAMS_ACore.Models.ElectionType;
using EAMS_ACore;
using System.ComponentModel.DataAnnotations.Schema;

namespace LBPAMS.ViewModels
{
    public class AddBlockPanchayatViewModel
    {
        public string BlockPanchayatName { get; set; }
        public int BlockPanchayatCode { get; set; }

        public string? BlockPanchayatType { get; set; }

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
        public int BlockPanchayatBooths { get; set; }
        public string? BlockPanchayatCategory { get; set; }
        public DateTime? BlockPanchayatCreatedAt { get; set; }

        public DateTime? BlockPanchayatUpdatedAt { get; set; }

        public DateTime? BlockPanchayatDeletedAt { get; set; }
        public bool IsStatus { get; set; }

    }
    public class UpdateBlockPanchayatViewModel
    {
        public int BlockPanchayatMasterId { get; set; }
        public string BlockPanchayatName { get; set; }
        public int BlockPanchayatCode { get; set; }

        public string? BlockPanchayatType { get; set; }

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
        public int BlockPanchayatBooths { get; set; }
        public string? BlockPanchayatCategory { get; set; }
        public DateTime? BlockPanchayatCreatedAt { get; set; }

        public DateTime? BlockPanchayatUpdatedAt { get; set; }

        public DateTime? BlockPanchayatDeletedAt { get; set; }
        public bool IsStatus { get; set; }


    }
}
