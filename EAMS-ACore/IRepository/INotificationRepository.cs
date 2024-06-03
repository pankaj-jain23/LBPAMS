using EAMS_ACore.HelperModels;
using EAMS_ACore.Models;
using EAMS_ACore.NotificationModels;
using EAMS_ACore.ReportModels;

namespace EAMS_ACore.IRepository
{
    public interface INotificationRepository
    {
        Task<ServiceResponse> SendNotification(Notification notificationModel);
        Task<ServiceResponse> IsNotificationSeen(int notificationId, bool isStatus);
        Task<ServiceResponse> AddSMSTemplate(SMSTemplate SMSModel);
        Task<SMSTemplate> GetSMSTemplateById(string smsTemplateMasterId);
        Task<ServiceResponse> SaveSMS(SMSSentModel sMSSentModel);
        Task<List<Notification>> GetNotification();
        Task<List<SMSTemplateModel>> GetSMSTemplate();
        Task<ServiceResponse> UpdateSMSTemplateById(SMSTemplate sMSTemplateModel);

        Task<List<SOList>> GetSectorOfficerstoSendSMS(SendSMSModel sendSMSModel);
    }
}
