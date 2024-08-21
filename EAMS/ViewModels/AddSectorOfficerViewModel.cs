using System.ComponentModel.DataAnnotations;

namespace EAMS.ViewModels
{
    public class AddSectorOfficerViewModel
    {
        [Required(ErrorMessage = "State Master Id is required")]
        public int StateMasterId { get; set; }

        [Required(ErrorMessage = "Sector officer Name is required")]
        public string SoName { get; set; }

        [Required(ErrorMessage = "Sector Officer Designation is required")]
        public string SoDesignation { get; set; }

        [Required(ErrorMessage = "Sector Officer office Name is required")]
        public string SoOfficeName { get; set; }

        [Required(ErrorMessage = "Sector Officer Assembly Code is required")]
        public int SoAssemblyCode { get; set; }

        [Required(ErrorMessage = "Sector Officer Mobile Number is required")]
        [StringLength(10, MinimumLength = 10)]
        //[RegularExpression(@"^[6-9]\d{9}$", ErrorMessage = "SoMobile must start with a digit between 6 and 9 and contain only digits")]
        //[RegularExpression("^([6-9]{10})$", ErrorMessage = "SoMobile must start with a digit between 6 and 9 and contain only digits.")]
        public string SoMobile { get; set; }
        public bool IsStatus { get; set; }
        public int ElectionTypeMasterId { get; set; }
    }
}
