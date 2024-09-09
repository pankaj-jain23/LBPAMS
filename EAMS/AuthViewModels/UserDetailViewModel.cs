using System.ComponentModel.DataAnnotations;

namespace LBPAMS.AuthViewModels
{
    public class UserDetailViewModel
    {
        [Phone]
        [Required(ErrorMessage = "Please Enter Mobile Number")]
        public string MobileNumber { get; set; }
        public string? Otp { get; set; }
    }
}
