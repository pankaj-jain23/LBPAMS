using System.ComponentModel.DataAnnotations;

namespace EAMS.AuthViewModels
{
    public class GetRefreshTokenViewModel
    {
        public GetAccessTokenViewModel AccessToken { get; set; }
        [Required]
        public string RefreshToken { get; set; }
    }
    public class GetAccessTokenViewModel
    {
        public string? LSToken { get; set; }
        public string? VSToken { get; set; }
    }
}
