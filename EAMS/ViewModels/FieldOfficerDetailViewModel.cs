using System.ComponentModel.DataAnnotations;

namespace LBPAMS.ViewModels
{
    public class FieldOfficerDetailViewModel
    {
        [Phone]
        [Required(ErrorMessage = "Please Enter Mobile Number")]
        public string MobileNumber { get; set; }
        public string? Otp { get; set; }
    }
}
