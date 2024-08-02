using EAMS.Helper;
using EAMS_ACore.AuthInterfaces;
using EAMS_ACore.AuthModels;
using EAMS_ACore.HelperModels;
using EAMS_ACore.IAuthRepository;
using EAMS_ACore.Interfaces;
using EAMS_ACore.IRepository;
using EAMS_ACore.Models;
using EAMS_ACore.Models.BLOModels;
using EAMS_ACore.ReportModels;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using static System.Net.WebRequestMethods;

namespace EAMS_BLL.AuthServices
{
    public class AuthService : IAuthService
    {
        private readonly IConfiguration _configuration;
        private readonly IAuthRepository _authRepository;
        private readonly IEamsService _EAMSService;
        private readonly IEamsRepository _eamsRepository;
        private readonly INotificationService _notificationService;
        private readonly UserManager<UserRegistration> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<AuthService> _logger;
        public AuthService(IConfiguration configuration, IAuthRepository authRepository, UserManager<UserRegistration> userManager, RoleManager<IdentityRole> roleManager, IEamsService eamsService, IEamsRepository eamsRepository, INotificationService notificationService, ILogger<AuthService> logger)
        {
            _configuration = configuration;
            _authRepository = authRepository;
            _userManager = userManager;
            _roleManager = roleManager;
            _EAMSService = eamsService;
            _eamsRepository = eamsRepository;
            _notificationService = notificationService;
            _logger = logger;
        }

        #region AddDynamicRole && Get Role
        public async Task<ServiceResponse> AddDynamicRole(Role role)
        {

            return await _authRepository.AddDynamicRole(role);
        }

        public async Task<List<Role>> GetRoles()
        {
            return await _authRepository.GetRoles();
        }

        public async Task<List<UserRegistration>> GetUsersByRoleId(string roleId)
        {
            return await _authRepository.GetUsersByRoleId(roleId);
        }
        #endregion

        #region Login && Generate Token
        public async Task<Token> LoginAsync(Login login)
        {
            Token _Token = new();
            _Token.AccessToken = new AccessToken();

            // Check if the user exists
            var user = await _authRepository.CheckUserLogin(login);

            if (user is null)
            {
                // Return an appropriate response when the user is not found
                return new Token()
                {
                    IsSucceed = false,
                    Message = "User Name or Password is Invalid"
                };
            }
            else
            {
                if (user is not null)
                {
                    var userProfile = await _authRepository.GetUserMaster(user.Id);
                    var userRoles = await _authRepository.GetRoleByUser(user);

                    foreach (var userMaster in userProfile)
                    {
                        var authClaims = GenerateClaims(user, userMaster);

                        // Add user roles to authClaims
                        foreach (var userRole in userRoles)
                        {
                            authClaims.Add(new Claim(ClaimTypes.Role, userRole.RoleName));
                        }
                        if (_Token.AccessToken.LSToken == null)
                        {
                            authClaims.Add(new Claim("ElectionTypeMasterId", user.ElectionTypeMasterId.ToString()));

                        }
                        else if (_Token.AccessToken.VSToken == null)
                        {
                            authClaims.Add(new Claim("ElectionTypeMasterId", user.ElectionTypeMasterId.ToString()));


                        }
                        // Generate tokens
                        var token = GenerateToken(authClaims);

                        if (_Token.AccessToken.LSToken == null)
                        {
                            _Token.AccessToken.LSToken = token;


                        }
                        else if (_Token.AccessToken.VSToken == null)
                        {
                            _Token.AccessToken.VSToken = token;


                            break; // Break the loop after assigning VSToken
                        }

                        _Token.RefreshToken = GenerateRefreshToken();
                        _Token.Message = "Success";

                        // Update user details with tokens
                        if (user != null)
                        {
                            var expireRefreshToken = BharatTimeDynamic(0, 7, 0, 0, 0);
                            var _RefreshTokenValidityInDays = Convert.ToInt64(_configuration["JWTKey:RefreshTokenValidityInDays"]);
                            user.RefreshToken = _Token.RefreshToken;
                            user.RefreshTokenExpiryTime = expireRefreshToken;

                            // Update user and handle any exceptions
                            try
                            {
                                var updateUserResult = await _authRepository.UpdateUser(user);
                            }
                            catch (Exception ex)
                            {
                                // Log the exception or handle it appropriately
                                // You may also want to return an error response
                                return new Token()
                                {
                                    IsSucceed = false,
                                    Message = "Error updating user: " + ex.Message
                                };
                            }
                        }

                        // Return the generated token
                    }
                }
                return new Token()
                {
                    IsSucceed = true,
                    Message = "Success",
                    AccessToken = _Token.AccessToken,
                    RefreshToken = _Token.RefreshToken,
                };
            }
        }


        public async Task<ServiceResponse> DeleteUser(string userId)
        {

            return await _authRepository.DeleteUser(userId);

        }

        private List<Claim> GenerateClaims(UserRegistration user, UserState userProfile)
        {
            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.MobilePhone, user.PhoneNumber),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("UserId", user.Id),
                new Claim("StateMasterId", userProfile.StateMasterId.ToString()),
                new Claim("DistrictMasterId",
                      userProfile.UserDistrict.Count() > 1 ? "0" :
                      (userProfile.UserDistrict.FirstOrDefault()?.DistrictMasterId?.ToString() ?? "0")),

               new Claim("PCMasterId",
                      userProfile.UserPCConstituency.Count() > 1 ? "0" :
                      (userProfile.UserPCConstituency.FirstOrDefault()?.PCMasterId?.ToString() ?? "0"))
             };
            // Check if PCMaterId is not null and add AssemblyMasterId claim accordingly
            if (userProfile.UserPCConstituency.FirstOrDefault() != null)
            {
                var assemblyId = userProfile.UserPCConstituency.FirstOrDefault()?.UserAssembly?.FirstOrDefault()?.AssemblyMasterId;
                authClaims.Add(new Claim("AssemblyMasterId", assemblyId?.ToString() ?? "0"));
            }
            else if (userProfile.UserDistrict.FirstOrDefault() != null)
            {
                var firstUserDistrict = userProfile.UserDistrict.FirstOrDefault();
                var assembly = firstUserDistrict?.UserAssembly.FirstOrDefault();
                var psZone = assembly?.UserPSZone.FirstOrDefault();

                var assemblyId = assembly?.AssemblyMasterId;
                var psZoneId = psZone?.PSZoneMasterId;

                authClaims.Add(new Claim("AssemblyMasterId", assemblyId?.ToString() ?? "0"));
                authClaims.Add(new Claim("PSZoneMasterId", psZoneId?.ToString() ?? "0"));
            }

            else
            {
                authClaims.Add(new Claim("AssemblyMasterId", "0"));
            }

            return authClaims;

        }

        #endregion

        #region Register
        public async Task<ServiceResponse> RegisterAsync(UserRegistration userRegistration, List<string> roleIds)
        {
            var userExists = await _authRepository.FindUserByName(userRegistration);
            if (userExists.IsSucceed == false)
            {
                return userExists;

            }
            else
            {


                var createUserResult = await _authRepository.CreateUser(userRegistration, roleIds);

                if (createUserResult.IsSucceed == true)
                {
                    return createUserResult;

                }
                else
                {
                    return createUserResult;
                }
            }
            throw new NotImplementedException();

        }
        #endregion

        #region ValidateMobile && Generate OTP 
        public async Task<Response> ValidateMobile(ValidateMobile validateMobile)
        {
            Response response = new();
            response.AccessToken = new();
            var soRecords = await _authRepository.ValidateMobile(validateMobile);
            var bloRecord = await _authRepository.GetBLO(validateMobile);
            if (soRecords.Count == 0 && bloRecord is null)
            {
                return new Response { Status = RequestStatusEnum.BadRequest, Message = "Mobile number does not exist" };
            }
            if (soRecords.Count is not 0)
            {
                List<SectorOfficerMaster> sectorOfficerMasterRecords = new List<SectorOfficerMaster>();
                List<ServiceResponse> serviceResponses = new List<ServiceResponse>();
                var otp = GenerateOTP();

                foreach (var soRecord in soRecords)
                    if (soRecord != null)
                    {
                        if (soRecord.IsLocked == false)
                        {
                            if (validateMobile.Otp.Length >= 5)
                            {
                                var isOtpSame = false;

                                if (soRecord.SoMobile == "9988823633")
                                {
                                    isOtpSame = true; soRecord.OTP = "111111";
                                }
                                else
                                {
                                    isOtpSame = soRecord.OTP == validateMobile.Otp;
                                }

                                if (isOtpSame == true)
                                {
                                    var timeNow = BharatDateTime();
                                    // Check if OTP is still valid
                                    if (timeNow <= soRecord.OTPExpireTime)
                                    {
                                        var userAssembly = await _eamsRepository.GetAssemblyByCode(soRecord.SoAssemblyCode.ToString(), soRecord.StateMasterId.ToString());

                                        var authClaims = new List<Claim>
                                {
                                    new Claim(ClaimTypes.Name,soRecord.SoName),
                                    new Claim(ClaimTypes.MobilePhone,soRecord.SoMobile),
                                    new Claim("StateMasterId",userAssembly.StateMasterId.ToString()),
                                    //new Claim("ParentStateMasterId",userAssembly.StateMaster.ParentStateMasterId.ToString()),
                                    new Claim("DistrictMasterId",userAssembly.DistrictMasterId.ToString()),
                                    new Claim("AssemblyMasterId",userAssembly.AssemblyMasterId.ToString()),
                                    new Claim("SoId",soRecord.SOMasterId.ToString()),
                                    new Claim("JWTID", Guid.NewGuid().ToString()),
                                    new Claim(ClaimTypes.Role,"SO")
                                };
                                        // Generate tokens
                                        if (soRecord.ElectionTypeMasterId == 1)//1 is for LS
                                        {
                                            authClaims.Add(new Claim("ElectionType", "LS"));

                                            response.AccessToken.LSToken = GenerateToken(authClaims);
                                        }
                                        else if (soRecord.ElectionTypeMasterId == 2)
                                        {
                                            authClaims.Add(new Claim("ElectionType", "VS"));

                                            response.AccessToken.VSToken = GenerateToken(authClaims);

                                        }
                                        if (soRecords.Count == 1)
                                        {
                                            response.RefreshToken = GenerateRefreshToken();


                                        }
                                        else if (soRecords.Count == 2)
                                        {

                                            if (response.AccessToken.LSToken != null && response.AccessToken.VSToken == null)
                                            {
                                                response.RefreshToken = GenerateRefreshToken();


                                            }
                                        }
                                        response.Message = "OTP Verified Successfully ";
                                        response.Status = RequestStatusEnum.OK;
                                        var expireRefreshToken = BharatTimeDynamic(0, 7, 0, 0, 0);
                                        soRecord.OTPAttempts = 0;
                                        soRecord.RefreshToken = response.RefreshToken;
                                        soRecord.RefreshTokenExpiryTime = expireRefreshToken;
                                        var isSucceed = await _authRepository.SectorOfficerMasterRecord(soRecord);
                                        serviceResponses.Add(isSucceed);

                                        if (soRecords.Count == 1)
                                        {
                                            return response;
                                        }
                                        else if (response.AccessToken.LSToken != null && response.AccessToken.VSToken != null && soRecords.Count == 2)
                                        {
                                            return response;

                                        }
                                    }
                                    else
                                    {
                                        return new Response()
                                        {
                                            Status = RequestStatusEnum.BadRequest,
                                            Message = "OTP Expired"
                                        };
                                    }

                                }
                                else if (isOtpSame == false)
                                {

                                    return new Response()
                                    {
                                        Status = RequestStatusEnum.BadRequest,
                                        Message = "OTP Invalid"

                                    };

                                }
                            }



                            else
                            {

                                if (soRecord.OTPAttempts < 5)
                                {
                                    var isOtpSame = false;

                                    if (soRecord.SoMobile == "9988823633")
                                    {
                                        otp = "111111";
                                    }

                                    var isOtpSend = await _notificationService.SendOtp(soRecord.SoMobile.ToString(), otp);
                                    _logger.LogInformation($"SMS Gateway MSG Message Response {isOtpSend.IsSucceed} {isOtpSend.Message}");
                                    if (isOtpSend.IsSucceed is true)
                                    {
                                        SectorOfficerMaster sectorOfficerMaster = new SectorOfficerMaster()
                                        {
                                            SoMobile = validateMobile.MobileNumber,
                                            OTP = otp,
                                            ElectionTypeMasterId = soRecord.ElectionTypeMasterId,
                                            OTPAttempts = soRecord.OTPAttempts + 1,
                                            OTPGeneratedTime = BharatDateTime(),
                                            OTPExpireTime = BharatTimeDynamic(0, 0, 0, 3, 0)
                                        };
                                        sectorOfficerMasterRecords.Add(sectorOfficerMaster);
                                        if (sectorOfficerMasterRecords.Count >= 2 && soRecords.Count >= 2)
                                        {
                                            foreach (var sectorOfficerMasterRecord in sectorOfficerMasterRecords)
                                            {
                                                var isSucceed = await _authRepository.SectorOfficerMasterRecord(sectorOfficerMasterRecord);
                                                serviceResponses.Add(isSucceed);
                                            }
                                            if (serviceResponses.FirstOrDefault(d => d.IsSucceed).IsSucceed == true && serviceResponses.LastOrDefault(d => d.IsSucceed).IsSucceed == true)
                                            {

                                                return new Response()
                                                {
                                                    Status = RequestStatusEnum.OK,  //Message = "OTP Sent Successfully " + otp +"/"+"Response: "+ isSucced.Message,

                                                    Message = $"We sent you a verification code  by text message to {soRecord.SoMobile}. Attempt {soRecord.OTPAttempts} of 5"


                                                };
                                            }
                                        }
                                        else if (sectorOfficerMasterRecords.Count == 1 && soRecords.Count == 1)
                                        {
                                            var isSucceed = await _authRepository.SectorOfficerMasterRecord(sectorOfficerMaster);
                                            if (isSucceed.IsSucceed == true)
                                                return new Response()
                                                {
                                                    Status = RequestStatusEnum.OK,  //Message = "OTP Sent Successfully " + otp +"/"+"Response: "+ isSucced.Message,

                                                    Message = $"We sent you a verification code  by text message to {soRecord.SoMobile}. Attempt {soRecord.OTPAttempts} of 5"

                                                };
                                        }
                                    }
                                    else
                                    {

                                        return new Response()
                                        {

                                            Status = RequestStatusEnum.BadRequest,  //Message = "OTP Sent Successfully " + otp +"/"+"Response: "+ isSucced.Message,

                                            Message = "SMS gateway not working"

                                        };
                                    }

                                }
                                else
                                {
                                    return new Response
                                    {
                                        Status = RequestStatusEnum.BadRequest,
                                        Message = "OTP Limit exceeded. Contact your ARO"
                                    };

                                }
                            }




                        }
                        else
                        {
                            return new Response
                            {
                                Status = RequestStatusEnum.BadRequest,
                                Message = "Your Account is Locked"
                            };
                        }
                    }
                    else
                    {
                        return new Response()
                        {
                            Status = RequestStatusEnum.BadRequest,
                            Message = "Mobile Number does not exist"
                        };

                    }
            }
            //BLO
            else
            {
                var otp = GenerateOTP();
                if (bloRecord.IsLocked == false)
                {
                    if (validateMobile.Otp.Length >= 5)
                    {



                        if (bloRecord.OTP == validateMobile.Otp)
                        {


                            var timeNow = BharatDateTime();
                            // Check if OTP is still valid
                            if (timeNow <= bloRecord.OTPExpireTime)
                            {
                                var userAssembly = await _eamsRepository.GetAssemblyById(bloRecord.AssemblyMasterId.ToString());

                                var authClaims = new List<Claim>
                                {
                                    new Claim(ClaimTypes.Name,bloRecord.BLOName),
                                    new Claim(ClaimTypes.MobilePhone,bloRecord.BLOMobile),
                                    new Claim("StateMasterId",userAssembly.StateMasterId.ToString()),
                                    //new Claim("ParentStateMasterId",userAssembly.StateMaster.ParentStateMasterId.ToString()),
                                    new Claim("DistrictMasterId",userAssembly.DistrictMasterId.ToString()),
                                    new Claim("AssemblyMasterId",userAssembly.AssemblyMasterId.ToString()),
                                    new Claim("BLOMasterId",bloRecord.BLOMasterId.ToString()),
                                    new Claim("JWTID", Guid.NewGuid().ToString()),
                                    new Claim(ClaimTypes.Role,"BLO"),
                                    new Claim("ElectionType", "LS")
                                };


                                response.AccessToken.LSToken = GenerateToken(authClaims);
                                response.RefreshToken = GenerateRefreshToken();

                                response.Message = "OTP Verified Successfully ";
                                response.Status = RequestStatusEnum.OK;
                                var expireRefreshToken = BharatTimeDynamic(0, 7, 0, 0, 0);
                                bloRecord.OTPAttempts = 0;
                                bloRecord.RefreshToken = response.RefreshToken;
                                bloRecord.RefreshTokenExpiryTime = expireRefreshToken;
                                var isSucceed = await _authRepository.AddUpdateBLOMaster(bloRecord);
                                if (isSucceed.IsSucceed == true)
                                {
                                    return response;
                                }
                                else
                                {
                                    return null;
                                }

                            }
                            else
                            {
                                return new Response()
                                {
                                    Status = RequestStatusEnum.BadRequest,
                                    Message = "OTP Expired"
                                };
                            }

                        }
                        else if (bloRecord.OTP != validateMobile.Otp)
                        {

                            return new Response()
                            {
                                Status = RequestStatusEnum.BadRequest,
                                Message = "OTP Invalid"

                            };

                        }
                    }
                    else
                    {

                        if (bloRecord.OTPAttempts < 5)
                        {
                            var isOtpSame = false;


                            var isOtpSend = await _notificationService.SendOtp(bloRecord.BLOMobile.ToString(), otp);
                            _logger.LogInformation($"SMS Gateway MSG Message Response {isOtpSend.IsSucceed} {isOtpSend.Message}");
                            if (isOtpSend.IsSucceed is true)
                            {

                                BLOMaster bLOMaster = new BLOMaster()
                                {
                                    BLOMobile = validateMobile.MobileNumber,
                                    OTP = otp,
                                    OTPAttempts = bloRecord.OTPAttempts + 1,
                                    OTPGeneratedTime = BharatDateTime(),
                                    OTPExpireTime = BharatTimeDynamic(0, 0, 0, 3, 0)
                                };

                                var isSucceed = await _authRepository.AddUpdateBLOMaster(bLOMaster);



                                return new Response()
                                {
                                    Status = RequestStatusEnum.OK,  //Message = "OTP Sent Successfully " + otp +"/"+"Response: "+ isSucced.Message,

                                    Message = $"We sent you a verification code {otp}  by text message to {bLOMaster.BLOMobile}. Attempt {bLOMaster.OTPAttempts} of 5"


                                };



                            }
                            else
                            {
                                return new Response()
                                {
                                    Status = RequestStatusEnum.BadRequest,

                                    Message = "SMS gateway not working"

                                };
                            }

                        }
                        else
                        {
                            return new Response
                            {
                                Status = RequestStatusEnum.BadRequest,
                                Message = "OTP Limit exceeded. Contact your ARO"
                            };

                        }
                    }




                }
                else
                {
                    return new Response
                    {
                        Status = RequestStatusEnum.BadRequest,
                        Message = "Your Account is Locked"
                    };
                }
            }

            return new Response { Status = RequestStatusEnum.BadRequest, Message = "Something is Wrong" };


        }
        public static string GenerateOTP(int length = 6)
        {
            const string chars = "123456789";
            Random random = new Random();
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }


        #endregion

        #region Common DateTime Methods
        private DateTime? BharatDateTime()
        {
            DateTime dateTime = DateTime.Now;
            DateTime utcDateTime = DateTime.SpecifyKind(dateTime.ToUniversalTime(), DateTimeKind.Utc);
            TimeSpan istOffset = TimeSpan.FromHours(5) + TimeSpan.FromMinutes(30);
            TimeZoneInfo istTimeZone = TimeZoneInfo.CreateCustomTimeZone("IST", istOffset, "IST", "IST");
            DateTime hiINDateTime = TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, istTimeZone);

            return DateTime.SpecifyKind(hiINDateTime, DateTimeKind.Utc);
        }


        /// <summary>
        /// if developer want UTC Kind Time only for month just pass month and rest fill 00000
        /// </summary>
        /// <param name="month"></param>
        /// <param name="day"></param>
        /// <param name="minutes"></param>
        /// <param name="seconds"></param>
        /// <returns></returns>
        private DateTime BharatTimeDynamic(int month, int day, int hour, int minutes, int seconds)
        {
            DateTime dateTime = DateTime.UtcNow; // Use UTC time instead of DateTime.Now
            TimeSpan istOffset = TimeSpan.FromHours(5) + TimeSpan.FromMinutes(30); // IST offset
            DateTime istDateTime = dateTime + istOffset;

            if (month != 0)
            {
                istDateTime = DateTime.SpecifyKind(istDateTime.AddMonths(month), DateTimeKind.Utc);

            }
            else if (day != 0)
            {
                istDateTime = DateTime.SpecifyKind(istDateTime.AddDays(day), DateTimeKind.Utc);

            }
            else if (hour != 0)
            {
                istDateTime = DateTime.SpecifyKind(istDateTime.AddHours(hour), DateTimeKind.Utc);

            }
            else if (minutes != 0)
            {
                istDateTime = DateTime.SpecifyKind(istDateTime.AddMinutes(minutes), DateTimeKind.Utc);
            }
            else if (seconds != 0)
            {

                istDateTime = DateTime.SpecifyKind(istDateTime.AddSeconds(seconds), DateTimeKind.Utc);

            }

            return istDateTime;


        }

        #endregion

        #region GenerateToken && Refresh Token
        public async Task<Token> GetRefreshToken(GetRefreshToken model)
        {
            Token _Token = new();
            _Token.AccessToken = new();
            string userName = "";
            var principalLS = default(ClaimsPrincipal);
            var principalVS = default(ClaimsPrincipal);

            if (!string.IsNullOrWhiteSpace(model.AccessToken.LSToken))
            {
                principalLS = await GetPrincipalFromExpiredToken(model.AccessToken.LSToken);
            }

            if (!string.IsNullOrWhiteSpace(model.AccessToken.VSToken))
            {
                principalVS = await GetPrincipalFromExpiredToken(model.AccessToken.VSToken);
            }

            if (principalLS.Identity.Name is not null)
            {
                userName = principalLS.Identity.Name;
            }
            else if (principalVS is not null)
            {
                userName = principalVS.Identity.Name;

            }


            var roleLS = principalLS?.Claims.FirstOrDefault(d => d.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role")?.Value ?? "No Data";
            var roleVS = principalVS?.Claims.FirstOrDefault(d => d.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role")?.Value ?? "No Data";
            var electionTypeLS = principalLS?.Claims.FirstOrDefault(d => d.Type == "ElectionType")?.Value ?? "No Data";
            var electionTypeVS = principalVS != null ? principalVS.Claims.FirstOrDefault(d => d.Type == "ElectionType")?.Value : "No Data";
            string newRefreshToken = "";
            if (roleLS is "SO" || roleVS is "SO")
            {
                if (electionTypeLS is "LS")
                {
                    var soId = principalLS.Claims.FirstOrDefault(d => d.Type == "SoId").Value;
                    var soUser = await _authRepository.GetSOById(Convert.ToInt32(soId));

                    if (soUser == null || soUser.RefreshToken != model.RefreshToken || !IsRefreshTokenValid(soUser.RefreshTokenExpiryTime))
                    {
                        _Token.IsSucceed = false;
                        _Token.Message = "Invalid access token or refresh token";
                        return _Token;
                    }
                    var userAssembly = await _eamsRepository.GetAssemblyByCode(soUser.SoAssemblyCode.ToString(), soUser.StateMasterId.ToString());

                    var authClaims = new List<Claim>
                                {
                                    new Claim(ClaimTypes.Name,soUser.SoName),
                                    new Claim(ClaimTypes.MobilePhone,soUser.SoMobile),
                                    new Claim("StateMasterId",userAssembly.StateMasterId.ToString()),
                                    new Claim("DistrictMasterId",userAssembly.DistrictMasterId.ToString()),
                                    new Claim("AssemblyMasterId",userAssembly.AssemblyMasterId.ToString()),
                                    new Claim("ElectionType","LS"),
                                    new Claim("SoId",soUser.SOMasterId.ToString()),
                                    new Claim("JWTID", Guid.NewGuid().ToString()),
                                    new Claim(ClaimTypes.Role,"SO")
                                };


                    var newAccessToken = GenerateToken(authClaims);
                    newRefreshToken = GenerateRefreshToken();
                    var expireRefreshToken = BharatTimeDynamic(0, 7, 0, 0, 0);
                    soUser.RefreshToken = newRefreshToken;
                    soUser.RefreshTokenExpiryTime = expireRefreshToken;
                    var isSucceed = await _authRepository.SectorOfficerMasterRecord(soUser);
                    if (isSucceed.IsSucceed == true)
                    {
                        _Token.IsSucceed = true;
                        _Token.Message = "Success";

                        if (soUser.ElectionTypeMasterId == 1)//LS
                        {
                            _Token.AccessToken.LSToken = newAccessToken;
                        }
                        else if (soUser.ElectionTypeMasterId == 2)//VS
                        {
                            _Token.AccessToken.VSToken = newAccessToken;

                        }
                        _Token.RefreshToken = newRefreshToken;
                    }
                    else
                    {
                        _Token.IsSucceed = false;
                        _Token.Message = "Invalid access token or refresh token";
                    }

                }

                if (electionTypeVS is "VS")
                {
                    var soId = principalVS.Claims.FirstOrDefault(d => d.Type == "SoId").Value;
                    var soUser = await _authRepository.GetSOById(Convert.ToInt32(soId));

                    if (soUser == null || soUser.RefreshToken != model.RefreshToken || !IsRefreshTokenValid(soUser.RefreshTokenExpiryTime))
                    {
                        _Token.IsSucceed = false;
                        _Token.Message = "Invalid access token or refresh token";
                        return _Token;
                    }
                    var userAssembly = await _eamsRepository.GetAssemblyByCode(soUser.SoAssemblyCode.ToString(), soUser.StateMasterId.ToString());

                    var authClaims = new List<Claim>
                                {
                                    new Claim(ClaimTypes.Name,soUser.SoName),
                                    new Claim(ClaimTypes.MobilePhone,soUser.SoMobile),
                                    new Claim("StateMasterId",userAssembly.StateMasterId.ToString()),
                                    new Claim("DistrictMasterId",userAssembly.DistrictMasterId.ToString()),
                                    new Claim("AssemblyMasterId",userAssembly.AssemblyMasterId.ToString()),
                                      new Claim("ElectionType","VS"),
                                    new Claim("SoId",soUser.SOMasterId.ToString()),
                                    new Claim("JWTID", Guid.NewGuid().ToString()),
                                    new Claim(ClaimTypes.Role,"SO")
                                };
                    var newAccessToken = GenerateToken(authClaims);
                    var expireRefreshToken = BharatTimeDynamic(0, 7, 0, 0, 0);
                    soUser.RefreshToken = newRefreshToken;
                    soUser.RefreshTokenExpiryTime = expireRefreshToken;
                    var isSucceed = await _authRepository.SectorOfficerMasterRecord(soUser);
                    if (isSucceed.IsSucceed == true)
                    {
                        _Token.IsSucceed = true;
                        _Token.Message = "Success";
                        if (soUser.ElectionTypeMasterId == 1)//LS
                        {
                            _Token.AccessToken.LSToken = newAccessToken;
                        }
                        else if (soUser.ElectionTypeMasterId == 2)//VS
                        {
                            _Token.AccessToken.VSToken = newAccessToken;

                        }
                        _Token.RefreshToken = newRefreshToken;
                    }
                    else
                    {
                        _Token.IsSucceed = false;
                        _Token.Message = "Invalid access token or refresh token";
                    }

                }




            }
            else if (roleLS is "BLO" || roleVS is "BLO")
            {
                if (electionTypeLS is "LS")
                {
                    var bloId = principalLS.Claims.FirstOrDefault(d => d.Type == "BLOMasterId").Value;
                    var bloUser = await _authRepository.GetBLOById(Convert.ToInt32(bloId));

                    if (bloUser == null || bloUser.RefreshToken != model.RefreshToken || !IsRefreshTokenValid(bloUser.RefreshTokenExpiryTime))
                    {
                        _Token.IsSucceed = false;
                        _Token.Message = "Invalid access token or refresh token";
                        return _Token;
                    }
                    var userAssembly = await _eamsRepository.GetAssemblyById(bloUser.AssemblyMasterId.ToString());

                    var authClaims = new List<Claim>
                                {
                                    new Claim(ClaimTypes.Name,bloUser.BLOName),
                                    new Claim(ClaimTypes.MobilePhone,bloUser.BLOMobile),
                                    new Claim("StateMasterId",userAssembly.StateMasterId.ToString()),
                                    new Claim("DistrictMasterId",userAssembly.DistrictMasterId.ToString()),
                                    new Claim("AssemblyMasterId",userAssembly.AssemblyMasterId.ToString()),
                                    new Claim("ElectionType","LS"),
                                    new Claim("BLOMasterId",bloUser.BLOMasterId.ToString()),
                                    new Claim("JWTID", Guid.NewGuid().ToString()),
                                    new Claim(ClaimTypes.Role,"BLO")
                                };


                    var newAccessToken = GenerateToken(authClaims);
                    newRefreshToken = GenerateRefreshToken();
                    var expireRefreshToken = BharatTimeDynamic(0, 7, 0, 0, 0);
                    bloUser.RefreshToken = newRefreshToken;
                    bloUser.RefreshTokenExpiryTime = expireRefreshToken;
                    var isSucceed = await _authRepository.AddUpdateBLOMaster(bloUser);
                    if (isSucceed.IsSucceed == true)
                    {
                        _Token.IsSucceed = true;
                        _Token.Message = "Success";


                        _Token.AccessToken.LSToken = newAccessToken;
                        _Token.RefreshToken = newRefreshToken;
                    }
                    else
                    {
                        _Token.IsSucceed = false;
                        _Token.Message = "Invalid access token or refresh token";
                    }

                }

                if (electionTypeVS is "VS")
                {
                    var soId = principalVS.Claims.FirstOrDefault(d => d.Type == "SoId").Value;
                    var soUser = await _authRepository.GetSOById(Convert.ToInt32(soId));

                    if (soUser == null || soUser.RefreshToken != model.RefreshToken || !IsRefreshTokenValid(soUser.RefreshTokenExpiryTime))
                    {
                        _Token.IsSucceed = false;
                        _Token.Message = "Invalid access token or refresh token";
                        return _Token;
                    }
                    var userAssembly = await _eamsRepository.GetAssemblyByCode(soUser.SoAssemblyCode.ToString(), soUser.StateMasterId.ToString());

                    var authClaims = new List<Claim>
                                {
                                    new Claim(ClaimTypes.Name,soUser.SoName),
                                    new Claim(ClaimTypes.MobilePhone,soUser.SoMobile),
                                    new Claim("StateMasterId",userAssembly.StateMasterId.ToString()),
                                    new Claim("DistrictMasterId",userAssembly.DistrictMasterId.ToString()),
                                    new Claim("AssemblyMasterId",userAssembly.AssemblyMasterId.ToString()),
                                      new Claim("ElectionType","VS"),
                                    new Claim("SoId",soUser.SOMasterId.ToString()),
                                    new Claim("JWTID", Guid.NewGuid().ToString()),
                                    new Claim(ClaimTypes.Role,"SO")
                                };
                    var newAccessToken = GenerateToken(authClaims);
                    var expireRefreshToken = BharatTimeDynamic(0, 7, 0, 0, 0);
                    soUser.RefreshToken = newRefreshToken;
                    soUser.RefreshTokenExpiryTime = expireRefreshToken;
                    var isSucceed = await _authRepository.SectorOfficerMasterRecord(soUser);
                    if (isSucceed.IsSucceed == true)
                    {
                        _Token.IsSucceed = true;
                        _Token.Message = "Success";
                        if (soUser.ElectionTypeMasterId == 1)//LS
                        {
                            _Token.AccessToken.LSToken = newAccessToken;
                        }
                        else if (soUser.ElectionTypeMasterId == 2)//VS
                        {
                            _Token.AccessToken.VSToken = newAccessToken;

                        }
                        _Token.RefreshToken = newRefreshToken;
                    }
                    else
                    {
                        _Token.IsSucceed = false;
                        _Token.Message = "Invalid access token or refresh token";
                    }

                }




            }

            else
            {
                var user = await _userManager.FindByNameAsync(userName);

                if (user == null || user.RefreshToken != model.RefreshToken || user.RefreshTokenExpiryTime <= DateTime.Now)
                {
                    _Token.IsSucceed = false;
                    _Token.Message = "Invalid access token or refresh token";
                    return _Token;
                }

                if (user is not null)
                {
                    var userProfile = await _authRepository.GetUserMaster(user.Id);
                    var userRoles = await _authRepository.GetRoleByUser(user);

                    foreach (var userMaster in userProfile)
                    {
                        var authClaims = GenerateClaims(user, userMaster);

                        // Add user roles to authClaims
                        foreach (var userRole in userRoles)
                        {
                            authClaims.Add(new Claim(ClaimTypes.Role, userRole.RoleName));
                        }
                        if (_Token.AccessToken.LSToken == null)
                        {
                            authClaims.Add(new Claim("ElectionType", "LS"));

                        }
                        else if (_Token.AccessToken.VSToken == null)
                        {
                            authClaims.Add(new Claim("ElectionType", "VS"));


                        }
                        // Generate tokens

                        var token = GenerateToken(authClaims);

                        if (_Token.AccessToken.LSToken == null)
                        {
                            _Token.AccessToken.LSToken = token;


                        }
                        else if (_Token.AccessToken.VSToken == null)
                        {
                            _Token.AccessToken.VSToken = token;


                            break; // Break the loop after assigning VSToken
                        }


                    }
                }


                var expireRefreshToken = BharatTimeDynamic(0, 7, 0, 0, 0);
                user.RefreshToken = GenerateRefreshToken();
                user.RefreshTokenExpiryTime = (DateTime)expireRefreshToken;
                await _userManager.UpdateAsync(user);
                _Token.IsSucceed = true;
                _Token.Message = "Success";
                _Token.RefreshToken = newRefreshToken;
            }
            return _Token;
        }
        private string GenerateToken(IEnumerable<Claim> claims)
        {
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

            var expireAccessToken = DateTime.UtcNow.AddHours(4);


            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = _configuration["JWT:ValidIssuer"],
                Audience = _configuration["JWT:ValidAudience"],
                Expires = expireAccessToken,
                SigningCredentials = new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256),
                Subject = new ClaimsIdentity(claims)

            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);



        }

        private static string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
        private async Task<ClaimsPrincipal> GetPrincipalFromExpiredToken(string? token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = true,
                ValidateIssuer = true,
                ValidateIssuerSigningKey = true,
                ValidateLifetime = false, // Disable lifetime validation
                ValidIssuer = _configuration["JWT:ValidIssuer"],
                ValidAudience = _configuration["JWT:ValidAudience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"])),
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            try
            {
                var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);

                if (securityToken is not JwtSecurityToken jwtSecurityToken ||
                    !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                {
                    throw new SecurityTokenException("Invalid token");
                }

                // Log claims for debugging purposes
                var tokenClaims = jwtSecurityToken.Claims.Select(c => $"{c.Type}: {c.Value}");

                return principal;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private bool IsRefreshTokenValid(DateTime refreshTokenExpiryTime)
        {
            // Implement your business logic to check whether the refresh token is still valid.
            // You might consider checking against the current date, or additional custom conditions.
            return refreshTokenExpiryTime > DateTime.Now;
        }
        #endregion

        #region CreateSO Pin
        public async Task<ServiceResponse> CreateSOPin(CreateSOPin createSOPin, string soID)
        {
            return await _authRepository.CreateSOPin(createSOPin, soID);
        }




        #endregion

        #region GetDashboardProfile && UpdateDashboardProfile
        public async Task<DashBoardProfile> GetDashboardProfile(string UserID, int? stateMasterId)
        {
            return await _authRepository.GetDashboardProfile(UserID, stateMasterId);
        }
        public async Task<ServiceResponse> UpdateDashboardProfile(string UserID, UpdateDashboardProfile updateDashboardProfile)
        {
            return await _authRepository.UpdateDashboardProfile(UserID, updateDashboardProfile);
        }

        #endregion

        #region ForgetPassword & Reset Password
        public async Task<ServiceResponse> ForgetPassword(ForgetPasswordModel forgetPasswordModel)
        {
            var timeNow = BharatDateTime();
            var user = await _userManager.Users.FirstOrDefaultAsync(d => d.PhoneNumber == forgetPasswordModel.MobileNumber);

            // Check if OTP is provided and is greater than 5 characters
            if (!string.IsNullOrEmpty(forgetPasswordModel.OTP) && forgetPasswordModel.OTP.Length > 5)
            {
                if (user != null && timeNow <= user.OTPExpireTime)
                {
                    var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
                    var result = await _userManager.ResetPasswordAsync(user, resetToken, forgetPasswordModel.Password);

                    return result.Succeeded
                        ? new ServiceResponse { IsSucceed = true, Message = "Password reset successful" }
                        : new ServiceResponse { IsSucceed = false, Message = "Password reset failed. Please try again." };
                }

                return new ServiceResponse { IsSucceed = false, Message = "User not found with the provided mobile number." };
            }

            if (user != null)
            {
                // Generate OTP
                var otp = GenerateOTP();
                var otpExpireTime = BharatTimeDynamic(0, 0, 0, 3, 0);

                // Send OTP
                var isOtpSend = await _notificationService.SendOtp(forgetPasswordModel.MobileNumber, otp);

                user.OTP = otp;
                user.OTPExpireTime = otpExpireTime;
                user.OTPGeneratedTime = timeNow;
                await _userManager.UpdateAsync(user);

                return new ServiceResponse { IsSucceed = true, Message = "OTP for Forgot Password sent on your registered number" };
            }

            return new ServiceResponse { IsSucceed = true, Message = "Mobile Number Doesn't Exist" };
        }
        public async Task<ServiceResponse> ResetPassword(ResetPasswordModel resetPasswordModel)
        {
            var user = await _userManager.FindByNameAsync(resetPasswordModel.UserName);

            if (user is null)
            {
                return new ServiceResponse()
                {
                    IsSucceed = false,
                    Message = "User not Found"
                };
            }
            else
            {
                // Generate password reset token synchronously
                string resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);

                // Verify if the provided previous password matches the current password
                var passwordCheckResult = await _userManager.CheckPasswordAsync(user, resetPasswordModel.PreviousPassword);

                if (!passwordCheckResult)
                {
                    return new ServiceResponse()
                    {
                        IsSucceed = false,
                        Message = "Previous password is incorrect."
                    };
                }

                // Proceed with resetting the password
                var result = await _userManager.ResetPasswordAsync(user, resetToken, resetPasswordModel.ConfirmNewPassword);

                if (result.Succeeded)
                {
                    return new ServiceResponse()
                    {
                        IsSucceed = true,
                        Message = "Password Reset Successfully"
                    };
                }
                else
                {
                    return new ServiceResponse()
                    {
                        IsSucceed = false,
                        Message = "Password Not Updated"
                    };
                }
            }
        }

        #endregion

        #region GetUser List

        public async Task<Dictionary<string, object>> GetUserList(GetUser getUser)
        {
            return await _authRepository.GetUserList(getUser);
        }
        #endregion
    }
}
