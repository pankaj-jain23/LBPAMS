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
        public int? RemainingVoters { get; set; }
    }
}
