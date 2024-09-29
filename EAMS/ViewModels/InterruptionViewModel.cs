using System.ComponentModel.DataAnnotations;
namespace EAMS.ViewModels
{
    public class InterruptionViewModel
    {

        [Required(ErrorMessage = "BoothMaster Id is required")]
        public int boothMasterId { get; set; }
        public int ElectionTypeMasterId { get; set; }
        [Required(ErrorMessage = "Interruption Type is required")]
        public int InterruptionType { get; set; }
        public string? stopTime { get; set; }
        public string? ResumeTime { get; set; }
        public string? newCU { get; set; }
        public string? newBU { get; set; }
        public string? oldCu { get; set; }
        public string? oldBu { get; set; }
        public string? Remarks { get; set; }

    }
}

