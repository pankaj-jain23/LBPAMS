namespace LBPAMS.ViewModels.EventActivityViewModels
{
    public class UpdateEventActivityViewModel
    {
        public int BoothMasterId { get; set; }     
        public int EventMasterId { get; set; }
        public string EventABBR { get; set; }
        public string EventName { get; set; }
        public int EventSequence { get; set; }
        public bool EventStatus { get; set; }
        public int? NoOfPollingAgents { get; set; }
        public int? VoterInQueue { get; set; }
    }
    public class UpdateVTEventActivityViewModel //Voter Turn Out
    {
        public int BoothMasterId { get; set; } 
        public int EventMasterId { get; set; }
        public string EventABBR { get; set; }
        public string EventName { get; set; }
        public int EventSequence { get; set; }
        public bool EventStatus { get; set; }
        public int? VotesPolled { get; set; }
    }
}
