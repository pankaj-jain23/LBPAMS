namespace LBPAMS.ViewModels.PublicModels
{
    public class UnOpposedViewModel
    {
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

        public int? BlockZonePanchayatMasterId
        {
            get;
            set;
        }
        public int? SarpanchWardsMasterId
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
        public IFormFile? NominationPdf
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
    public class UpdateUnOpposedViewModel
    {
        public int? UnOpposedMasterId
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

        public int? BlockZonePanchayatMasterId //Sarpanch is Panchyat
        {
            get;
            set;
        }

        public int? SarpanchWardsMasterId //Panch Wards
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
        public IFormFile? NominationPdf
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
