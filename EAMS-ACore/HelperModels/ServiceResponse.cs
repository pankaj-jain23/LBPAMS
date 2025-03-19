namespace EAMS_ACore.HelperModels
{
    public class ServiceResponse
    {
        public bool IsSucceed { get; set; }
        public string Message { get; set; }
    }
    public class AuthServiceResponse
    {
        public bool IsSucceed { get; set; }
        public string? UserId { get; set; }
        public string Message { get; set; }
    }
    public class ServiceResponseForRD
    {
        public bool IsWinner { get; set; }
        public bool IsDraw { get; set; }
        public bool IsSucceed { get; set; }
        public string Message { get; set; }
    }
}
