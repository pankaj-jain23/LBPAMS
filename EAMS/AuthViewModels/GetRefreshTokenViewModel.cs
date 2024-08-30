using System.ComponentModel.DataAnnotations;

namespace EAMS.AuthViewModels
{
    public class GetRefreshTokenViewModel
    {
        public string? AccessToken { get; set; }
        [Required]
        public string? RefreshToken { get; set; }
    }
   
}
