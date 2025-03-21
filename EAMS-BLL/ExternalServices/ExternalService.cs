﻿using EAMS_ACore.HelperModels;
using EAMS_ACore.IExternal;
using System.Text;

namespace EAMS_BLL.ExternalServices
{
    public class ExternalService : IExternal
    {

        private readonly HttpClient _httpClient;

        public ExternalService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("SmsClient");
        }


        public async Task<ServiceResponse> SendSmsAsync(  string mobileNo, string otp)
        { 

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
