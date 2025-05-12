using EAMS_ACore.HelperModels;
using EAMS_ACore.IExternal;
using EAMS_DAL.DBContext;
using Microsoft.AspNetCore.StaticAssets.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Text;
using System.Xml.Linq;
using static System.Net.WebRequestMethods;

namespace EAMS_BLL.ExternalServices
{
    public class ExternalService : IExternal
    { 
        private readonly HttpClient _httpClient;
        private readonly SMSContext _smsContext;

        public ExternalService(IHttpClientFactory httpClientFactory,SMSContext sMSContext )
        {
            _httpClient = httpClientFactory.CreateClient("SmsClient");
            _smsContext = sMSContext;

        }

        public async Task<ServiceResponse> SendBulkSMS(int stateMasterId, int districtMasterId)
        {
           var getTemplate=await _smsContext.SMSConfiguration
                .Where(x => x.StateMasterId == stateMasterId && x.DistrictMasterId == districtMasterId  )
                .FirstOrDefaultAsync();
            var getUsers = await _smsContext.SMSNumbers
                .Where(x => x.StateMasterId == stateMasterId && x.DistrictMasterId == districtMasterId)
                .ToListAsync();
            var serviceResponse = new ServiceResponse()
            { IsSucceed = true, Message = $"SMS sent to {getUsers.Count} Users successfully." };
       
            foreach (var user in getUsers)
            {
                var soapEnvelope = new StringBuilder()
                    .Append("<soapenv:Envelope xmlns:soapenv='http://schemas.xmlsoap.org/soap/envelope/' xmlns:tem='http://tempuri.org/'>")
                    .Append("<soapenv:Header/>")
                    .Append("<soapenv:Body>")
                    .Append("<tem:SendSMS>")
                    .AppendFormat("<tem:userName>{0}</tem:userName>", getTemplate.UserName)
                    .AppendFormat("<tem:password>{0}</tem:password>", getTemplate.Password)
                    .AppendFormat("<tem:senderId>{0}</tem:senderId>", getTemplate.SenderId)
                    .AppendFormat("<tem:mobileNo>{0}</tem:mobileNo>", user.Number)
                    .AppendFormat("<tem:message>{0}</tem:message>", getTemplate.Message)
                    .AppendFormat("<tem:entityId>{0}</tem:entityId>", getTemplate.EntityId)
                    .AppendFormat("<tem:templateId>{0}</tem:templateId>", getTemplate.TemplateId)
                    .Append("</tem:SendSMS>")
                    .Append("</soapenv:Body>")
                    .Append("</soapenv:Envelope>")
                    .ToString();

                var content = new StringContent(soapEnvelope, Encoding.UTF8, "text/xml");

 
                    var response = await _httpClient.PostAsync("SendSMSToAny.asmx", content);

                // Assume this is your XML string
                string xml = await response.Content.ReadAsStringAsync();

                // Load into XDocument
                var doc = XDocument.Parse(xml);

                // Extract <SendSMSResult> content
                XNamespace ns = "http://tempuri.org/";
                var rawMessage = doc.Descendants(ns + "SendSMSResult").FirstOrDefault()?.Value ?? "";

                // Clean it up: remove line breaks and trim
                user.Response = rawMessage.Replace("\n", "").Replace("\r", "").Trim();
                user.Response = rawMessage
    .Replace("\n", "")
    .Replace("\r", "")
    .Replace("Response Content:", "")
    .Trim();

                 

                 
                user.SendTime = DateTime.UtcNow;
                user.SendCount = user.SendCount == null ? 1 : user.SendCount + 1;
                _smsContext.SMSNumbers.Update(user);
                await _smsContext.SaveChangesAsync();
            }

            return serviceResponse;
        }

        public async Task<ServiceResponse> SendSmsAsync(  string mobileNo, string otp)
        {
            var startTime = DateTime.UtcNow;  // Log the start time
           
            // Get the SMS template from the repository
            var message = $"OTP: {otp} is your OTP for LBPAMS. Keep it safe for next 10 minutes. SMS generated on Date {DateTime.Now}";             
            var soapEnvelope = new StringBuilder()
                .Append("<soapenv:Envelope xmlns:soapenv='http://schemas.xmlsoap.org/soap/envelope/' xmlns:tem='http://tempuri.org/'>")
                .Append("<soapenv:Header/>")
                .Append("<soapenv:Body>")
                .Append("<tem:SendSMS>")
                .AppendFormat("<tem:userName>{0}</tem:userName>", SMSEnum.UserName.GetStringValue())
                .AppendFormat("<tem:password>{0}</tem:password>", SMSEnum.Password.GetStringValue())
                .AppendFormat("<tem:senderId>{0}</tem:senderId>", SMSEnum.SenderId.GetStringValue())
                .AppendFormat("<tem:mobileNo>{0}</tem:mobileNo>", mobileNo)
                .AppendFormat("<tem:message>{0}</tem:message>", message)
                .AppendFormat("<tem:entityId>{0}</tem:entityId>", SMSEnum.EntityId.GetStringValue())
                .AppendFormat("<tem:templateId>{0}</tem:templateId>", SMSEnum.TemplateId.GetStringValue())
                .Append("</tem:SendSMS>")
                .Append("</soapenv:Body>")
                .Append("</soapenv:Envelope>")
                .ToString();

            var content = new StringContent(soapEnvelope, Encoding.UTF8, "text/xml");

            var serviceResponse = new ServiceResponse();

            try
            {
                var response = await _httpClient.PostAsync("SendSMSToAny.asmx", content);
                var endTime = DateTime.UtcNow;  // Log the end time
                var timeTaken = endTime - startTime;  // Calculate the time taken for the request

                

                serviceResponse.IsSucceed = true;
                serviceResponse.Message = response.IsSuccessStatusCode
                    ? await response.Content.ReadAsStringAsync()
                    : $"Error: {response.StatusCode}";
            }
            catch (Exception ex)
            {
                serviceResponse.IsSucceed = false;
                serviceResponse.Message = $"Exception: {ex.Message}";
               
            }

            return serviceResponse;
        }
       
    }

     
}
