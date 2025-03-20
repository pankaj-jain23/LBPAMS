using System.Net.Http.Json;
using System.Text;
using EAMS_ACore.AuthModels;
using EAMS_ACore.HelperModels;
using EAMS_ACore.IAuthRepository;
using EAMS_ACore.IExternal;
using EAMS_ACore.Interfaces;
using EAMS_ACore.IRepository;
using EAMS_BLL.AuthServices;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;

namespace EAMS_BLL.ExternalServices
{
    public class ExternalService : IExternal
    {

        private readonly HttpClient _httpClient;

        public ExternalService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("SmsClient");
        }


        public async Task<ServiceResponse> SendSmsAsync(string userName, string password, string senderId, string mobileNo, string message, string entityId, string templateId)
        {
            var soapEnvelope = new StringBuilder()
                .Append("<soapenv:Envelope xmlns:soapenv='http://schemas.xmlsoap.org/soap/envelope/' xmlns:tem='http://tempuri.org/'>")
                .Append("<soapenv:Header/>")
                .Append("<soapenv:Body>")
                .Append("<tem:SendSMS>")
                .AppendFormat("<tem:userName>{0}</tem:userName>", userName)
                .AppendFormat("<tem:password>{0}</tem:password>", password)
                .AppendFormat("<tem:senderId>{0}</tem:senderId>", senderId)
                .AppendFormat("<tem:mobileNo>{0}</tem:mobileNo>", mobileNo)
                .AppendFormat("<tem:message>{0}</tem:message>", message)
                .AppendFormat("<tem:entityId>{0}</tem:entityId>", entityId)
                .AppendFormat("<tem:templateId>{0}</tem:templateId>", templateId)
                .Append("</tem:SendSMS>")
                .Append("</soapenv:Body>")
                .Append("</soapenv:Envelope>")
                .ToString();

            var content = new StringContent(soapEnvelope, Encoding.UTF8, "text/xml");

            var serviceResponse = new ServiceResponse();

            try
            {
                var response = await _httpClient.PostAsync("SendSMSToAny.asmx", content);

                serviceResponse.IsSucceed = response.IsSuccessStatusCode;
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
