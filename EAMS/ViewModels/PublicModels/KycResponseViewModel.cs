namespace EAMS.ViewModels.PublicModels
{
    public class KycResponseViewModel
    {


        public int KycMasterId
        {
            get;
            set;
        }
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
        public int? AssemblyMasterId
        {
            get;
            set;
        }
        public int FourthLevelHMasterId

        {
            get;
            set;
        }

        public int PSZonePanchayatMasterId
        {
            get;
            set;
        }
        public int GPPanchayatWardsMasterId
        {
            get;
            set;
        }

        public string? Option1
        {

            get; set;
        }
        public string? Option2
        {

            get; set;
        }
        public string? NominationPdfPath
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
        public bool IsUnOppossed
        {
            get;
            set;
        }
        public string? Age
        {

            get; set;
        }
    }

}
