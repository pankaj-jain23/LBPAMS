using System.ComponentModel.DataAnnotations;

namespace EAMS.ViewModels
{
    public class ElectionInfoViewModel
    {
        
        public int StateMasterId { get; set; }
       
        public int DistrictMasterId { get; set; }
        public int ElectionTypeMasterId { get; set; }
        public int AssemblyMasterId { get; set; }

        public int? FourthLevelMasterId { get; set; }
        public int? PSZoneMasterId { get; set; }

        public int BoothMasterId { get; set; }
       
        public int EventMasterId { get; set; }
        public int? EventSequence { get; set; }
        public int? EventABBR { get; set; }
        public string? FOUserId { get; set; }
        public string? AROUserId { get; set; }
        public bool EventStatus { get; set; }
        /*** for MockPoll Polling Agents**/
        public int? NoOfPollingAgents { get; set; }
        public string? VoterInQueue { get; set; }
        public string? FinalVotes { get; set; }

        /*** for Voter Turn Out**/
        public int VoterValue { get; set; } 

        /**Male,Female,Transgender**/
        public string? Male { get; set; }
        public string? Female { get; set; }
        public string? Transgender { get; set; }

        public bool? IsQueueUndo { get; set; }
        public string? EDC { get; set; }
       
    }
}
