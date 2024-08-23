using System.ComponentModel.DataAnnotations;

namespace EAMS.ViewModels
{
    public class AddSectorOfficerViewModel
    {
        [Required(ErrorMessage = "State MasterId is required")]
        public int StateMasterId { get; set; }
        [Required(ErrorMessage = "District MasterId is required")]
        public int DistrictMasterId
        {
            get;
            set;
        }
       
        public int? AssemblyMasterId
        {
            get;
            set;
        }
        public int? FourthLevelHMasterId
        {
            get;
            set;
        }
        [Required(ErrorMessage = "Sector officer Name is required")]
        public string SoName { get; set; }

        [Required(ErrorMessage = "Sector Officer Designation is required")]
        public string SoDesignation { get; set; }

        [Required(ErrorMessage = "Sector Officer office Name is required")]
        public string SoOfficeName { get; set; } 

        [Required(ErrorMessage = "Sector Officer Mobile Number is required")]
        [StringLength(10, MinimumLength = 10)] 
        public string SoMobile { get; set; }
        public bool IsStatus { get; set; }
        public int ElectionTypeMasterId { get; set; }
    }
}
