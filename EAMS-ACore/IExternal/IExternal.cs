using EAMS_ACore.HelperModels;

namespace EAMS_ACore.IExternal
{
    public interface IExternal
    {
      
        Task<ServiceResponse> SendSmsAsync(  string mobileNo, string otp );
        Task<ServiceResponse> SendBulkSMS(int stateMasterId,int districtMasterId);
    }
}
