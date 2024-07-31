namespace EAMS.ViewModels.PublicModels
{
    public class KycResponseViewModel
    {
        public int KycMasterId { get; set; }
        public int StateMasterId
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
        public int? PsZoneMasterId
        {
            get;
            set;
        }
        public int? BoothMasterId
        {
            get;
            set;
        }

        public int? SarpanchWardsMasterId
        {
            get;
            set;
        }
        public string NominationPdfPath
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
    }

}
