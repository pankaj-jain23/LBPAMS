namespace EAMS_ACore.AuthModels
{
    public class Token
    {
        public bool IsSucceed { get; set; }
        public bool IsOtp { get; set; }
        public bool Is2FA { get; set; }
        public string? Message { get; set; }
        public string? AccessToken { get; set; }
        public string? RefreshToken { get; set; }
    }
   

}
