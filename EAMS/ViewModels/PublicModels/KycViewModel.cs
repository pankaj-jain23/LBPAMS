using System.Text.Json.Serialization;

namespace EAMS.ViewModels.PublicModels
{
    public class KycViewModel
    {

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
        public IFormFile NominationPdf
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
