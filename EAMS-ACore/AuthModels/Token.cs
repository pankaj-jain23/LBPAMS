namespace EAMS_ACore.AuthModels
{
    public class Token
    {
        public bool IsSucceed { get; set; }
        public string Message { get; set; }
        public AccessToken AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }
    public class AccessToken
    {
        public string? LSToken { get; set; }
        public string? VSToken { get; set; }
    }
   
}
