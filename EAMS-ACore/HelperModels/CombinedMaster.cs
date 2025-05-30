﻿namespace EAMS_ACore.HelperModels
{
    public class CombinedMaster
    {
        public int StateId { get; set; }
        public string StateName { get; set; }
        public int DistrictId { get; set; }
        public string DistrictName { get; set; }
        public bool DistrictStatus { get; set; }
        public string DistrictCode { get; set; }
        public string? SecondLanguage { get; set; }
        public int AssemblyId { get; set; }
        public string AssemblyName { get; set; }
        public int AssemblyCode { get; set; }

        public int FieldOfficerMasterId { get; set; }
        public string soName { get; set; }
        public string soMobile { get; set; }
        public string soDesignation { get; set; }
        public string BoothCode_No { get; set; }

        public int BoothMasterId { get; set; }
        public string BoothName { get; set; }
        public string? BoothAuxy { get; set; }
        public int? Male { get; set; }
        public int? Female { get; set; }
        public int? Transgender { get; set; }

        public int PCMasterId { get; set; }
        public string PCName { get; set; }
        public string PCCode { get; set; }
        public string PcType { get; set; }
        public bool PCStatus { get; set; }
        //public string BoothAuxy { get; set; }
        public bool IsAssigned { get; set; }
        public bool IsStatus { get; set; }
        public int? LocationMasterId { get; set; }
        public int? NoOfPollingAgent { get; set; }
        public string? RecentOTP { get; set; }
        public DateTime? OTPExpireTime { get; set; }
        public int? OTPAttempts { get; set; }
        public int ElectionTypeMasterId { get; set; }
        public string ElectionTypeName { get; set; }
        public int? FourthLevelHMasterId
        {
            get;
            set;
        }
        public int? FourthLevelHTotalVoters
        {
            get;
            set;
        }
        public string? FourthLevelHName { get; set; }
        public string? PSZonePanchayatName { get; set; }
        public int? PSZoneMasterId
        {
            get;
            set;
        }
        public int EventMasterId { get; set; }
        public string? EventName { get; set; }
        public string? EventABBR { get; set; }
        public int EventSequence { get; set; }
        public bool EventStatus { get; set; }
        public bool IsPrimaryBooth { get; set; }
        public bool IsBoothInterrupted { get; set; }
        public bool IsVTInterrupted { get; set; }//Voter Turnout Event 
        public int? TotalVoters { get; set; }
    }
    public class CombinedPanchayatMaster
    {
        public int StateId { get; set; }
        public string StateName { get; set; }
        public int DistrictId { get; set; }
        public string DistrictName { get; set; }
        public bool DistrictStatus { get; set; }
        public string DistrictCode { get; set; }
        public string? SecondLanguage { get; set; }
        public int AssemblyId { get; set; }
        public string AssemblyName { get; set; }
        public int AssemblyCode { get; set; }
        public int ElectionTypeMasterId { get; set; }
        public string ElectionTypeName { get; set; }
        public int? FourthLevelHMasterId
        {
            get;
            set;
        }
        public string? HierarchyName { get; set; }
        public int HierarchyCode { get; set; }
        public bool IsAssigned { get; set; }
        public bool IsStatus { get; set; }
        public int? Male { get; set; }
        public int? Female { get; set; }
        public int? Transgender { get; set; }
        public int? TotalVoters { get; set; }
        public bool IsWinner { get; set; }
        public bool IsDraw { get; set; }//For Draw Candidates
        public bool IsDrawLottery { get; set; }//Choosen  of candidate by blindfolded  box
        public bool IsReCounting { get; set; } // ReCounting in case of draw candidates
        public bool IsResultDeclared { get; set; }
        public bool IsCC { get; set; }
        public bool IsNN { get; set; }
    }

    public class CombinedGPWardMaster
    {
        public int StateId { get; set; }
        public string StateName { get; set; }
        public int DistrictId { get; set; }
        public string DistrictName { get; set; }
        public bool DistrictStatus { get; set; }
        public string DistrictCode { get; set; }
        public string? SecondLanguage { get; set; }
        public int AssemblyId { get; set; }
        public string AssemblyName { get; set; }
        public int AssemblyCode { get; set; }
        public int ElectionTypeMasterId { get; set; }
        public string ElectionTypeName { get; set; }
        public int? FourthLevelHMasterId
        {
            get;
            set;
        }
        public string? HierarchyName { get; set; }
        public int HierarchyCode { get; set; }
        public int GPPanchayatWardsMasterId { get; set; }
        public string GPPanchayatWardsName { get; set; }
        public int GPPanchayatWardsCode { get; set; }
        public bool IsAssigned { get; set; }
        public bool IsStatus { get; set; }
        public int? Male { get; set; }
        public int? Female { get; set; }
        public int? Transgender { get; set; }
        public int? TotalVoters { get; set; }
        public bool IsWinner { get; set; }
        public bool IsDraw { get; set; }//For Draw Candidates
        public bool IsDrawLottery { get; set; }//Choosen  of candidate by blindfolded  box
        public bool IsReCounting { get; set; } // ReCounting in case of draw candidates
        public bool IsResultDeclared { get; set; }
        public bool IsCC { get; set; }
        public bool IsNN { get; set; }
    }
}


