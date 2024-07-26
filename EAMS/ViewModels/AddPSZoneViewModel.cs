using EAMS_ACore.Models.ElectionType;
using EAMS_ACore;
using System.ComponentModel.DataAnnotations.Schema;

namespace LBPAMS.ViewModels
{
    public class AddPSZoneViewModel
    {
        public string PSZoneName { get; set; }
        public int PSZoneCode { get; set; }

        public string? PSZoneType { get; set; }

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

        public int? PCMasterId
        {
            get;
            set;
        }

        public int AssemblyMasterId
        {
            get;
            set;
        }

        public int PSZoneBooths { get; set; }
        public string? PSZoneCategory { get; set; }
        public DateTime? PSZoneCreatedAt { get; set; }

        public DateTime? PSZoneUpdatedAt { get; set; }

        public DateTime? PSZoneDeletedAt { get; set; }
        public bool IsStatus { get; set; }
      
    }
    public class UpdatePSZoneViewModel
    {
        public int PSZoneMasterId { get; set; }
        public string PSZoneName { get; set; }
        public int PSZoneCode { get; set; }

        public string? PSZoneType { get; set; }

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

        public int? PCMasterId
        {
            get;
            set;
        }

        public int AssemblyMasterId
        {
            get;
            set;
        }

        public int PSZoneBooths { get; set; }
        public string? PSZoneCategory { get; set; }
        public DateTime? PSZoneCreatedAt { get; set; }

        public DateTime? PSZoneUpdatedAt { get; set; }

        public DateTime? PSZoneDeletedAt { get; set; }
        public bool IsStatus { get; set; }
      
    }
}
