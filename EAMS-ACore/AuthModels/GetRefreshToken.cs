using System.ComponentModel.DataAnnotations;

namespace EAMS_ACore.AuthModels
{
    public class GetRefreshToken
    { 
        public GetAccessToken AccessToken { get; set; }
       
        public string RefreshToken { get; set; }
    }
    public class GetAccessToken
    {
        public string? LSToken { get; set; }
        public string? VSToken { get; set; }
    }
}
