namespace LBPAMS.ViewModels
{
    public class ResultListViewModel
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
        public string? FourthLevelHName { get; set; }

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
}
