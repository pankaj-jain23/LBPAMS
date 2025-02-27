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
        private readonly IConfiguration _configuration;
        private readonly IAuthRepository _authRepository;
        private readonly IEamsService _EAMSService;
        private readonly IEamsRepository _eamsRepository;
        private readonly UserManager<UserRegistration> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<AuthService> _logger;
        private readonly HttpClient _httpClient;
        private readonly AsyncRetryPolicy<HttpResponseMessage> _retryPolicy;

        public ExternalService(
            IConfiguration configuration,
            IAuthRepository authRepository,
            UserManager<UserRegistration> userManager,
            RoleManager<IdentityRole> roleManager,
            IEamsService eamsService,
            IEamsRepository eamsRepository,
            ILogger<AuthService> logger,
            HttpClient httpClient,
            AsyncRetryPolicy<HttpResponseMessage> retryPolicy)
        {
            _configuration = configuration;
            _authRepository = authRepository;
            _userManager = userManager;
            _roleManager = roleManager;
            _EAMSService = eamsService;
            _eamsRepository = eamsRepository;
            _logger = logger;
            _httpClient = httpClient;
            _retryPolicy = retryPolicy;

            // Add necessary headers once
            _httpClient.DefaultRequestHeaders.Add("SOAPAction", "http://tempuri.org/SendSMS");
        }

        public Task<ServiceResponse> SendOTP()
        {
            throw new NotImplementedException();
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
                var response = await _retryPolicy.ExecuteAsync(async () =>
                    await _httpClient.PostAsync("http://10.44.250.220/SendSMSToAny.asmx", content));

                serviceResponse.IsSucceed = response.IsSuccessStatusCode;
                serviceResponse.Message = response.IsSuccessStatusCode
                    ? await response.Content.ReadAsStringAsync()
                    : $"Error: {response.StatusCode}";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending SMS.");
                serviceResponse.IsSucceed = false;
                serviceResponse.Message = $"Exception: {ex.Message}";
            }

            return serviceResponse;
        }
    }
}
