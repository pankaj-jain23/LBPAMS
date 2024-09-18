using System.ComponentModel.DataAnnotations;

namespace EAMS.ViewModels
{
    public class EventMasterViewModel
    {
        
        public string EventName { get; set; }
        [Required(ErrorMessage = "Event Sequence is required")]
        public int EventSequence { get; set; }
        [Required(ErrorMessage = "Event ABBR is required")]
        public string EventABBR { get; set; }
        [Required(ErrorMessage = "StateMasterId is required")]
        public int StateMasterId { get; set; }
        [Required(ErrorMessage = "ElectionTypeMasterId is required")]
        public int ElectionTypeMasterId { get; set; }
        [Required(ErrorMessage = "Active Status is required")]

        public bool IsStatus { get; set; }
    }
    public class UpdateEventMasterViewModel
    {
        public int EventMasterId { get; set; }
        [Required(ErrorMessage = "Event Name is required")]
        public string EventName { get; set; }
        [Required(ErrorMessage = "Event Sequence is required")]
        public int EventSequence { get; set; }
        [Required(ErrorMessage = "Event ABBR is required")]
        public string EventABBR { get; set; }
        [Required(ErrorMessage = "StateMasterId is required")]
        public int StateMasterId { get; set; }
        [Required(ErrorMessage = "ElectionTypeMasterId is required")]
        public int ElectionTypeMasterId { get; set; }
        [Required(ErrorMessage = "Active Status is required")]

        public bool IsStatus { get; set; }
    }
}
