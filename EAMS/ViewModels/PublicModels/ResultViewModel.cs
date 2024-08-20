using System.ComponentModel.DataAnnotations.Schema;
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
            [ForeignKey("StateMasterId")]

            public int? DistrictMasterId
            {
                get;
                set;
            }
            [ForeignKey("DistrictMasterId")]
            
            public int? ElectionTypeMasterId
            {
                get;
                set;
            }
            [ForeignKey("ElectionTypeMasterId")]
            public int? AssemblyMasterId
            {
                get;
                set;
            }
            [ForeignKey("AssemblyMasterId")]
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
            
            public DateTime? ResultDecCreatedAt { get; set; }
            public DateTime? ResultDecUpdatedAt { get; set; }

            public bool ResultDecStatus {  get; set; }

    }
    
}
