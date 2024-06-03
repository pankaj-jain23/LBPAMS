using System.ComponentModel.DataAnnotations;

namespace EAMS.ViewModels.BLOMaster
{
    public class AddBLOViewModel
    {
        [Required(ErrorMessage = "State Master Id is required")]
        public int StateMasterId { get; set; }
        [Required(ErrorMessage = "DistrictMasterId Id is required")]
        public int DistrictMasterId { get; set; }
        [Required(ErrorMessage = "AssemblyMasterId is required")]
        public int AssemblyMasterId { get; set; }
        [Required(ErrorMessage = "BLOMobile is required")]
        public string BLOMobile { get; set; }
        [Required(ErrorMessage = "BLOName is required")]
        public string BLOName { get; set; }

        public bool IsStatus { get; set; }
    }
}
