namespace EAMS.ViewModels.PSFormViewModel
{
    public class PSFormViewModel
    {
        public int? PollingStationMasterId { get; set; }
        public int? StateMasterId { get; set; }
        public int? PCasterId { get; set; }
        public int? DistrictMasterId { get; set; }
        public int? AssemblyMasterId { get; set; }
        public int? BoothMasterId { get; set; }

        public string StateName { get; set; }
        public string StateCode { get; set; }
        public string DistrictName { get; set; }
        public string DistrictCode { get; set; }
        public string ParliamentaryConstituencyNo { get; set; }
        public string ParliamentaryConstituencyName { get; set; }
        public string AssemblySegmentNo { get; set; }
        public string AssemblySegmentName { get; set; }
        public string PollingStationNo { get; set; }
        public string PollingStationName { get; set; }
        public string PollingStationAuxy { get; set; }

        /** For Voter Turn Out Reports**/

        public int TotalCUsUsed { get; set; }
        public int TotalBUsUsed { get; set; }
        public int TotalVVPATUsed { get; set; }

        public bool EVMReplaced { get; set; }
        public string EVMReplacementTime { get; set; }
        public string EVMReplacementReason { get; set; }

        public bool VVPATReplaced { get; set; }
        public string VVPATReplacementTime { get; set; }
        public string VVPATReplacementReason { get; set; }

        public int PollingAgents { get; set; }

        public List<PSGenderViewModel> pollingStationGenderViewModel { get; set; }

        public int VisuallyImpaired { get; set; }
        public int HearingImpaired { get; set; }
        public int LocoMotive { get; set; }
        public int PWDDisabilityOthers { get; set; }
        /// <summary>
        /// Dummy Ballot Sheet in Braille
        /// </summary>
        public int DummyBSB { get; set; }
        /// <summary>
        /// With help pf companion
        /// </summary>
        public int WHC { get; set; }
        /// <summary>
        /// with both facilites
        /// </summary>
        public int WBF { get; set; }
        public int VotePolledEPIC { get; set; }
        public string VotePolledOtherDocument { get; set; }
        public int TenderedVote { get; set; }
        public int ChallengedVote { get; set; }
        public int ProxyVote { get; set; }
        public bool IsWebCastingDone { get; set; }
        public bool IsWebCastingOperatorAvailable { get; set; }
        public string WebCastingName { get; set; }
        public string WebCastingMobileNumber { get; set; }
        public bool PSManagedByPwD { get; set; }
        public bool PinkPSIsManagedByWomen { get; set; }
        public bool IsModelStation { get; set; }

        public bool IPresidingOfficerAgree { get; set; }
        //public Int32? UserId { get; set; }
        public bool Freezed { get; set; }


    }

    public class PSGenderViewModel
    {
        public int? PollingStationMasterId { get; set; }
        public int? PollingStationGenderId { get; set; }
        public int Male { get; set; }
        public int Female { get; set; }
        public int ThirdGender { get; set; }
        public int Type { get; set; }
        public int Total { get; set; }
    }
}
