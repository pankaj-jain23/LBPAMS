﻿using AutoMapper;
using EAMS.AuthViewModels;
using EAMS.ViewModels;
using EAMS_ACore;
using EAMS_ACore.AuthInterfaces;
using EAMS_ACore.AuthModels;
using EAMS_ACore.HelperModels;
using EAMS_ACore.Models.ElectionType;
using LBPAMS.AuthViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EAMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EAMSAuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IMapper _mapper;
        private readonly ILogger<EAMSAuthController> _logger;
        public EAMSAuthController(IAuthService authService, IMapper mapper, ILogger<EAMSAuthController> logger)
        {
            _authService = authService;
            _mapper = mapper;
            _logger = logger;

        }

        #region Register
        [HttpPost]
        [Route("Register")]

        public async Task<IActionResult> Register(UserRegistrationViewModel registerViewModel)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest("Invalid payload");
                var mappedData = _mapper.Map<UserRegistration>(registerViewModel);
                var roleId = registerViewModel.RoleId;
                var registerResult = await _authService.RegisterAsync(mappedData, roleId);
                if (registerResult.IsSucceed == false)
                {
                    return BadRequest(registerResult.Message);
                }
                else
                {
                    return Ok(registerResult.Message);
                }

            }
            catch (Exception ex)
            {
                _logger.LogError($" Register: {ex.Message}");

                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
 
        [HttpPut] 
        [Route("SwitchDashboardUser")]
        [Authorize]
        public async Task<IActionResult> SwitchDashboardUser( SwitchDashboardUserViewModel switchDashboardUserViewModel)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                // Assuming you have a logged-in user, you can fetch their ID from the current context
                var userId = User.Claims.FirstOrDefault(c => c.Type == "UserId").Value;

                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized("User doesn't exist.");
                }
                // Validate the new ElectionTypeMasterId
                if (switchDashboardUserViewModel.ElectionTypeMasterId <= 0)
                {
                    return BadRequest("ElectionType doesn't exist.");
                }
                // Call the service to update the mobile number
                var updateResult = await _authService.SwitchDashboardUser(userId, switchDashboardUserViewModel.ElectionTypeMasterId);

                if (!updateResult.IsSucceed)
                {
                    return BadRequest(updateResult);
                }

                return Ok(updateResult);
            }
            catch (Exception ex)
            {
                _logger.LogError($"SwitchDashboardUser: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while Switching Dashboard User.");
            }
        }
       
        #endregion

        #region Login


        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login(LoginViewModel loginViewModel)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest("Invalid payload");
                var mappedData = _mapper.Map<Login>(loginViewModel);

                //var loginResult = await _authService.LoginWithTwoFactorCheckAsync(mappedData);

                var loginResult = await _authService.LoginAsync(mappedData);

                if (loginResult.IsSucceed == false)
                    return BadRequest(loginResult.Message);
                return Ok(loginResult);
            }
            catch (Exception ex)
            {
                _logger.LogError($" Login: {ex.Message}");

                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        #endregion

        #region AddDyanmicRole && Get Role
        [HttpPost]
        [Route("AddDyanmicRole")]
        //[Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> AddDyanmicRole([FromBody] RolesViewModel rolesViewModel)
        {
            try
            {

                if (ModelState.IsValid)
                {
                    var mappedData = _mapper.Map<Role>(rolesViewModel);
                    var roleResult = await _authService.AddDynamicRole(mappedData);

                    if (roleResult.IsSucceed)
                    {
                        // Role creation succeeded, return a success status
                        return Ok(new { Message = roleResult.Message });
                    }
                    else
                    {
                        // Role creation failed, return unauthorized status
                        return Unauthorized(new { Message = roleResult.Message });
                    }

                }


                else
                {
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"AddDyanmicRole: {ex.Message}");
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpGet]
        [Route("GetRoles")]
        [Authorize]
        public async Task<IActionResult> GetRoles()
        {
            try
            {
                var roles = await _authService.GetRoles();

                return Ok(roles);
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while adding dynamic role. Error: {ex.Message}");
                return StatusCode(500, "Internal Server Error");
            }
        }
        #endregion

        #region Validate Mobile 
        [HttpPost]
        [Route("ValidateMobile")]
        public async Task<IActionResult> ValidateMobile(ValidateMobileViewModel validateMobileViewModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var mappedData = _mapper.Map<ValidateMobile>(validateMobileViewModel);
                    var result = await _authService.ValidateMobile(mappedData);

                    switch (result.Status)
                    {
                        case RequestStatusEnum.OK:
                            var response = new
                            {
                                Message = result.Message,
                                AccessToken = result.AccessToken,
                                RefreshToken = result.RefreshToken,
                            };
                            return Ok(response);
                        case RequestStatusEnum.BadRequest:
                            return BadRequest(result.Message);
                        case RequestStatusEnum.NotFound:
                            return NotFound(result.Message);
                        default:
                            return StatusCode(500, "Internal Server Error");
                    }
                }
                else
                {
                    return BadRequest();
                }

            }
            catch (Exception ex)
            {
                _logger.LogError($"ValidateMobile: {ex.Message}");
                return StatusCode(500, "Internal Server Error");
            }

        }


        #endregion

        #region Refresh Token
        [HttpPost]
        [Route("refresh-token")]
        public async Task<IActionResult> RefreshToken(GetRefreshTokenViewModel refreshTokenViewModel)
        {
            try
            {
                if (refreshTokenViewModel is null)
                {
                    return BadRequest("Invalid client request");
                }
                var mapped = _mapper.Map<GetRefreshToken>(refreshTokenViewModel);

                var result = await _authService.GetRefreshToken(mapped);
                if (result.IsSucceed is false)
                    return BadRequest(result.Message);
                else
                    return Ok(result);
            }
            catch (Exception ex)
            {
                 
                return null;
            }
        }
        #endregion

        #region CreateSoPin
        [HttpPost]
        [Route("CreateSOPin")]
        [Authorize(Roles = "SO,SuperAdmin")]
        public async Task<IActionResult> CreateSOPin(CreateSOPinViewModel createSOPinViewModel)
        {
            // Retrieve SoId from the claims
            try
            {
                var soIdClaim = User.Claims.FirstOrDefault(c => c.Type == "SoId");
                if (soIdClaim == null)
                {
                    // Handle the case where the SoId claim is not present
                    return BadRequest("SoId claim not found.");
                }

                var soID = soIdClaim.Value;

                var mappedData = _mapper.Map<CreateSOPin>(createSOPinViewModel);
                var result = await _authService.CreateSOPin(mappedData, soID);
                if (result.IsSucceed is true)
                {
                    return Ok(result);
                }
                else
                {
                    return BadRequest(result);
                }


            }
            catch (Exception ex)
            {
                _logger.LogError($"CreateSOPin: {ex.Message}");
                return StatusCode(500, "Internal Server Error");
            }

        }


        //[HttpPost]
        //[Route("ForgetSOPin")]
        //public async Task<IActionResult> ForgetSOPin()
        //{
        //    return Ok();
        //}
        #endregion

        #region DashBoardProfile
        [HttpGet]
        [Route("GetDashboardProfile")]
        [Authorize]
        public async Task<IActionResult> GetDashboardProfile()
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == "UserId").Value;
            var stateMasterId = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type == "StateMasterId").Value);
            var userRecord = await _authService.GetDashboardProfile(userId, stateMasterId);

            if (userRecord is not null)

                return Ok(userRecord);
            else
                return NotFound();

        }

        [HttpPut]
        [Route("UpdateDashboardProfile")]
        [Authorize]
        public async Task<IActionResult> UpdateDashboardProfile(UpdateDashboardViewModel updateDashboardViewModel)
        {
            if (ModelState.IsValid)
            {
                var soIdClaim = User.Claims.FirstOrDefault(c => c.Type == "UserId");
                var soId = soIdClaim.Value;
                var mappedData = _mapper.Map<UpdateDashboardProfile>(updateDashboardViewModel);

                var userRecord = await _authService.UpdateDashboardProfile(soId, mappedData);

                if (userRecord.IsSucceed == true)

                    return Ok(userRecord.Message);
                else
                    return BadRequest(userRecord.Message);
            }
            else
            {
                return BadRequest();
            }

        }
        #endregion

        #region GetDashboardUsersByRoleId
        [HttpGet]
        [Route("GetDashboardUsersByRoleId")]
        [Authorize]
        public async Task<IActionResult> GetDashboardUsersByRoleId(string roleId)
        {
            var userlist = await _authService.GetUsersByRoleId(roleId);
            //var soId = soIdClaim.Value;
            //var userRecord = await _authService.GetDashboardProfile(soId);
            //if (userRecord is not null)
            //    return Ok(userlist);
            //else
            return Ok(userlist);

        }
        #endregion

        #region ForgetPassword && ResetPasswordViewModel    
        [HttpPost]
        [Route("ForgetPassword")]
        public async Task<IActionResult> ForgetPassword(ForgetPasswordViewModel forgetPassword)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid data provided.");
            }

            var mapped = _mapper.Map<ForgetPasswordModel>(forgetPassword);
            var result = await _authService.ForgetPassword(mapped);

            if (result.IsSucceed)
            {
                return Ok(result.Message);
            }

            return BadRequest(result.Message);
        }

        [HttpPost]
        [Route("ResetPassword")]
        [Authorize]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel resetPasswordViewModel)
        {
            if (ModelState.IsValid)
            {
                var mapped = _mapper.Map<ResetPasswordModel>(resetPasswordViewModel);
                var result = await _authService.ResetPassword(mapped);
                return Ok(result.Message);
            }

            return BadRequest(ModelState);
        }
        #endregion

        #region GetUserList
        [HttpPost]
        [Route("GetUserList")]
        [Authorize]
        public async Task<IActionResult> GetUserList(GetUserViewModel getUserViewModel)
        {
            if (ModelState.IsValid)
            {
                var mappedData = _mapper.Map<GetUser>(getUserViewModel);
                var result = await _authService.GetUserList(mappedData);
                if (result is not null)
                {
                    return Ok(result);
                }
                else
                {
                    return NotFound();
                }
            }
            else
            {
                return BadRequest();
            }

        }
        #endregion

        #region Delete User

        [HttpDelete]
        [Route("DeletePortalUser")]
        [Authorize]
        public async Task<IActionResult> DeleteUser(string userId)
        {
            try
            {

                //var userId = User.Claims.FirstOrDefault(c => c.Type == "UserId").Value;

                // Ensure the user ID is valid
                if (string.IsNullOrEmpty(userId))
                    return BadRequest("Invalid user ID");

                // Find the user by ID

                var result = await _authService.DeleteUser(userId);
                //if (user == null)
                //    return NotFound("User not found");

                //// Delete the user
                //var result = await _authService.DeleteUser(user);
                //if (result.Succeeded)
                //    return Ok("User deleted successfully");

                // Return errors if deletion fails
                var errors = string.Join(", ", result.Message.ToString());
                return BadRequest($"Error deleting user: {errors}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"DeleteUser: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        #endregion

        #region UpdateUserDetail
        [HttpPost]
        [Route("UpdateUserDetail")]
        [Authorize]
        public async Task<IActionResult> UpdateUserDetail([FromBody] UserDetailViewModel userDetailViewModel)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                // Assuming you have a logged-in user, you can fetch their ID from the current context
                var userId = User.Claims.FirstOrDefault(c => c.Type == "UserId").Value;

                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized("User doesn't exist.");
                }

                // Call the service to update the mobile number
                var updateResult = await _authService.UpdateUserDetail(userId, userDetailViewModel.MobileNumber, userDetailViewModel.Otp);

                if (!updateResult.IsSucceed)
                {
                    return BadRequest(updateResult.Message);
                }

                return Ok(updateResult.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError($"UpdateMobileNumber: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while updating the mobile number.");
            }
        }
        #endregion

        #region UpdateFieldOfficerDetail
        [HttpPost]
        [Route("UpdateFieldOfficerDetail")]
        [Authorize]
        public async Task<IActionResult> UpdateFieldOfficerDetail([FromBody] ValidateMobileViewModel validateMobileViewModel)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                // Assuming you have a logged-in user, you can fetch their ID from the current context
                //var userId = User.Claims.FirstOrDefault(c => c.Type == "UserId").Value;

                //if (string.IsNullOrEmpty(userId))
                //{
                //    return Unauthorized("Field Officer doesn't exist.");
                //}

                // Call the service to update the mobile number
                var updateResult = await _authService.UpdateFieldOfficerDetail(validateMobileViewModel.MobileNumber, validateMobileViewModel.Otp);

                if (!updateResult.IsSucceed)
                {
                    return BadRequest(updateResult.Message);
                }

                return Ok(updateResult.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError($"UpdateMobileNumber: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while updating the mobile number.");
            }
        }
        #endregion 

        #region  GetROUserListByAssemblyId
        [HttpGet]
        [Route("GetROUserListByAssemblyId")]
        public async Task<IActionResult> GetROUserListByAssemblyId(int stateMasterId, int districtMasterId, int assemblyMasterId)
        {
            // Get the list of RO users based on the provided parameters
            var roUserList = await _authService.GetROUserListByAssemblyId(stateMasterId, districtMasterId, assemblyMasterId);

            // If no users found, return 404 Not Found
            if (roUserList == null || !roUserList.Any())
            {
                return NotFound(new { Message = "No users with the role RO found for the specified parameters." });
            }

            // Return the list of RO users with a 200 OK response
            return Ok(roUserList);
        }

        #endregion
    }
}
