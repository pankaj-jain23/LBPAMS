using System.ComponentModel.DataAnnotations;

namespace EAMS_ACore.Models.PublicModels
{
    public class Kyc
    {
        [Key]
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

        public int BlockZonePanchayatMasterId
        {
            get;
            set;
        }
        public int SarpanchWardsMasterId
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
        public string NominationPdfPath
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

    }
    public class KycList
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
        public string StateName
        {
            get;
            set;

        }
        public int? DistrictMasterId
        {
            get;
            set;
        }
        public string DistrictName
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
        public string AssemblyName
        {
            get;
            set;

        }
        public int FourthLevelHMasterId

        {
            get;
            set;
        }
        public string FourthLevelName
        {
            get;
            set;

        }
        public int BlockZonePanchayatMasterId
        {
            get;
            set;
        }
        public string BlockZonePanchayatName
        {
            get;
            set;

        }
        public int SarpanchWardsMasterId
        {
            get;
            set;
        }
        public string SarpanchWardsName
        {
            get;
            set;

        }
        public string CandidateType
        {
            get;
            set;
        }
        public string SarpanchName
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
        public string NominationPdfPath
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

    }
}
