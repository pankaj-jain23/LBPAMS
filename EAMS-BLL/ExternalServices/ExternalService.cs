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

namespace EAMS_BLL.ExternalServices
{
    public class ExternalService : IExternal
    {
        private readonly IConfiguration _configuration;
        private readonly IAuthRepository _authRepository;
        private readonly IEamsService _EAMSService;
        private readonly IEamsRepository _eamsRepository;
        private readonly INotificationService _notificationService;
        private readonly UserManager<UserRegistration> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<AuthService> _logger;
        private readonly HttpClient _httpClient;
        public ExternalService(IConfiguration configuration, IAuthRepository authRepository, UserManager<UserRegistration> userManager,
            RoleManager<IdentityRole> roleManager, IEamsService eamsService, IEamsRepository eamsRepository, INotificationService notificationService, ILogger<AuthService> logger,HttpClient httpClient) 
        {
            _configuration = configuration;
            _authRepository = authRepository;
            _userManager = userManager;
            _roleManager = roleManager;
            _EAMSService = eamsService;
            _eamsRepository = eamsRepository;
            _notificationService = notificationService;
            _logger = logger;
            _httpClient = httpClient;
        }

        public async Task<ServiceResponse> SendOTP()
        {
           //_httpClient.PostAsJsonAsync();
            throw new NotImplementedException();
        }
    }

}
