using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAMS_ACore.Models
{
    public class BoothDetailForVoterInQueue
    {
        public int BoothMasterId { get; set; }
        public int? TotalVoters { get; set; }
        public int? VotesPolled { get; set; }
        public int? RemainingVoters { get; set; }
        public DateTime? VotesPolledTime { get; set; }
        public bool IsVoteEnabled { get; set; }
        public string Message { get; set; }
    }
}
