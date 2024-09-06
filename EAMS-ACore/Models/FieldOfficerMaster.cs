using System.ComponentModel.DataAnnotations;

namespace EAMS_ACore.Models
{
    public class FieldOfficerMaster
    {
        [Key]
        public int FieldOfficerMasterId { get; set; }
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

        public int? AssemblyMasterId
        {
            get;
            set;
        }

        public string FieldOfficerName { get; set; }

        public string FieldOfficerDesignation { get; set; }

        public string FieldOfficerOfficeName { get; set; }

        public string FieldOfficerMobile { get; set; }

        public DateTime? FieldOfficerCreatedAt { get; set; }

        public DateTime? FieldOfficerUpdatedAt { get; set; }
        public bool FieldOfficerStatus { get; set; }

        public DateTime? OTPGeneratedTime { get; set; }
        public string? OTP { get; set; }
        public DateTime? OTPExpireTime { get; set; }
        public int OTPAttempts { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime RefreshTokenExpiryTime { get; set; }
        public int AppPin { get; set; }
        public bool IsLocked { get; set; }
        public int ElectionTypeMasterId { get; set; }



    }


    public class SectorOfficerMasterCustom
    {
        public int StateMasterId { get; set; }
        public string StateName { get; set; }
        public int DistrictMasterId { get; set; }
        public string SoOfficeName { get; set; }
        public string SoDesignation { get; set; }
        public string DistrictName { get; set; }
        public bool DistrictStatus { get; set; }
        public string DistrictCode { get; set; }
        public int AssemblyMasterId { get; set; }
        public string AssemblyName { get; set; }
        public int AssemblyCode { get; set; }
        public int SoMasterId { get; set; }
        public string SoName { get; set; }
        public string SoMobile { get; set; }
        public bool IsStatus { get; set; }
        public int ElectionTypeMasterId { get; set; }
        public string ElectionTypeName { get; set; }
    }



    public class BLOOfficerCustom
    {
        public int StateMasterId { get; set; }
        public string StateName { get; set; }
        public int DistrictMasterId { get; set; }
        public string DistrictName { get; set; }


        public int AssemblyMasterId { get; set; }
        public string AssemblyName { get; set; }
        public int AssemblyCode { get; set; }
        public int bloMasterId { get; set; }
        public string bloName { get; set; }
        public string bloMobile { get; set; }
        public bool IsStatus { get; set; }
    }


}

