using System.ComponentModel.DataAnnotations;

namespace LBPAMS.ViewModels
{
    public class PanchayatReleaseViewModel
    {
        [Required(ErrorMessage = "StateMasterID is required")]
        public int StateMasterId { get; set; }

        [Required(ErrorMessage = "DistrictMasterId is required")]
        public int DistrictMasterId { get; set; }
        [Required(ErrorMessage = "AssemblyMasterId is required")]
        public int AssemblyMasterId { get; set; }
        [Required(ErrorMessage = "FourthLevelHMasterId is required")]
        public int FourthLevelHMasterId { get; set; }
        [Required(ErrorMessage = "IsAssigned is required")]
        public bool IsAssigned { get; set; }
    }
}
