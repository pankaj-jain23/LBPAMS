using EAMS.Helper;
using EAMS_ACore.HelperModels;
using EAMS_ACore.IExternal;
using EAMS_ACore.Interfaces;
using EAMS_ACore.IRepository;
using EAMS_ACore.NotificationModels;
using EAMS_ACore.ReportModels;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
namespace EAMS_BLL.Services
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _notificationRepository;
        private readonly FcmNotificationSetting _fcmNotificationSetting;
        private readonly ILogger<NotificationService> _logger;
        private readonly IExternal _external;

        public NotificationService(IOptions<FcmNotificationSetting> settings, INotificationRepository notificationRepository, ILogger<NotificationService> logger, IExternal external
)
        {
            _fcmNotificationSetting = settings.Value;
            _notificationRepository = notificationRepository;
            _logger = logger;
            _external = external;   

        }
        private DateTime? BharatDateTime()
        {
            DateTime dateTime = DateTime.Now;
            DateTime utcDateTime = DateTime.SpecifyKind(dateTime.ToUniversalTime(), DateTimeKind.Utc);
            TimeSpan istOffset = TimeSpan.FromHours(5) + TimeSpan.FromMinutes(30);
            TimeZoneInfo istTimeZone = TimeZoneInfo.CreateCustomTimeZone("IST", istOffset, "IST", "IST");
            DateTime hiINDateTime = TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, istTimeZone);

            return DateTime.SpecifyKind(hiINDateTime, DateTimeKind.Utc);
        }
        public async Task<List<Notification>> GetNotification()
        {
            return await _notificationRepository.GetNotification();
        }

        public async Task<ServiceResponse> SendNotification(Notification notificationModel)
        {
            return await _notificationRepository.SendNotification(notificationModel);
        }
        public async Task<ServiceResponse> IsNotificationSeen(string notificationId, bool isStatus)
        {
            var notificationMasterId = Convert.ToInt32(notificationId);
            return await _notificationRepository.IsNotificationSeen(notificationMasterId, isStatus);
        }
        public async Task<ServiceResponse> AddSMSTemplate(SMSTemplate SMSModel)
        {
            return await _notificationRepository.AddSMSTemplate(SMSModel);
        }
        public async Task<SMSTemplate> GetSMSTemplateById(string smsTemplateMasterId)
        {
            return await _notificationRepository.GetSMSTemplateById(smsTemplateMasterId);

        }
        public async Task<List<SMSTemplateModel>> GetSMSTemplate()
        {
            return await _notificationRepository.GetSMSTemplate();
        }
        public async Task<ServiceResponse> UpdateSMSTemplateById(SMSTemplate sMSTemplate)
        {
            return await _notificationRepository.UpdateSMSTemplateById(sMSTemplate);
        }
        public async Task<ServiceResponse> SendOtp(string mobile, string otp)
        {
            string userNameSMS = SMSEnum.UserName.GetStringValue();
            string password = SMSEnum.Password.GetStringValue();
            string senderId = SMSEnum.SenderId.GetStringValue();
            string entityId = SMSEnum.EntityId.GetStringValue();
            string smsTypeOTP = SMSEnum.OTP.GetStringValue();
            string placeholder = "{#var#}";

            // Get the SMS template from the repository
            var getTemplate = await _notificationRepository.GetSMSTemplateById(smsTypeOTP);
            string template = getTemplate.Message;

            // Replace the placeholder with the OTP
            string finalsmsTemplateMsg = template.Replace(placeholder, otp.Trim());

            // Call the SendSmsAsync method from ExternalService
           var smsResponse = await _external.SendSmsAsync(userNameSMS, password, senderId, mobile, finalsmsTemplateMsg, entityId, getTemplate.TemplateId.ToString());

            // Return the service response
            return smsResponse;
        }



        //public async Task<ServiceResponse> SendOtp(string mobile, string otp)
        //{
        //    string userNameSMS = SMSEnum.UserName.GetStringValue();
        //    string password = SMSEnum.Password.GetStringValue();
        //    string senderId = SMSEnum.SenderId.GetStringValue();
        //    string entityId = SMSEnum.EntityId.GetStringValue();
        //    string smsTypeOTP = SMSEnum.OTP.GetStringValue();
        //    string placeholder = "{#var#}";

        //    var getTemplate = await _notificationRepository.GetSMSTemplateById(smsTypeOTP);
        //    string template = getTemplate.Message;

        //    string finalsmsTemplateMsg = template.Replace(placeholder, otp.Trim());

        //    var result = await SendSMSAsync(userNameSMS, password, senderId, mobile, finalsmsTemplateMsg, entityId, getTemplate.TemplateId.ToString());

        //    bool isSucceed = result.Contains(SMSEnum.MessageAccepted.GetStringValue());

        //    return new ServiceResponse
        //    {
        //        IsSucceed = isSucceed,
        //        Message = result
        //    };
        //}
        public async Task<ServiceResponse> SendSMSToSectorOfficers(SendSMSModel sendSMSModel)
        {
            SMSSentModel sMSSentModel = new SMSSentModel(); int sent = 0; int Notsent = 0;
            var soRecord = await _notificationRepository.GetSectorOfficerstoSendSMS(sendSMSModel);
            if (sendSMSModel.EventId > 0)
            {

                string FinalsmsTemplateMsg = "";
                var smsTemplateRecord = await _notificationRepository.GetSMSTemplateById(sendSMSModel.TemplateMasterId.ToString());
                if (soRecord.Count > 0)
                {
                    /*if (smsTemplateRecord is not null)
                    {
                        string userNameSMS = SMSEnum.UserName.GetStringValue();
                        string Password = SMSEnum.Password.GetStringValue();
                        string senderId = SMSEnum.SenderId.GetStringValue();
                        string entityId = SMSEnum.EntityId.GetStringValue();
                        string MessageDb = smsTemplateRecord.Message;
                        string mobile = ""; string userName = "";
                        string userNamePlaceholder = "{#userName#}";


                        foreach (var soDetail in soRecord)
                        {
                            userName = soDetail.SoName; mobile = soDetail.SoMobile;
                            if (smsTemplateRecord.Message.Contains(userNamePlaceholder))
                            {

                                FinalsmsTemplateMsg = smsTemplateRecord.Message.Replace(userNamePlaceholder, userName);

                            }
                            else
                            {
                                FinalsmsTemplateMsg = smsTemplateRecord.Message;
                            }


                            var result = SendSMSAsync(userNameSMS, Password, senderId, mobile, FinalsmsTemplateMsg, entityId, sendSMSModel.TemplateMasterId.ToString());
                            if (result.Result.Contains(SMSEnum.MessageAccepted.GetStringValue()))
                            {

                                sent += 1;
                            }
                            else
                            {
                                Notsent += 0;
                            }

                            sMSSentModel = new SMSSentModel()
                            {
                                SMSTemplateMasterId = smsTemplateRecord.SMSTemplateMasterId,
                                Message = FinalsmsTemplateMsg,
                                Mobile = soDetail.SoMobile,
                                RemarksFromGW = result.Result,
                                CreatedAt = BharatDateTime(),
                                //Status=,
                                //SentToUserType=

                            };

                            var res = _notificationRepository.SaveSMS(sMSSentModel);


                        }
                    }*/

                }
            }
            else
            {
                string FinalsmsTemplateMsg = "";
                if (soRecord.Count > 0)
                {
                    var smsTemplateRecord = await _notificationRepository.GetSMSTemplateById(sendSMSModel.TemplateMasterId.ToString());

                    if (smsTemplateRecord is not null)
                    {
                        string userNameSMS = SMSEnum.UserName.GetStringValue();
                        string Password = SMSEnum.Password.GetStringValue();
                        string senderId = SMSEnum.SenderId.GetStringValue();
                        string entityId = SMSEnum.EntityId.GetStringValue();
                        string MessageDb = smsTemplateRecord.Message;
                        string mobile = ""; string userName = "";
                        string userNamePlaceholder = "{#userName#}";


                        foreach (var soDetail in soRecord)
                        {
                            userName = soDetail.SoName; mobile = soDetail.SoMobile;
                            if (smsTemplateRecord.Message.Contains(userNamePlaceholder))
                            {

                                FinalsmsTemplateMsg = smsTemplateRecord.Message.Replace(userNamePlaceholder, userName);

                            }
                            else
                            {
                                FinalsmsTemplateMsg = smsTemplateRecord.Message;
                            }


                            var result = SendSMSAsync(userNameSMS, Password, senderId, mobile, FinalsmsTemplateMsg, entityId, sendSMSModel.TemplateMasterId.ToString());
                            if (result.Result.Contains(SMSEnum.MessageAccepted.GetStringValue()))
                            {

                                sent += 1;
                            }
                            else
                            {
                                Notsent += 0;
                            }

                            sMSSentModel = new SMSSentModel()
                            {
                                SMSTemplateMasterId = smsTemplateRecord.SMSTemplateMasterId,
                                Message = FinalsmsTemplateMsg,
                                Mobile = soDetail.SoMobile,
                                RemarksFromGW = result.Result,
                                CreatedAt = BharatDateTime(),
                                //Status=,
                                //SentToUserType=

                            };

                            var res = _notificationRepository.SaveSMS(sMSSentModel);


                        }
                    }

                }

            }
            return new ServiceResponse() { Message = "SMS Sent: " + sent + "/Not Sent: " + Notsent };
        }


        #region SendSMSAsync 
        public async Task<string> SendSMSAsync(string uname, string password, string senderidstr, string mobileNo, string message, string entityidstr, string templateidstr)
        {
            string username = uname?.Trim() ?? string.Empty;
            string pin = password?.Trim() ?? string.Empty;
            string senderid = senderidstr?.Trim() ?? string.Empty;
            string entityid = entityidstr?.Trim() ?? string.Empty;
            string templateid = templateidstr?.Trim() ?? string.Empty;
            string msg = message?.Trim() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(msg))
                return string.Empty;

            //string smsPostUrl = "https://smsgw.sms.gov.in/failsafe/MLink?username={0}&pin={1}&message={2}&mnumber={3}&signature={4}&dlt_entity_id={5}&dlt_template_id={6}";
            string smsPostUrl = "https://smsgw.sms.gov.in/failsafe/MLink?username={0}&pin={1}&message={2}&mnumber={3}&signature={4}&dlt_entity_id={5}&dlt_template_id={6}";
            string requestUrl = string.Format(smsPostUrl, username, pin, Uri.EscapeDataString(msg), mobileNo, senderid, entityid, templateid);

            using (HttpClient client = CreateHttpClient())
            {
                client.Timeout = TimeSpan.FromMinutes(5);
                try
                {
                    HttpResponseMessage response = await client.GetAsync(requestUrl);
                    response.EnsureSuccessStatusCode();
                    return await response.Content.ReadAsStringAsync();
                }
                catch (HttpRequestException ex)
                {
                    return ex.Message;
                }
            }
        }

        //public async Task<string> SendSMSAsync(string uname, string password, string mobileNo, string message, string senderid, string entityid, string templateid)
        //{
        //    string username = uname?.Trim() ?? string.Empty;
        //    string pin = password?.Trim() ?? string.Empty;
        //    string msg = message?.Trim() ?? string.Empty;

        //    if (string.IsNullOrWhiteSpace(msg))
        //        return string.Empty;

        //    var url = "https://smsgw.sms.gov.in/failsafe/MLink";

        //    // Prepare the data as a string
        //    var postData = $"username={username}&pin={pin}&mnumber={mobileNo}&message={Uri.EscapeDataString(msg)}&signature={senderid}&dlt_entity_id={entityid}&dlt_template_id={templateid}";

        //    using (var httpClient = new HttpClient())
        //    {
        //        httpClient.Timeout = TimeSpan.FromMinutes(3); // Adjust the timeout as necessary
        //        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        //        // Create StringContent for POST
        //        var content = new StringContent(postData, Encoding.UTF8, "text/plain");

        //        try
        //        {
        //            // Send POST request
        //            HttpResponseMessage response = await httpClient.PostAsync(url, content);
        //            response.EnsureSuccessStatusCode();
        //            _logger.LogInformation("ValidateCheckKro"+response);
        //            // Read the response content
        //            string result = await response.Content.ReadAsStringAsync();
        //            Console.WriteLine(response.StatusCode);
        //            return result;
        //        }
        //        catch (HttpRequestException ex)
        //        {
        //            _logger.LogInformation("ValidateCheckKro" + ex.Message); // Log exception details if necessary
        //            Console.WriteLine(ex.Message);
        //            return ex.Message;
        //        }
        //    }
        //}










        public static void Check_SSL_Certificate()
        {
            ServicePointManager.ServerCertificateValidationCallback = RemoteCertificateValidate;
        }

        public static bool RemoteCertificateValidate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            if ((sslPolicyErrors & SslPolicyErrors.RemoteCertificateChainErrors) == SslPolicyErrors.RemoteCertificateChainErrors)
            {
                return true;
            }
            else if ((sslPolicyErrors & SslPolicyErrors.RemoteCertificateNameMismatch) == SslPolicyErrors.RemoteCertificateNameMismatch)
            {
                // You can adapt the zone logic if needed
                if ((sender as HttpRequestMessage)?.RequestUri.IsLoopback ?? false)
                {
                    return true;
                }
                return false;
            }
            else
            {
                return true;
            }
        }

        public static HttpClient CreateHttpClient()
        {
            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (sender, certificate, chain, sslPolicyErrors) =>
                    RemoteCertificateValidate(sender, certificate, chain, sslPolicyErrors)
            };

            return new HttpClient(handler);
        }





        #endregion
    }
}
