namespace LBPAMS.ViewModels.PublicModels
{
    public class GPVoterViewModel
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
        public IFormFile? GPVoterPdf
        {
            get;
            set;
        }
        public DateTime? GPVoterCreatedAt { get; set; }

        public DateTime? GPVoterUpdatedAt { get; set; }

        public DateTime? GPVoterDeletedAt { get; set; }
        public bool GPVoterStatus { get; set; }
    }
    public class UpdateGPVoterViewModel
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
        public IFormFile? GPVoterPdf
        {
            get;
            set;
        }
        public DateTime? GPVoterCreatedAt { get; set; }

        public DateTime? GPVoterUpdatedAt { get; set; }

        public DateTime? GPVoterDeletedAt { get; set; }
        public bool GPVoterStatus { get; set; }
    }
}
