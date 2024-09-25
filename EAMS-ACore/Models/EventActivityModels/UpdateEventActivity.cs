using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAMS_ACore.Models.EventActivityModels
{
    public class UpdateEventActivity
    {
        public int StateMasterId { get; set; }
        public int DistrictMasterId { get; set; }
        public int AssemblyMasterId { get; set; }
        public int ElectionTypeMasterId { get; set; }
        public int BoothMasterId { get; set; }
        public int EventMasterId { get; set; }
        public string EventABBR { get; set; }
        public string EventName { get; set; }
        public int EventSequence { get; set; }
        public bool EventStatus { get; set; }
        public int? NoOfPollingAgents { get; set; }
        public int? VoterInQueue { get; set; }
        public int? VotesPolled { get; set; } //Voter Turnout Case 
        public int FinalMaleVotes { get; set; }
        public int FinalFeMaleVotes { get; set; }
        public int FinalTransgenderVotes { get; set; }
    }
    public class CheckEventActivity
    {
        public int StateMasterId { get; set; }
        public int DistrictMasterId { get; set; }
        public int AssemblyMasterId { get; set; }
        public int ElectionTypeMasterId { get; set; }
        public int BoothMasterId { get; set; }
        public int EventMasterId { get; set; }
        public string EventName { get; set; }
        public string EventABBR { get; set; }
        public int EventSequence { get; set; }
        public bool EventStatus { get; set; }
    }
}
