using System.ComponentModel.DataAnnotations;

namespace EAMS.ViewModels
{
    public class ElectionInfoViewModel
    {
        [Required(ErrorMessage = "State Master Id is required")]
        public int StateMasterId { get; set; }
        [Required(ErrorMessage = "District Master Id is required")]
        public int DistrictMasterId { get; set; }
        [Required(ErrorMessage = "Assembly Master Id is required")]
        public int AssemblyMasterId { get; set; }

        [Required(ErrorMessage = "Booth Master Id is required")]
        public int BoothMasterId { get; set; }
        public int? PCMasterId { get; set; }
        [Required(ErrorMessage = "Event Master Id is required")]
        public int EventMasterId { get; set; }

        [Required(ErrorMessage = "Event Status is required")]
        public bool EventStatus { get; set; }
        /*** for MockPoll Polling Agents**/
        public int? NoOfPollingAgents { get; set; }
        public string? VoterInQueue { get; set; }
        public string? FinalVotes { get; set; }

        /*** for Voter Turn Out**/
        public int VoterValue { get; set; }
        //public string? SOUserId { get; set; }
        //public string? AROUserId { get; set; }

        /**Male,Female,Transgender**/
        public string? Male { get; set; }
        public string? Female { get; set; }
        public string? Transgender { get; set; }

        public bool? IsQueueUndo { get; set; }
        public string? EDC { get; set; }

        //public string? UserType { get;set; }
    }
}
