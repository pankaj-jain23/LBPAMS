using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAMS_ACore.Models
{
    #region Result Declaration for ZP And Ps Zone

    public class RoundConfigurationViewModel
    {
        public int RoundNo { get; set; }
        public bool IsLocked { get; set; }
        public bool IsFinalize { get; set; }
    }

    public class CandidateRoundResultViewModel
    {
        public int RoundNo { get; set; }
        public int TotalVotes { get; set; }
        public int TotalCummilative { get; set; }
    }

    public class CandidateResultViewModel
    {
        public string Name { get; set; }
        public int Id { get; set; }
        public List<CandidateRoundResultViewModel> RoundResult { get; set; }
    }

    public class ResultDeclarationForZPAndPsZoneViewModel
    {
        public int ElectionType { get; set; }
        public string StateMasterId { get; set; }
        public string DistrictMasterId { get; set; }
        public string AssemblyMasterId { get; set; }
        public string FourthLevelHMasterId { get; set; }
        public int TotalRounds { get; set; }
        public int TotalBooths { get; set; }
        public int TotalTable { get; set; }
        public List<RoundConfigurationViewModel> RoundConfiguration { get; set; }
        public List<CandidateResultViewModel> Candidates { get; set; }
    }


    #endregion
}
