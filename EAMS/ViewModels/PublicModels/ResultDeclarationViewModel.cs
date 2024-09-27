namespace LBPAMS.ViewModels.PublicModels
{
    public class ResultDeclarationViewModel
    { 
        public int CandidateId { get; set; }//KYC or UNopposed
        public int? BoothMasterId { get; set; }
        public int? GPPanchayatWardsMasterId { get; set; }
        public int CandidateType { get; set; }//UNOppose/KYC
        public int? VoteMargin
        {
            get;
            set;
        }
        public bool ResultDecStatus { get; set; }
    }
    public class UpdateResultDeclarationViewModel
    {
        public int? ResultDeclarationMasterId
        {
            get;
            set;
        }
        public int? StateMasterId
        {
            get;
            set;
        }
        public int? DistrictMasterId
        {
            get;
            set;
        }
        public int? ElectionTypeMasterId
        {
            get;
            set;
        }
        public int? AssemblyMasterId //ZP
        {
            get;
            set;
        }
        public int? FourthLevelHMasterId // GPSarpanch // MCorp //MC //NP

        {
            get;
            set;
        }
        public int? GPPanchayatWardsMasterId //GPpanch-Ward
        {
            get;
            set;
        }
        public string CandidateName
        {
            get;
            set;
        }
        public string FatherName
        {
            get;
            set;
        }
        public string? VoteMargin
        {
            get;
            set;
        }
        public bool ResultDecStatus { get; set; }
    }

}
