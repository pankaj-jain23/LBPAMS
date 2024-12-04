using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAMS_ACore.Models
{
    public class AROResultMasterList
    {
        public int AROMasterId { get; set; }
        public int StateMasterId
        {
            get;
            set;
        }
        public string StateName
        {
            get;
            set;
        }
        public int DistrictMasterId
        {
            get;
            set;
        }
        public string DistrictName
        {
            get;
            set;
        }
        public int? AssemblyMasterId
        {
            get;
            set;
        }
        public string AssemblyName
        {
            get;
            set;
        }
        public int? FourthLevelHMasterId
        {
            get;
            set;
        }
        public string? HierarchyName { get; set; }

        public int AssemblyCode { get; set; }
        public string AROName { get; set; }
        public string ARODesignation { get; set; }
        public string AROOfficeName { get; set; }
        public string AROMobile { get; set; }
        public bool IsStatus { get; set; }
        public DateTime? OTPGeneratedTime { get; set; }
        public string? OTP { get; set; }
        public DateTime? OTPExpireTime { get; set; }
        public int OTPAttempts { get; set; }
        public bool IsLocked { get; set; }
        public int ElectionTypeMasterId { get; set; }
        public string ElectionTypeName { get; set; }
    }

    public class IsRDProfileUpdated
    {
        public int? AROMasterId { get; set; }
        public bool IsProfileUpdated { get; set; }

    }
}
