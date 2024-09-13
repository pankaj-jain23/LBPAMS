using System.ComponentModel.DataAnnotations;

namespace EAMS.ViewModels
{
    public class BoothViewModel
    {
        [Required(ErrorMessage = "State Master Id is required")]
        public int StateMasterId { get; set; }
        [Required(ErrorMessage = "District Master Id is required")]
        public int DistrictMasterId { get; set; }

        [Required(ErrorMessage = "Assembly Master Id is required")]
        public int AssemblyMasterId { get; set; }
        [Required(ErrorMessage = "Booth Master Id is required")]
        public List<int> BoothMasterId { get; set; }

        public string AssignedBy { get; set; }

        public string AssignedTo { get; set; }
        [Required(ErrorMessage = "Is Assigned Status is required")]

        public bool IsAssigned { get; set; }
        [Required(ErrorMessage = "Election Type Master Id is required")]
        public int ElectionTypeMasterId { get; set; }
    }
}
