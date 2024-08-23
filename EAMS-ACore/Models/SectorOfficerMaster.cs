using System.ComponentModel.DataAnnotations;

namespace EAMS_ACore.Models
{
    public class SectorOfficerMaster
    {
        [Key]
        public int SOMasterId { get; set; }
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
        public int? FourthLevelHMasterId
        {
            get;
            set;
        }
        public string SoName { get; set; }

        public string SoDesignation { get; set; }

        public string SoOfficeName { get; set; }
       
        public string SoMobile { get; set; }

        public DateTime? SoCreatedAt { get; set; }

        public DateTime? SOUpdatedAt { get; set; }
        public bool SoStatus { get; set; }

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

