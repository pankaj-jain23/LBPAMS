using System.Text.Json.Serialization;

namespace LBPAMS.ViewModels.PublicModels
{
    public class ResultViewModel
    {
            public object? BoothCode;
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

    }
    
}
