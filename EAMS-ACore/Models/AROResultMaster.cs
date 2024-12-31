using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAMS_ACore.Models
{
    public class AROResultMaster
    {
        [Key]
        public int AROMasterId { get; set; }
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
        public string AROName { get; set; }

        public string ARODesignation { get; set; }

        public string AROOfficeName { get; set; }

        public string AROMobile { get; set; }

        public DateTime? AROCreatedAt { get; set; }

        public DateTime? AROUpdatedAt { get; set; }
        public bool IsStatus { get; set; }

        public DateTime? OTPGeneratedTime { get; set; }
        public string? OTP { get; set; }
        public DateTime? OTPExpireTime { get; set; }
        public int OTPAttempts { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime RefreshTokenExpiryTime { get; set; }
        public int AppPin { get; set; }
        public bool IsLocked { get; set; }
        public int ElectionTypeMasterId { get; set; }
        public string? ROUserId { get; set; }
    }
}
