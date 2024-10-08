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
        public bool IsUnOppossed
        {
            get;
            set;
        }
        public string? Age
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
        public int? StateMasterId
        {
            get;
            set;
        }
        public string? StateName
        {
            get;
            set;

        }
        public int? DistrictMasterId
        {
            get;
            set;
        }
        public string? DistrictName
        {
            get;
            set;

        }
        public int? ElectionTypeMasterId
        {
            get;
            set;
        }
        public string? ElectionTypeName
        {
            get;
            set;
        }
        public int? AssemblyMasterId
        {
            get;
            set;
        }
        public string? AssemblyName
        {
            get;
            set;

        }
        public int? FourthLevelHMasterId

        {
            get;
            set;
        }
        public string? FourthLevelHName
        {
            get;
            set;

        }
        public string? HierarchyType { get; set; }
        public int? PSZonePanchayatMasterId
        {
            get;
            set;
        }
      
        public string? PSZonePanchayatName
        {
            get;
            set;

        }
        public int? GPPanchayatWardsMasterId
        {
            get;
            set;
        }
        public string? GPPanchayatWardsName
        {
            get;
            set;

        }
        public string? GPPanchayatWardsCategory { get; set; }
        public int? GPPanchayatWardsCode { get; set; }

        public string CandidateType
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
        public string? Age
        {

            get; set;
        }
        public string? Option2
        {

            get; set;
        }
        public bool IsUnOppossed
        {
            get;
            set;
        }
        
    }
}
