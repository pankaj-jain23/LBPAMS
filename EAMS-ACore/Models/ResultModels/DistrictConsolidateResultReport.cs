using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAMS_ACore.Models.ResultModels
{
    public class DistrictConsolidateResultReport
    { 
        public string DistrictName { get; set; }
        public string AssemblyName { get; set; }
        public string FourthLevelName { get; set; }
        public string ElectionType { get; set; }
        public string Category { get; set; } // e.g., GEN, SC, OBC, etc.
        public int? TotalVotes { get; set; }
        public int? TotalNOTA { get; set; }
        public int? TotalPolledVotes { get; set; }
        public int? TotalWinningCandidate{ get; set; }
        public List<CandidateResult> CandidateResults { get; set; } = new List<CandidateResult>();
    }

    public class CandidateResult
    {
        public string CandidateName { get; set; }
        public int? Votes { get; set; }
    }
}
