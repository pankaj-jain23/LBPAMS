using EAMS_ACore.HelperModels;

namespace EAMS.Helper
{
    public class Response
    {
        public RequestStatusEnum Status { get; set; }
        public string? Message { get; set; }
        public MobileAccessToken AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }
    public class MobileAccessToken
    {
        public string? LSToken { get; set; }
        public string? VSToken { get; set; }
    }
}
