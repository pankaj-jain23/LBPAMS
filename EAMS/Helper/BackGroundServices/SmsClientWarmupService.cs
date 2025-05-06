using EAMS_ACore.HelperModels;
using System.Text;

namespace LBPAMS.Helper.BackGroundServices
{
    public class SmsClientWarmupService : IHostedService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<SmsClientWarmupService> _logger;

        public SmsClientWarmupService(
            IHttpClientFactory httpClientFactory,
            ILogger<SmsClientWarmupService> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Performing thorough SMS client warmup...");
                var client = _httpClientFactory.CreateClient("SmsClient");

                // Create a real SOAP envelope
                var soapEnvelope = new StringBuilder()
                   .Append("<soapenv:Envelope xmlns:soapenv='http://schemas.xmlsoap.org/soap/envelope/' xmlns:tem='http://tempuri.org/'>")
                   .Append("<soapenv:Header/>")
                   .Append("<soapenv:Body>")
                   .Append("<tem:SendSMS>")
                   .AppendFormat("<tem:userName>{0}</tem:userName>", SMSEnum.UserName.GetStringValue())
                   .AppendFormat("<tem:password>{0}</tem:password>", SMSEnum.Password.GetStringValue())
                   .AppendFormat("<tem:senderId>{0}</tem:senderId>", SMSEnum.SenderId.GetStringValue())
                   .AppendFormat("<tem:mobileNo>{0}</tem:mobileNo>", 0000000000)
                   .AppendFormat("<tem:message>{0}</tem:message>", "Pre Warm Up")
                   .AppendFormat("<tem:entityId>{0}</tem:entityId>", SMSEnum.EntityId.GetStringValue())
                   .AppendFormat("<tem:templateId>{0}</tem:templateId>", SMSEnum.TemplateId.GetStringValue())
                   .Append("</tem:SendSMS>")
                   .Append("</soapenv:Body>")
                   .Append("</soapenv:Envelope>")
                   .ToString();

                var content = new StringContent(soapEnvelope, Encoding.UTF8, "text/xml");

               
                await client.PostAsync("SendSMSToAny.asmx", content);
                _logger.LogInformation("SMS client fully warmed up");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during thorough warmup");
            }
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
