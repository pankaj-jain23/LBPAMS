using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAMS_ACore.Models.ResultModels
{
    public class ResultList
    {
        public string Key { get; set; }
        public int? DistrictMasterId { get; set; }
        public int? AssemblyMasterId { get; set; }
        public int? MasterId { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string? PartyName
        {
            get;
            set;
        }
        public int? TotalWonCandidate { get; set; }
        public int? TotalVoteMargin { get; set; }
        public int? IsUnOpposed { get; set; }
        public List<object> Children { get; set; }
    }
}
