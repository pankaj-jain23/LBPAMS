using System.ComponentModel.DataAnnotations;

namespace EAMS_ACore.Models.PublicModels
{
    public class Rsult
    {
        public string? BoothCode;

        [Key]
        public int RsultMasterId
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
        public required string CandidateName
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

            get; set;
        }
        public string? Option2
        {

            get; set;
        }

    }
}