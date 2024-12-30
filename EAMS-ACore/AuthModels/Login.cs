using System.ComponentModel.DataAnnotations;

namespace EAMS_ACore.AuthModels
{
    public class Login
    {
        [Required(ErrorMessage = "UserName is required")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }
        public string? Otp { get; set; }
    }
}
