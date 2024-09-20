namespace LBPAMS.ViewModels.EventActivityViewModels
{
    public class UpdateEventActivityViewModel
    {
        public int BoothMasterId { get; set; }
        public string BoothCode { get; set; }
        public int EventMasterId { get; set; }
        public string EventABBR { get; set; }
        public int EventSequence { get; set; }
        public bool EventStatus { get; set; } 

    }
}
