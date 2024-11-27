using System.ComponentModel.DataAnnotations;

namespace EAMS.AuthViewModels
{
    public class UserRegistrationViewModel
    {
        public int? ElectionTypeMasterId { get; set; }
        public int? StateMasterId
        {
            get;
            set;
        }
        public int? DistrictMasterId
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

        [Required(ErrorMessage = "PhoneNumber is required")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "UserName is required")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }



        [Required(ErrorMessage = "Role is Required")]
        public List<string>? RoleId
        {
            get;
            set;
        }
    }
    public class SwitchDashboardUserViewModel
    {
        [Required]
        public int ElectionTypeMasterId { get; set; }
    }

}
