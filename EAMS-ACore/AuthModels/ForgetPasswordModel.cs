using System.ComponentModel.DataAnnotations;

namespace EAMS_ACore.AuthModels
{
    public class ForgetPasswordModel
    {
        [Required]
        [Phone]
        public string? MobileNumber { get; set; }
        public string UserName{ get; set; }

        [DataType(DataType.Password)]
        public string? Password { get; set; }

        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string? ConfirmPassword { get; set; }
        public string? OTP { get; set; }
    }
}
