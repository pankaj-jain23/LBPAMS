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
using System.Net.Http.Json;
using System.Text;

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
        public ExternalService(IConfiguration configuration, IAuthRepository authRepository, UserManager<UserRegistration> userManager,
            RoleManager<IdentityRole> roleManager, IEamsService eamsService, IEamsRepository eamsRepository,   ILogger<AuthService> logger,HttpClient httpClient) 
        {
            _configuration = configuration;
            _authRepository = authRepository;
            _userManager = userManager;
            _roleManager = roleManager;
            _EAMSService = eamsService;
            _eamsRepository = eamsRepository; 
            _logger = logger;
            _httpClient = httpClient;
        }

        public async Task<ServiceResponse> SendOTP()
        {
           //_httpClient.PostAsJsonAsync();
            throw new NotImplementedException();
        }
        public async Task<ServiceResponse> SendSmsAsync(string userName, string password, string senderId, string mobileNo, string message, string entityId, string templateId)
        {
            // Define the SOAP envelope.
            var soapEnvelope = $@"
                                <soapenv:Envelope xmlns:soapenv='http://schemas.xmlsoap.org/soap/envelope/' xmlns:tem='http://tempuri.org/'>
                                    <soapenv:Header/>
                                    <soapenv:Body>
                                        <tem:SendSMS>
                                            <tem:userName>{userName}</tem:userName>
                                            <tem:password>{password}</tem:password>
                                            <tem:senderId>{senderId}</tem:senderId>
                                            <tem:mobileNo>{mobileNo}</tem:mobileNo>
                                            <tem:message>{message}</tem:message>
                                            <tem:entityId>{entityId}</tem:entityId>
                                            <tem:templateId>{templateId}</tem:templateId>
                                        </tem:SendSMS>
                                    </soapenv:Body>
                                </soapenv:Envelope>";

            var content = new StringContent(soapEnvelope, Encoding.UTF8, "text/xml");

            // Clear existing headers and add new SOAPAction header.
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("SOAPAction", "http://tempuri.org/SendSMS");

            var serviceResponse = new ServiceResponse();

            try
            {
                var response = await _httpClient.PostAsync("http://10.44.250.220/SendSMSToAny.asmx", content);

                if (response.IsSuccessStatusCode)
                {
                    string result = await response.Content.ReadAsStringAsync();

                    // Check for success indication in result.
                    bool isSucceed = result.Contains(SMSEnum.MessageAccepted.GetStringValue());

                    serviceResponse.IsSucceed = isSucceed;
                    serviceResponse.Message = result;
                }
                else
                {
                    serviceResponse.IsSucceed = false;
                    serviceResponse.Message = $"Error: {response.StatusCode}";
                }
            }
            catch (Exception ex)
            {
                // Catch and return any exceptions.
                serviceResponse.IsSucceed = false;
                serviceResponse.Message = $"Exception: {ex.Message}";
            }

            return serviceResponse;
        }

    }

}
