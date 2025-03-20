using EAMS.Helper;
using EAMS_ACore.AuthInterfaces;
using EAMS_ACore.AuthModels;
using EAMS_ACore.HelperModels;
using EAMS_ACore.IAuthRepository;
using EAMS_ACore.IExternal;
using EAMS_ACore.Interfaces;
using EAMS_ACore.IRepository;
using EAMS_ACore.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace EAMS_BLL.AuthServices
{
    public class AuthService : IAuthService
    {
        private readonly IConfiguration _configuration;
        private readonly IAuthRepository _authRepository;
        private readonly IEamsService _EAMSService;
        private readonly IEamsRepository _eamsRepository;
        private readonly IExternal _external;
        private readonly UserManager<UserRegistration> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<AuthService> _logger;
        public AuthService(IConfiguration configuration, IAuthRepository authRepository, UserManager<UserRegistration> userManager,
            RoleManager<IdentityRole> roleManager, IEamsService eamsService,
            IEamsRepository eamsRepository, IExternal external, ILogger<AuthService> logger)
        {
            _configuration = configuration;
            _authRepository = authRepository;
            _userManager = userManager;
            _roleManager = roleManager;
            _EAMSService = eamsService;
            _eamsRepository = eamsRepository;
            _external = external;
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
            Token _Token = new Token();


            // Check if the user exists
            var user = await _authRepository.CheckUserLogin(login);
           
            var is2FA = await _authRepository.LoginWithTwoFactorCheckAsync(login);
            if (user is null)
            {
                // Return an appropriate response when the user is not found
                return new Token()
                {
                    IsSucceed = false,
                    Message = "User Name or Password is Invalid"
                };
            }
            if (!user.LockoutEnabled)
            {
                return new Token()
                {
                    IsSucceed = false,
                    Message = "User is Disabled Kindly contact your admin"
                };

            }
            var isDashBoardUserValidate = await IsDashBoardUserValidate(login);
            if (!isDashBoardUserValidate.IsSucceed)
            {
                return new Token()
                {
                    IsSucceed = false,
                    IsOtp=true,
                    Message = isDashBoardUserValidate.Message

                };

            }
            var isPasswordExpire = await IsPasswordExpire(login.UserName);
            if (!isPasswordExpire.IsSucceed)
            {
                return new Token()
                {
                    IsSucceed = false,
                    Message = isPasswordExpire.Message

                };

            }

            if (user is not null)
            {

                var authClaims = await GenerateClaims(user);
                var token = GenerateToken(authClaims);

                _Token.RefreshToken = GenerateRefreshToken();

                // Update user details with tokens
                if (user != null)
                {
                    var expireRefreshToken = BharatTimeDynamic(0, 7, 0, 0, 0);
                    var refreshTokenValidityInDays = Convert.ToInt64(_configuration["JWTKey:RefreshTokenValidityInDays"]);
                    user.RefreshToken = _Token.RefreshToken;
                    user.RefreshTokenExpiryTime = expireRefreshToken;
                    user.CurrentToken = token;

                    // Update user and handle any exceptions
                    try
                    {
                        var updateUserResult = await _authRepository.UpdateUser(user);
                    }
                    catch (Exception ex)
                    {
                        // Log the exception or handle it appropriately
                        // You may also want to return an error response
                        return null;
                    }
                }
                _Token.IsSucceed = true;
                _Token.AccessToken = token;
                _Token.Message = $"✨ Welcome, {user.UserName}! Enjoy your experience with LBPAMS! ✨";


            }
            var isNotifyPassword = await NotifyPasswordExpiry(login.UserName);

            return new Token()
            {
                IsSucceed = _Token.IsSucceed,
                Message = isNotifyPassword.IsSucceed ? isNotifyPassword.Message : _Token.Message,
                AccessToken = _Token.AccessToken,
                RefreshToken = _Token.RefreshToken,
                Is2FA = is2FA.IsSucceed,
            };
        }
        public async Task<ServiceResponse> IsDashBoardUserValidate(Login login)
        {

            // Check user in the repository
            var user = await _authRepository.CheckUserLogin(login);

            if (login.Otp is null||login.Otp?.Length < 6)
            {
                // Generate new OTP and update user details
                user.OTP = GenerateOTP();
                user.OTPGeneratedTime = DateTime.UtcNow;
                user.OTPExpireTime = BharatTimeDynamic(0, 0, 0, 1, 0);
                var isOtpSend = await _external.SendSmsAsync(user.PhoneNumber, user.OTP);
                if (isOtpSend.IsSucceed == true)
                {
                    var updateUserResult = await _authRepository.UpdateUser(user);
                    if (updateUserResult.IsSucceed == true)
                    {
                        return new ServiceResponse()
                        {
                            IsSucceed = false,//"false is here Soft True to send otp to user "
                            Message = $"OTP Sent to Registered Mobile Number: {MaskPhoneNumber(user.PhoneNumber)}"
                        };
                    }


                }
                else
                {
                    return isOtpSend;
                }
            }
            else
            {
                if (user.OTP == login.Otp && BharatDateTime() <= user.OTPExpireTime)
                {
                    return new ServiceResponse
                    {
                        IsSucceed = true,
                        Message = "Login successful."
                    };
                }
            }
            return new ServiceResponse
            {
                IsSucceed = false,
                Message = "Invalid or expired OTP."
            };
        }
        private string MaskPhoneNumber(string phoneNumber)
        {
            if (string.IsNullOrWhiteSpace(phoneNumber) || phoneNumber.Length < 4)
            {
                return "InvalidNumber";
            }

            var lastFourDigits = phoneNumber[^4..];
            return $"xxxxxx{lastFourDigits}";
        }
        public async Task<ServiceResponse> IsPasswordExpire(string userName)
        {
            var user = await _authRepository.FindUserByName(userName);
            if (user == null)
            {
                return new ServiceResponse()
                {
                    IsSucceed = false,
                    Message = "UserNotFound"
                };
            }
            else
            {
                if (user.PasswordExpireTime.HasValue && DateTime.Now > user.PasswordExpireTime.Value)
                {
                    return new ServiceResponse()
                    {
                        IsSucceed = false,
                        Message = "Password Expired Kindly Reset it or Try to Forget Password"
                    };
                }

                return new ServiceResponse()
                {
                    IsSucceed = true,
                    Message = "PasswordValid"
                };
            }
        }
        public async Task<ServiceResponse> NotifyPasswordExpiry(string userName)
        {
            var user = await _authRepository.FindUserByName(userName);
            if (user == null)
            {
                return new ServiceResponse()
                {
                    IsSucceed = false,
                    Message = "UserNotFound"
                };
            }
            else
            {
                if (user.PasswordExpireTime.HasValue)
                {
                    var daysUntilExpiry = (user.PasswordExpireTime.Value - DateTime.Now).TotalDays;

                    if (daysUntilExpiry <= 3 && daysUntilExpiry > 0)
                    {
                        return new ServiceResponse()
                        {
                            IsSucceed = true,
                            Message = "⚠️ Reminder: Password will expire soon; please change it promptly! ⚠️"
                        };
                    }
                }

                return new ServiceResponse()
                {
                    IsSucceed = false,
                    Message = "PasswordExpiryNotificationNotNeeded"
                };
            }
        }

        public async Task<ServiceResponse> DeleteUser(string userId)
        {

            return await _authRepository.DeleteUser(userId);

        }

        private async Task<List<Claim>> GenerateClaims(UserRegistration user)
        {
            var getElection = await _authRepository.GetElectionTypeById(user.ElectionTypeMasterId);
            var userRoles = await _authRepository.GetRoleByUser(user);


            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.MobilePhone, user.PhoneNumber),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("ElectionTypeMasterId",user.ElectionTypeMasterId.ToString()),
                new Claim("ElectionName",getElection.ElectionType),
                new Claim("UserId", user.Id),
                new Claim("StateMasterId", user.StateMasterId.ToString()),
                new Claim("DistrictMasterId", user.DistrictMasterId.ToString()),
                new Claim("AssemblyMasterId", user.AssemblyMasterId.ToString()),
                new Claim("FourthLevelHMasterId", user.FourthLevelHMasterId.ToString()),

             };

            foreach (var userRole in userRoles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, userRole.RoleName));
            }
            return authClaims;

        }

        #endregion


        #region Register
        public async Task<AuthServiceResponse> RegisterAsync(UserRegistration userRegistration, List<string> roleIds)
        {

            var userExists = await _authRepository.FindUserByName(userRegistration);
            if (userExists.IsSucceed == false)
            {
                return userExists;

            }
            else
            {
                var passwordValidationResult = ValidatePassword(userRegistration.PasswordHash);
                if (!passwordValidationResult.IsValid)
                {
                    return new AuthServiceResponse
                    {
                        IsSucceed = false,
                        Message = passwordValidationResult.ErrorMessage
                    };
                }

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

        }
        private (bool IsValid, string ErrorMessage) ValidatePassword(string password)
        {
            if (password.Length < 8)
                return (false, "Password must be at least 8 characters long.");

            if (!password.Any(char.IsDigit))
                return (false, "Password must contain at least one digit.");

            if (!password.Any(char.IsLower))
                return (false, "Password must contain at least one lowercase letter.");

            if (!password.Any(char.IsUpper))
                return (false, "Password must contain at least one uppercase letter.");

            if (!password.Any(c => !char.IsLetterOrDigit(c)))
                return (false, "Password must contain at least one non-alphanumeric character.");

            return (true, string.Empty); // Password is valid
        }

        public async Task<ServiceResponse> SwitchDashboardUser(string userId, int electionTypeMasterId)
        {
            return await _authRepository.SwitchDashboardUser(userId, electionTypeMasterId);
        }
        #endregion

        #region ValidateMobile && Generate OTP 
        public async Task<Response> ValidateMobile(ValidateMobile validateMobile)
        {
            var foRecords = await _authRepository.ValidateMobile(validateMobile);
            var aroRecords = await _authRepository.ValidateMobileForARO(validateMobile);

            if (foRecords == null && aroRecords == null)
            {
                return new Response()
                {
                    Status = RequestStatusEnum.BadRequest,
                    Message = "Mobile Number doesn't exist"
                };
            }

            // Check if OTP is empty or not 6 digits
            if (string.IsNullOrEmpty(validateMobile.Otp) || validateMobile.Otp.Length != 6)
            {
                string generatedOtp = GenerateOTP();
                // Check if mobile number is the default number
                if (foRecords != null && foRecords.FieldOfficerMobile == "9988823633")
                {
                    // If the mobile number is the default one, set OTP to "111111"
                    generatedOtp = "111111";
                }
                if (foRecords != null)
                {

                    // Generate a new OTP and update Field Officer's record
                    foRecords.OTP = generatedOtp;
                    foRecords.OTPExpireTime = BharatTimeDynamic(0, 0, 0, 10, 0);
                    foRecords.OTPAttempts += 1;

                    var updateFO = await _eamsRepository.UpdateFieldOfficerValidate(foRecords);
                    if (updateFO.Status == RequestStatusEnum.OK)
                    {
                        // Send OTP via SMS
                        var sendOtpResponse = await _external.SendSmsAsync(foRecords.FieldOfficerMobile, foRecords.OTP);
                        if (sendOtpResponse.IsSucceed)
                        {
                            return new Response { Status = RequestStatusEnum.OK, Message = $"OTP Sent to {foRecords.FieldOfficerMobile}" };
                        }
                        //  return new Response { Status = RequestStatusEnum.OK, Message = $"OTP Sent to {foRecords.OTP}" };
                        return new Response { Status = RequestStatusEnum.BadRequest, Message = "Failed to send OTP" };
                    }
                    return new Response { Status = RequestStatusEnum.BadRequest, Message = "Failed to update OTP" };
                }
                else if (aroRecords != null)
                {
                    // Generate a new OTP and update ARO's record
                    aroRecords.OTP = generatedOtp;
                    aroRecords.OTPExpireTime = BharatTimeDynamic(0, 0, 0, 10, 0);
                    aroRecords.OTPAttempts += 1;

                    var updateARO = await _eamsRepository.UpdateAROValidate(aroRecords);
                    if (updateARO.Status == RequestStatusEnum.OK)
                    {
                        // Send OTP via SMS
                        var sendOtpResponse = await _external.SendSmsAsync(aroRecords.AROMobile, aroRecords.OTP);
                        if (sendOtpResponse.IsSucceed)
                        {
                            return new Response { Status = RequestStatusEnum.OK, Message = $"OTP Sent to {aroRecords.AROMobile}" };
                        }
                        // return new Response { Status = RequestStatusEnum.OK, Message = $"OTP Sent to {aroRecords.OTP}" };

                        return new Response { Status = RequestStatusEnum.BadRequest, Message = "Failed to send OTP" };
                    }
                    return new Response { Status = RequestStatusEnum.BadRequest, Message = "Failed to update OTP" };
                }
            }

            // Validate OTP and expiration time for Field Officer
            if (foRecords != null && foRecords.OTP == validateMobile.Otp && BharatDateTime() <= foRecords.OTPExpireTime)
            {
                foRecords.OTPAttempts = 0;
                foRecords.RefreshToken = GenerateRefreshToken();
                foRecords.RefreshTokenExpiryTime = BharatTimeDynamic(0, 7, 0, 0, 0);

                var updateFO = await _eamsRepository.UpdateFieldOfficerValidate(foRecords);
                if (updateFO.Status == RequestStatusEnum.BadRequest)
                {
                    return new Response { Status = RequestStatusEnum.BadRequest, Message = "Failed to update mobile number" };
                }

                var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, foRecords.FieldOfficerName),
                    new Claim(ClaimTypes.MobilePhone, foRecords.FieldOfficerMobile),
                    new Claim(ClaimTypes.Role, "FO"),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim("ElectionTypeMasterId", foRecords.ElectionTypeMasterId.ToString()),
                    new Claim("FieldOfficerMasterId", foRecords.FieldOfficerMasterId.ToString()),
                    new Claim("StateMasterId", foRecords.StateMasterId.ToString()),
                    new Claim("DistrictMasterId", foRecords.DistrictMasterId.ToString()),
                    new Claim("AssemblyMasterId", foRecords.AssemblyMasterId.ToString())
                };

                var token = GenerateToken(authClaims);

                return new Response
                {
                    Status = RequestStatusEnum.OK,
                    Message = "Mobile number updated successfully",
                    AccessToken = token,
                    RefreshToken = foRecords.RefreshToken
                };
            }

            // Validate OTP and expiration time for ARO
            if (aroRecords != null && aroRecords.OTP == validateMobile.Otp && BharatDateTime() <= aroRecords.OTPExpireTime)
            {
                aroRecords.OTPAttempts = 0;
                aroRecords.RefreshToken = GenerateRefreshToken();
                aroRecords.RefreshTokenExpiryTime = BharatTimeDynamic(0, 7, 0, 0, 0);

                var updateARO = await _eamsRepository.UpdateAROValidate(aroRecords);
                if (updateARO.Status == RequestStatusEnum.BadRequest)
                {
                    return new Response { Status = RequestStatusEnum.BadRequest, Message = "Failed to update mobile number" };
                }

                var authClaims = new List<Claim>
                            {
                                new Claim(ClaimTypes.Name, aroRecords.AROName),
                                new Claim(ClaimTypes.MobilePhone, aroRecords.AROMobile),
                                new Claim(ClaimTypes.Role, "ARO"),
                                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                                new Claim("ElectionTypeMasterId", aroRecords.ElectionTypeMasterId.ToString()),
                                new Claim("AROMasterId", aroRecords.AROMasterId.ToString()),
                                new Claim("StateMasterId", aroRecords.StateMasterId.ToString()),
                                new Claim("DistrictMasterId", aroRecords.DistrictMasterId.ToString()),
                                new Claim("AssemblyMasterId", aroRecords.AssemblyMasterId?.ToString())
                            };

                var token = GenerateToken(authClaims);

                return new Response
                {
                    Status = RequestStatusEnum.OK,
                    Message = "Mobile number updated successfully",
                    AccessToken = token,
                    RefreshToken = aroRecords.RefreshToken
                };
            }

            // OTP validation failed
            return new Response
            {
                Status = RequestStatusEnum.BadRequest,
                Message = "OTP Expired or Invalid"
            };
        }
        //public async Task<Response> ValidateMobile(ValidateMobile validateMobile)
        //{
        //    var foRecords = await _authRepository.ValidateMobile(validateMobile);
        //    var aroRecords = await _authRepository.ValidateMobileForARO(validateMobile);

        //    if (foRecords == null && aroRecords == null)
        //    {
        //        return new Response()
        //        {
        //            Status = RequestStatusEnum.BadRequest,
        //            Message = "Mobile Number doesn't Exist"
        //        };
        //    }

        //    // Check if OTP is empty or not 6 digits
        //    if (string.IsNullOrEmpty(validateMobile.Otp) || validateMobile.Otp.Length != 6)
        //    {
        //        if (foRecords != null)
        //        {
        //            // Generate a new OTP and update the field officer's record
        //            foRecords.OTP = GenerateOTP();
        //            foRecords.OTPExpireTime = BharatTimeDynamic(0, 0, 0, 0, 60);
        //            foRecords.OTPAttempts += 1;

        //            var updateFO = await _eamsRepository.UpdateFieldOfficerValidate(foRecords);
        //            if (updateFO.Status == RequestStatusEnum.OK)
        //            {
        //                return new Response { Status = RequestStatusEnum.OK, Message = $"OTP Sent on your number {foRecords.OTP}" };
        //            }
        //            return new Response { Status = RequestStatusEnum.BadRequest, Message = "Failed to send OTP" };
        //        }
        //        else if (aroRecords != null)
        //        {
        //            // Generate a new OTP and update the ARO's record
        //            aroRecords.OTP = GenerateOTP();
        //            aroRecords.OTPExpireTime = BharatTimeDynamic(0, 0, 0, 0, 60);
        //            aroRecords.OTPAttempts += 1;

        //            var updateARO = await _eamsRepository.UpdateAROValidate(aroRecords);
        //            if (updateARO.Status == RequestStatusEnum.OK)
        //            {
        //                return new Response { Status = RequestStatusEnum.OK, Message = $"OTP Sent on your number {aroRecords.OTP}" };
        //            }
        //            return new Response { Status = RequestStatusEnum.BadRequest, Message = "Failed to send OTP" };
        //        }
        //    }

        //    // Validate OTP and expiration time
        //    if (foRecords != null && foRecords.OTP == validateMobile.Otp && BharatDateTime() <= foRecords.OTPExpireTime)
        //    {
        //        foRecords.OTPAttempts = 0;
        //        foRecords.RefreshToken = GenerateRefreshToken();
        //        foRecords.RefreshTokenExpiryTime = BharatTimeDynamic(0, 7, 0, 0, 0);

        //        var updateFO = await _eamsRepository.UpdateFieldOfficerValidate(foRecords);
        //        if (updateFO.Status == RequestStatusEnum.BadRequest)
        //        {
        //            return new Response { Status = RequestStatusEnum.BadRequest, Message = "Failed to update mobile number" };
        //        }

        //        var authClaims = new List<Claim>
        //{
        //    new Claim(ClaimTypes.Name, foRecords.FieldOfficerName),
        //    new Claim(ClaimTypes.MobilePhone, foRecords.FieldOfficerMobile),
        //    new Claim(ClaimTypes.Role, "FO"),
        //    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        //    new Claim("ElectionTypeMasterId", foRecords.ElectionTypeMasterId.ToString()),
        //    new Claim("FieldOfficerMasterId", foRecords.FieldOfficerMasterId.ToString()),
        //    new Claim("StateMasterId", foRecords.StateMasterId.ToString()),
        //    new Claim("DistrictMasterId", foRecords.DistrictMasterId.ToString()),
        //    new Claim("AssemblyMasterId", foRecords.AssemblyMasterId.ToString())
        //};

        //        var token = GenerateToken(authClaims);

        //        return new Response
        //        {
        //            Status = RequestStatusEnum.OK,
        //            Message = "Mobile number updated successfully",
        //            AccessToken = token,
        //            RefreshToken = foRecords.RefreshToken
        //        };
        //    }
        //    else if (aroRecords != null && aroRecords.OTP == validateMobile.Otp && BharatDateTime() <= aroRecords.OTPExpireTime)
        //    {
        //        aroRecords.OTPAttempts = 0;
        //        aroRecords.RefreshToken = GenerateRefreshToken();
        //        aroRecords.RefreshTokenExpiryTime = BharatTimeDynamic(0, 7, 0, 0, 0);

        //        var updateARO = await _eamsRepository.UpdateAROValidate(aroRecords);
        //        if (updateARO.Status == RequestStatusEnum.BadRequest)
        //        {
        //            return new Response { Status = RequestStatusEnum.BadRequest, Message = "Failed to update mobile number" };
        //        }

        //        var authClaims = new List<Claim>
        //{
        //    new Claim(ClaimTypes.Name, aroRecords.AROName),
        //    new Claim(ClaimTypes.MobilePhone, aroRecords.AROMobile),
        //    new Claim(ClaimTypes.Role, "ARO"),
        //    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        //    new Claim("ElectionTypeMasterId", aroRecords.ElectionTypeMasterId.ToString()),
        //    new Claim("AROMasterId", aroRecords.AROMasterId.ToString()),
        //    new Claim("StateMasterId", aroRecords.StateMasterId.ToString()),
        //    new Claim("DistrictMasterId", aroRecords.DistrictMasterId.ToString()),
        //    new Claim("AssemblyMasterId", aroRecords.AssemblyMasterId?.ToString()),
        //    new Claim("FourthLevelHMasterId", aroRecords.FourthLevelHMasterId?.ToString())
        //};

        //        var token = GenerateToken(authClaims);

        //        return new Response
        //        {
        //            Status = RequestStatusEnum.OK,
        //            Message = "Mobile number updated successfully",
        //            AccessToken = token,
        //            RefreshToken = aroRecords.RefreshToken
        //        };
        //    }

        //    // OTP validation failed
        //    return new Response
        //    {
        //        Status = RequestStatusEnum.BadRequest,
        //        Message = "OTP Expired or Invalid"
        //    };
        //}

        //public async Task<Response> ValidateMobile(ValidateMobile validateMobile)
        //{
        //    var foRecords = await _authRepository.ValidateMobile(validateMobile);
        //    var aroRecords = await _authRepository.ValidateMobileForARO(validateMobile);
        //    if (foRecords == null && aroRecords == null)
        //    {
        //        return new Response()
        //        {
        //            Status = RequestStatusEnum.BadRequest,
        //            Message = "Mobile Number doesn't Exist"
        //        };
        //    }


        //    // Check if OTP is empty or not 6 digits
        //    if (string.IsNullOrEmpty(validateMobile.Otp) || validateMobile.Otp.Length != 6)
        //    {
        //        // Generate a new OTP and update the field officer's record
        //        foRecords.OTP = GenerateOTP();
        //        foRecords.OTPExpireTime = BharatTimeDynamic(0, 0, 0, 0, 60);  // Set OTP expiration time
        //        foRecords.OTPAttempts = foRecords.OTPAttempts + 1;


        //        var updateFO = await _eamsRepository.UpdateFieldOfficerValidate(foRecords);
        //        // Check if OTP send was successful
        //        if (updateFO.Status == RequestStatusEnum.OK)
        //        {
        //            // await _authRepository.UpdateUserAsync(fieldOfficer);  // Ensure the field officer's record is updated with OTP
        //            return new Response { Status = RequestStatusEnum.OK, Message = $"OTP Sent on your number {foRecords.OTP}" };
        //        }

        //        // OTP send failed
        //        return new Response { Status = RequestStatusEnum.BadRequest, Message = "Failed to send OTP" };
        //    }

        //    // Validate OTP and expiration time
        //    if (foRecords.OTP == validateMobile.Otp && BharatDateTime() <= foRecords.OTPExpireTime)
        //    {
        //        foRecords.OTPAttempts = 0;
        //        foRecords.RefreshToken = GenerateRefreshToken();
        //        foRecords.RefreshTokenExpiryTime = BharatTimeDynamic(0, 7, 0, 0, 0);

        //        var updateFO = await _eamsRepository.UpdateFieldOfficerValidate(foRecords);


        //        if (updateFO.Status == RequestStatusEnum.BadRequest)
        //        {
        //            return new Response { Status = RequestStatusEnum.BadRequest, Message = "Failed to update mobile number" };
        //        }
        //        var authClaims = new List<Claim>
        //        {
        //            new Claim(ClaimTypes.Name, foRecords.FieldOfficerName),
        //            new Claim(ClaimTypes.MobilePhone, foRecords.FieldOfficerMobile),
        //            new Claim(ClaimTypes.Role,"FO"),
        //            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        //            new Claim("ElectionTypeMasterId",foRecords.ElectionTypeMasterId.ToString()),
        //            new Claim("FieldOfficerMasterId", foRecords.FieldOfficerMasterId.ToString()),
        //            new Claim("StateMasterId", foRecords.StateMasterId.ToString()),
        //            new Claim("DistrictMasterId", foRecords.DistrictMasterId.ToString()),
        //            new Claim("AssemblyMasterId", foRecords.AssemblyMasterId.ToString())

        //        };
        //        var token = GenerateToken(authClaims);

        //        return new Response { Status = RequestStatusEnum.OK, Message = "Mobile number updated successfully", AccessToken = token,RefreshToken=foRecords.RefreshToken };
        //    }

        //    // OTP validation failed
        //    return new Response { Status = RequestStatusEnum.BadRequest, Message = "OTP Expired or Invalid" };



        //}


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
            ClaimsPrincipal? principal = null;

            if (!string.IsNullOrWhiteSpace(model.AccessToken))
            {
                principal = GetPrincipalFromExpiredToken(model.AccessToken);
            }
            var role = principal.Claims.FirstOrDefault(d => d.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role").Value;
            if (role.Contains("FO"))
            {
                var fieldOfficerMasterId = principal.Claims.FirstOrDefault(d => d.Type == "FieldOfficerMasterId").Value;

                var foRecords = await _authRepository.GetFOById(Convert.ToInt32(fieldOfficerMasterId));
                if (foRecords == null || foRecords.RefreshToken != model.RefreshToken || DateTime.Compare(foRecords.RefreshTokenExpiryTime, (DateTime)BharatDateTime()) <= 0)
                {
                    return new Token
                    {
                        Message = "Token Expired or Invalid Token"
                    };
                }
                if (foRecords != null)
                {

                    foRecords.RefreshToken = GenerateRefreshToken();
                    foRecords.RefreshTokenExpiryTime = BharatTimeDynamic(0, 7, 0, 0, 0);
                    var updateUser = await _eamsRepository.UpdateFieldOfficerValidate(foRecords);
                    var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, foRecords.FieldOfficerName),
                    new Claim(ClaimTypes.MobilePhone, foRecords.FieldOfficerMobile),
                    new Claim(ClaimTypes.Role,"FO"),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim("ElectionTypeMasterId",foRecords.ElectionTypeMasterId.ToString()),
                    new Claim("FieldOfficerMasterId", foRecords.FieldOfficerMasterId.ToString()),
                    new Claim("StateMasterId", foRecords.StateMasterId.ToString()),
                    new Claim("DistrictMasterId", foRecords.DistrictMasterId.ToString()),
                    new Claim("AssemblyMasterId", foRecords.AssemblyMasterId.ToString())

                };
                    var getAccessToken = GenerateToken(authClaims);
                    if (updateUser.Status == RequestStatusEnum.OK)
                    {
                        _Token.IsSucceed = true;
                        _Token.Is2FA = true;
                        _Token.AccessToken = getAccessToken;
                        _Token.RefreshToken = foRecords.RefreshToken;
                    }
                }
            }
            else if (role.Contains("ARO"))
            {
                var rorMasterId = principal.Claims.FirstOrDefault(d => d.Type == "AROMasterId").Value;

                var roRecords = await _authRepository.GetAROById(Convert.ToInt32(rorMasterId));
                var todayDate = BharatDateTime();
                // Check if the RefreshToken matches first
                if (roRecords == null || roRecords.RefreshToken != model.RefreshToken)
                {
                    return new Token
                    {
                        Message = "Invalid Token"
                    };
                }

                // Check if the RefreshTokenExpiryTime is less than today's date
                if (roRecords.RefreshTokenExpiryTime < todayDate)
                {
                    return new Token
                    {
                        Message = "Token Expired"
                    };
                }
                if (roRecords != null)
                {

                    roRecords.RefreshToken = GenerateRefreshToken();
                    roRecords.RefreshTokenExpiryTime = BharatTimeDynamic(0, 7, 0, 0, 0);
                    var updateUser = await _eamsRepository.UpdateAROValidate(roRecords);

                    var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, roRecords.AROName),
                    new Claim(ClaimTypes.MobilePhone, roRecords.AROMobile),
                    new Claim(ClaimTypes.Role,"ARO"),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim("ElectionTypeMasterId",roRecords.ElectionTypeMasterId.ToString()),
                    new Claim("AROMasterId", roRecords.AROMasterId.ToString()),
                    new Claim("StateMasterId", roRecords.StateMasterId.ToString()),
                    new Claim("DistrictMasterId", roRecords.DistrictMasterId.ToString()),
                    new Claim("AssemblyMasterId", roRecords.AssemblyMasterId.ToString()),
                    new Claim("FourthLevelHMasterId", roRecords.FourthLevelHMasterId.ToString())

                };
                    var getAccessToken = GenerateToken(authClaims);
                    if (updateUser.Status == RequestStatusEnum.OK)
                    {
                        _Token.IsSucceed = true;
                        _Token.Is2FA = true;
                        _Token.AccessToken = getAccessToken;
                        _Token.RefreshToken = roRecords.RefreshToken;
                    }
                }
            }
            else
            {
                var userId = principal.Claims.FirstOrDefault(d => d.Type == "UserId").Value;
                var getCurrentUser = await _authRepository.GetUserById(userId);
                if (getCurrentUser == null || getCurrentUser.RefreshToken != model.RefreshToken || DateTime.Compare(getCurrentUser.RefreshTokenExpiryTime, (DateTime)BharatDateTime()) <= 0)
                {
                    return new Token
                    {
                        Message = "Token Expired or Invalid Token"
                    };
                }
                if (getCurrentUser != null)
                {

                    var getClaims = await GenerateClaims(getCurrentUser);
                    var getAccessToken = GenerateToken(getClaims);
                    getCurrentUser.CurrentToken = getAccessToken;
                    getCurrentUser.RefreshToken = GenerateRefreshToken();
                    getCurrentUser.RefreshTokenExpiryTime = BharatTimeDynamic(0, 7, 0, 0, 0);
                    var updateUser = await _authRepository.UpdateUser(getCurrentUser);

                    if (updateUser.IsSucceed == true)
                    {
                        _Token.IsSucceed = true;
                        _Token.Is2FA = true;
                        _Token.AccessToken = getAccessToken;
                        _Token.RefreshToken = getCurrentUser.RefreshToken;
                    }
                }

            }

            return _Token;
        }

        private string GenerateToken(IEnumerable<Claim> claims)
        {
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

            // var expireAccessToken = BharatTimeDynamic(0, 0, 0, 1, 0); // Your method for setting expiration time

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = _configuration["JWT:ValidIssuer"],
                Audience = _configuration["JWT:ValidAudience"],
                Expires = DateTime.Now.AddHours(4),
                SigningCredentials = new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256),
                Subject = new ClaimsIdentity(claims),
            };

            // Optionally add 'kid' to the token header for key rotation support
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateJwtSecurityToken(
                issuer: tokenDescriptor.Issuer,
                audience: tokenDescriptor.Audience,
                subject: tokenDescriptor.Subject,
                expires: tokenDescriptor.Expires,
                signingCredentials: tokenDescriptor.SigningCredentials);


            return tokenHandler.WriteToken(token);
        }


        private static string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        private ClaimsPrincipal? GetPrincipalFromExpiredToken(string? token)
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
                    return null;
                }

                return principal;
            }
            catch (Exception ex)
            {
                return null; // Avoid throwing exceptions to prevent memory leaks
            }
        }
        private bool IsRefreshTokenValid(DateTime refreshTokenExpiryTime)
        {
            // Implement your business logic to check whether the refresh token is still valid.
            // You might consider checking against the current date, or additional custom conditions.
            return refreshTokenExpiryTime > BharatDateTime();
        }
        #endregion

        #region CreateSO Pin
        public async Task<ServiceResponse> CreateSOPin(CreateSOPin createSOPin, string soID)
        {
            return await _authRepository.CreateSOPin(createSOPin, soID);
        }




        #endregion

        #region GetDashboardProfile && UpdateDashboardProfile
        public async Task<DashBoardProfile> GetDashboardProfile(string userID, int? stateMasterId)
        {
            return await _authRepository.GetDashboardProfile(userID, stateMasterId);
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
            var user = await _userManager.Users.FirstOrDefaultAsync(d => d.PhoneNumber == forgetPasswordModel.MobileNumber&&d.UserName==forgetPasswordModel.UserName);

            // Check if OTP is provided and is greater than 5 characters
            if (!string.IsNullOrEmpty(forgetPasswordModel.OTP) && forgetPasswordModel.OTP.Length > 5)
            {
                if (user != null && timeNow <= user.OTPExpireTime)
                {
                    var passwordValidationResult = ValidatePassword(forgetPasswordModel.ConfirmPassword);
                    if (!passwordValidationResult.IsValid)
                    {
                        return new ServiceResponse
                        {
                            IsSucceed = false,
                            Message = passwordValidationResult.ErrorMessage
                        };
                    }
                    var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
                    var result = await _userManager.ResetPasswordAsync(user, resetToken, forgetPasswordModel.Password);

                    return result.Succeeded
                        ? new ServiceResponse { IsSucceed = true, Message = $"Password reset successful" }
                        : new ServiceResponse { IsSucceed = false, Message = $"{result}" };
                }


            }

            if (user != null)
            {
                // Generate OTP
                var otp = GenerateOTP();
                var otpExpireTime = BharatTimeDynamic(0, 0, 0, 3, 0);

                // Send OTP
                var isOtpSend = await _external.SendSmsAsync(forgetPasswordModel.MobileNumber, otp);

                user.OTP = otp;
                user.OTPExpireTime = otpExpireTime;
                user.OTPGeneratedTime = timeNow;
                user.PasswordExpireTime = DateTime.UtcNow.AddDays(15);
                user.IsPasswordExpire = false;
                await _userManager.UpdateAsync(user);

                return new ServiceResponse { IsSucceed = true, Message = "OTP for Forgot Password sent on your registered number" };
            }

            return new ServiceResponse { IsSucceed = false, Message = "User not found with the provided mobile number." };
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
                var passwordValidationResult = ValidatePassword(resetPasswordModel.NewPassword);
                if (!passwordValidationResult.IsValid)
                {
                    return new ServiceResponse
                    {
                        IsSucceed = false,
                        Message = passwordValidationResult.ErrorMessage
                    };
                }
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
                user.PasswordExpireTime = DateTime.UtcNow.AddDays(15);
                user.IsPasswordExpire = false;
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
        public async Task<bool> UpdateLockoutUser(UpdateLockoutUser updateLockoutUser)
        {
            return await _authRepository.UpdateLockoutUser(updateLockoutUser);
        }
        public async Task<int> UpdateLockoutUserInBulk(UpdateLockoutUserInBulk updateLockoutUser)
        {
            return await _authRepository.UpdateLockoutUserInBulk(updateLockoutUser);
        }

        #endregion

        #region UpdateUserDetail
        public async Task<ServiceResponse> UpdateUserDetail(string userId, string mobileNumber, string? otp)
        {
            // Fetch the user by userId
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return new ServiceResponse { IsSucceed = false, Message = "User not found" };
            }

            // Check if OTP is empty or not 6 digits
            if (string.IsNullOrEmpty(otp) || otp.Length != 6)
            {
                // Generate a new OTP and update the user record
                var generatedOtp = GenerateOTP();
                user.OTP = generatedOtp;
                user.OTPExpireTime = BharatTimeDynamic(0, 0, 0, 10, 0);
                // var otpRecord = await _notificationService.SendOtp(mobileNumber, generatedOtp);
                // Simulating OTP send and response
                var otpRecord = new ServiceResponse
                {
                    IsSucceed = true,
                    Message = "otp"
                };

                if (otpRecord.IsSucceed)
                {
                    await _userManager.UpdateAsync(user);
                    return new ServiceResponse { IsSucceed = true, Message = "OTP Sent on your number" };
                }

                return new ServiceResponse { IsSucceed = false, Message = "Failed to send OTP" };
            }

            // Validate OTP and expiration time
            if (user.OTP == otp && BharatDateTime() <= user.OTPExpireTime)
            {
                // Update mobile number and enable two-factor authentication
                user.PhoneNumber = mobileNumber;
                user.TwoFactorEnabled = true;
                var updateResult = await _userManager.UpdateAsync(user);

                if (!updateResult.Succeeded)
                {
                    return new ServiceResponse { IsSucceed = false, Message = "Failed to update mobile number" };
                }

                return new ServiceResponse { IsSucceed = true, Message = "Mobile number updated successfully" };
            }

            return new ServiceResponse { IsSucceed = false, Message = "OTP Expired or Invalid" };
        }
        #endregion

        #region UpdateFieldOfficerDetail
        public async Task<ServiceResponse> UpdateFieldOfficerDetail(string mobileNumber, int? otp)
        {
            // Fetch the user by userId
            //var user = await _userManager.FindByIdAsync(userId);
            //if (user == null)
            //{
            //    return new ServiceResponse { IsSucceed = false, Message = "User not found" };
            //}

            //// Check if OTP is empty or not 6 digits
            //if (string.IsNullOrEmpty(otp) || otp.Length != 6)
            //{
            //    // Generate a new OTP and update the user record
            //    var generatedOtp = GenerateOTP();
            //    user.OTP = generatedOtp;
            //    user.OTPExpireTime = BharatTimeDynamic(0, 0, 0, 0, 60);
            //    // var otpRecord = await _notificationService.SendOtp(mobileNumber, generatedOtp);
            //    // Simulating OTP send and response
            //    var otpRecord = new ServiceResponse
            //    {
            //        IsSucceed = true,
            //        Message = "otp"
            //    };

            //    if (otpRecord.IsSucceed)
            //    {
            //        await _userManager.UpdateAsync(user);
            //        return new ServiceResponse { IsSucceed = true, Message = "OTP Sent on your number" };
            //    }

            //    return new ServiceResponse { IsSucceed = false, Message = "Failed to send OTP" };
            //}

            //// Validate OTP and expiration time
            //if (user.OTP == otp && BharatDateTime() <= user.OTPExpireTime)
            //{
            //    // Update mobile number and enable two-factor authentication
            //    user.PhoneNumber = mobileNumber;
            //    user.TwoFactorEnabled = true;
            //    var updateResult = await _userManager.UpdateAsync(user);

            //    if (!updateResult.Succeeded)
            //    {
            //        return new ServiceResponse { IsSucceed = false, Message = "Failed to update mobile number" };
            //    }

            //    return new ServiceResponse { IsSucceed = true, Message = "Mobile number updated successfully" };
            //}

            return new ServiceResponse { IsSucceed = false, Message = "OTP Expired or Invalid" };
        }


        #endregion


        #region
        public async Task<List<ROUserList>> GetROUserListByAssemblyId(int stateMasterId, int districtMasterId, int assemblyMasterId)
        {
            // Get all users with the role "RO"
            var roRoleUsers = await _userManager.GetUsersInRoleAsync("RO");

            // Filter by state, district, and assembly master ID
            var filteredUsers = roRoleUsers
                .Where(u => u.StateMasterId == stateMasterId
                            && u.DistrictMasterId == districtMasterId
                            && u.AssemblyMasterId == assemblyMasterId)
                .Select(d => new ROUserList
                {
                    StateMasterId = d.StateMasterId,
                    DistrictMasterId = d.DistrictMasterId,
                    AssemblyMasterId = d.AssemblyMasterId,
                    ElectionTypeMasterId = d.ElectionTypeMasterId,
                    FourthLevelHMasterId = d.FourthLevelHMasterId,
                    UserName = d.UserName,
                    Id = d.Id,
                    PhoneNumber = d.PhoneNumber

                }).OrderBy(d => d.UserName)
                .ToList();

            return filteredUsers;
        }



        #endregion
    }
}
