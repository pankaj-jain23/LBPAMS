using System.ComponentModel.DataAnnotations;

namespace EAMS.ViewModels
{
    public class FieldOfficerViewModel
    {
       
        [Required(ErrorMessage = "State MasterId is required")]
        public int StateMasterId { get; set; }
        [Required(ErrorMessage = "District MasterId is required")]
        public int DistrictMasterId
        {
            get;
            set;
        }
        [Required(ErrorMessage = "Assembly MasterId is required")]
        public int? AssemblyMasterId
        {
            get;
            set;
        }

        [Required(ErrorMessage = "Sector officer Name is required")]
        public string FieldOfficerName { get; set; }

        [Required(ErrorMessage = "Sector Officer Designation is required")]
        public string FieldOfficerDesignation { get; set; }

        [Required(ErrorMessage = "Sector Officer office Name is required")]
        public string FieldOfficerOfficeName { get; set; }

        [Required(ErrorMessage = "Sector Officer Mobile Number is required")]
        [StringLength(10, MinimumLength = 10)]
        public string FieldOfficerMobile { get; set; }
        public bool IsStatus { get; set; }
        public int ElectionTypeMasterId { get; set; }
    }
}
