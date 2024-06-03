using EAMS_ACore.HelperModels;
using EAMS_ACore.NotificationModels;
using EAMS_ACore.ReportModels;

namespace EAMS_ACore.Interfaces
{
    public interface INotificationService
    {
        Task<ServiceResponse> SendNotification(Notification notificationModel);
        Task<ServiceResponse> IsNotificationSeen(string notificationId,bool isStatus);
        Task<ServiceResponse> AddSMSTemplate(SMSTemplate SMSModel);
        Task<ServiceResponse> UpdateSMSTemplateById(SMSTemplate sMSTemplate);
        Task<SMSTemplate> GetSMSTemplateById(string smsTemplateMasterId);
        Task<ServiceResponse> SendSMSToSectorOfficers(SendSMSModel sendsmsModel);
        Task<ServiceResponse> SendOtp(string mobile, string otp);
        Task<List<Notification>> GetNotification();
        Task<List<SMSTemplateModel>> GetSMSTemplate();


    }
}
