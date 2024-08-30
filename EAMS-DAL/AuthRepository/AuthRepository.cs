﻿using EAMS_ACore;
using EAMS_ACore.AuthModels;
using EAMS_ACore.HelperModels;
using EAMS_ACore.IAuthRepository;
using EAMS_ACore.Models;
using EAMS_ACore.Models.BLOModels;
using EAMS_DAL.DBContext;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Security.Claims;

namespace EAMS_DAL.AuthRepository
{
    public class AuthRepository : IAuthRepository
    {
        private readonly UserManager<UserRegistration> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;
        private readonly EamsContext _context;



        public AuthRepository(UserManager<UserRegistration> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration, EamsContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _context = context;

        }
        #region AddDynamicRole && Get Role
        public async Task<ServiceResponse> AddDynamicRole(Role role)
        {
            var existingRole = await _roleManager.FindByNameAsync(role.RoleName);
            if (existingRole != null)
            {
                return new ServiceResponse()
                {
                    IsSucceed = false,
                    Message = "Role already exists!"
                };
            }

            // Create a new role
            var newRole = new IdentityRole(role.RoleName);

            // Add the role to the database
            var result = await _roleManager.CreateAsync(newRole);

            if (!result.Succeeded)
            {
                return new ServiceResponse()
                {
                    IsSucceed = false,
                    Message = "Role creation failed! Please check role details and try again."
                };
            }
            else
            {
                return new ServiceResponse()
                {
                    IsSucceed = true,
                    Message = "Role created successfully!"
                };

            }
        }

        public async Task<List<Role>> GetRoles()
        {
            // Define the roles you want to exclude
            var excludedRoles = new List<string> { "SO", "PC", "ECI" };

            // Retrieve roles excluding the specified ones
            var identityRoles = await _roleManager.Roles
                .Where(d => !excludedRoles.Contains(d.Name))
                .ToListAsync();

            // Map the identity roles to your Role model
            var roles = identityRoles.Select(identityRole => new Role
            {
                RoleId = identityRole.Id,
                RoleName = identityRole.Name
            }).ToList();

            return roles;
        }

        #endregion

        public async Task<List<UserRegistration>> GetUsersByRoleId(string roleId)
        {
            var identityRoles = await _roleManager.FindByIdAsync(roleId);
            var userInRTole = await _userManager.GetUsersInRoleAsync(identityRoles.Name);

            //GetDashboardProfile(userInRTole.)


            return userInRTole.ToList();
        }

        #region LoginAsync && GenerateToken
        public async Task<ServiceResponse> LoginAsync(Login login)
        {
            var user = await _userManager.FindByNameAsync(login.UserName);

            if (user is null)
                return new ServiceResponse()
                {
                    IsSucceed = false,
                    Message = "Invalid Credentials"
                };

            var isPasswordCorrect = await _userManager.CheckPasswordAsync(user, login.Password);

            if (!isPasswordCorrect)
                return new ServiceResponse()
                {
                    IsSucceed = false,
                    Message = "Invalid Credentials"
                };

            var userRoles = await _userManager.GetRolesAsync(user);

            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim("JWTID", Guid.NewGuid().ToString())
            };

            foreach (var userRole in userRoles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, userRole));
            }

            //  var token = GenerateNewJsonWebToken(authClaims);

            return new ServiceResponse()
            {
                IsSucceed = true,
                // Message = token
            };
        }
        #endregion

        #region RegisterAsync

        public async Task<ServiceResponse> RegisterAsync(UserRegistration userRegistration)
        {
            throw new NotImplementedException();
        }





        #endregion

        #region FindUserByName
        public async Task<ServiceResponse> FindUserByName(UserRegistration userRegistration)
        {
            var userExists = await _userManager.FindByNameAsync(userRegistration.UserName);
            if (userExists != null)
            {
                return new ServiceResponse()
                {
                    IsSucceed = false,
                    Message = "User Already Exist"
                };
            }
            else
            {
                return new ServiceResponse()
                {
                    IsSucceed = true,
                    Message = "User Not Exist"
                };

            }
        }
        #endregion


        public async Task<List<UserRegistration>> FindUserListByName(string userName)
        {
            var users = await _userManager.Users
                .Where(u => EF.Functions.Like(u.UserName.ToUpper(), "%" + userName.ToUpper() + "%"))
                .ToListAsync();

            return users;
        }


        #region Check User Login
        public async Task<UserRegistration> CheckUserLogin(Login login)
        {
            var user = await _userManager.FindByNameAsync(login.UserName);

            // Use PasswordHasher to verify the password
            var passwordVerificationResult = await _userManager.CheckPasswordAsync(user, login.Password);

            if (passwordVerificationResult == true)
            {
                // Password is correct 
                return user;
            }
            else
            {
                // Password is incorrect
                return null;
            }
        }

        public async Task<UserRegistration> GetUserById(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
                return user;
            else
                return null;
        }


        #endregion

        #region GetRoleByUser
        public async Task<List<Role>> GetRoleByUser(UserRegistration user)
        {
            var userExist = await _userManager.FindByNameAsync(user.UserName);

            if (userExist == null)
            {
                // Handle the case where the user is not found
                return null;
            }

            var roles = await _userManager.GetRolesAsync(userExist);

            var rolesList = roles.Select(role => new Role
            {
                RoleId = role,
                RoleName = role
            }).ToList();

            return rolesList;
        }

        #endregion

        #region CreateUser
        public async Task<ServiceResponse> CreateUser(UserRegistration userRegistration, List<string> roleIds)
        {
            try
            {
                var isExist = await _roleManager.Roles.Where(r => roleIds.Contains(r.Id)).ToListAsync();
                if (isExist.Count > 0)
                {


                    var createUserResult = await _userManager.CreateAsync(userRegistration, userRegistration.PasswordHash);
                    if (!createUserResult.Succeeded)
                    {
                        return new ServiceResponse()
                        {
                            IsSucceed = false,
                            Message = "User creation failed! Please check user details and try again.",
                            // Log the error details for investigation
                        };
                    }
                }
                else
                {
                    return new ServiceResponse()
                    {
                        IsSucceed = false,
                        Message = $"Failed to assign roles to user '{userRegistration.UserName}'.",
                        // Log the error details for investigation
                    };

                }
                var user = await _userManager.FindByNameAsync(userRegistration.UserName);


                if (roleIds != null && roleIds.Any())
                {
                    var roles = await _roleManager.Roles.Where(r => roleIds.Contains(r.Id)).ToListAsync();

                    foreach (var role in roles)
                    {
                        var userRoleResult = await _userManager.AddToRoleAsync(user, role.Name);

                        if (!userRoleResult.Succeeded)
                        {
                            // Handle role assignment failure
                            return new ServiceResponse()
                            {
                                IsSucceed = false,
                                Message = $"Failed to assign roles to user '{userRegistration.UserName}'.",
                                // Log the error details for investigation
                            };
                        }
                    }

                }


                return new ServiceResponse()
                {
                    IsSucceed = true,
                    Message = $"User '{userRegistration.UserName}' created successfully!."
                };
            }
            catch (DbUpdateException ex)
            {
                

                return new ServiceResponse()
                {
                    IsSucceed = false,
                    Message = ex.Message,
                };
            }
        }

        #endregion

        #region  UpdateUser
        public async Task<ServiceResponse> UpdateUser(UserRegistration userRegistration)
        {
            try
            {

                var updateUser = await _userManager.UpdateAsync(userRegistration);
                if (updateUser.Succeeded is true)
                {
                    return new ServiceResponse()
                    {
                        IsSucceed = true,
                        Message = "User Updated Succesfully"
                    };
                }
                else
                {
                    return new ServiceResponse()
                    {
                        IsSucceed = false,
                        Message = "User Updation Failed!!w"
                    };

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }


        }
        #endregion

        #region  DeleteUser
        // public async Task<ServiceResponse> DeleteUser(UserRegistration userRegistration)
        public async Task<ServiceResponse> DeleteUser(string userId)
        {
            var executionStrategy = _context.Database.CreateExecutionStrategy();
            // Find the user record
            var userRecord = await _userManager.FindByIdAsync(userId);
            return await executionStrategy.ExecuteAsync(async () =>
            {
                using var transaction = await _context.Database.BeginTransactionAsync();
                try
                {
                    // Find the user record
                    var userRecord = await _userManager.FindByIdAsync(userId);
                    if (userRecord == null)
                    {
                        return new ServiceResponse
                        {
                            IsSucceed = false,
                            Message = "User Record Not Found!"
                        };
                    }


                    // Delete the user
                    var result = await _userManager.DeleteAsync(userRecord);
                    if (!result.Succeeded)
                    {
                        var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                        return new ServiceResponse
                        {
                            IsSucceed = false,
                            Message = $"Error Deleting Record: {errors}"
                        };
                    }

                    // Commit transaction
                    await transaction.CommitAsync();
                    return new ServiceResponse
                    {
                        IsSucceed = true,
                        Message = "User deleted successfully."
                    };
                }
                catch (Exception ex)
                {
                    // Rollback transaction on error
                    await transaction.RollbackAsync();
                    return new ServiceResponse
                    {
                        IsSucceed = false,
                        Message = $"Error: {ex.Message}"
                    };
                }
            });


        }

        #endregion



        #region ValidateMobile && Sector Officer Master && BLO Master
        public async Task<List<SectorOfficerMaster>> ValidateMobile(ValidateMobile validateMobile)
        {
            var soRecord = await _context.SectorOfficerMaster.Where(d => d.SoMobile == validateMobile.MobileNumber && d.SoStatus == true).OrderBy(d => d.ElectionTypeMasterId).ToListAsync();
            return soRecord;
        }

        public async Task<ServiceResponse> SectorOfficerMasterRecord(SectorOfficerMaster sectorOfficerMaster)
        {
            var soRecord = await _context.SectorOfficerMaster
                .FirstOrDefaultAsync(d => d.SoMobile == sectorOfficerMaster.SoMobile &&
                                          d.ElectionTypeMasterId == sectorOfficerMaster.ElectionTypeMasterId);

            if (soRecord != null && !soRecord.IsLocked)
            {
                soRecord.OTP = sectorOfficerMaster.OTP;
                soRecord.OTPGeneratedTime = DateTime.SpecifyKind(sectorOfficerMaster.OTPGeneratedTime ?? DateTime.UtcNow, DateTimeKind.Utc);
                soRecord.OTPExpireTime = DateTime.SpecifyKind(sectorOfficerMaster.OTPExpireTime ?? DateTime.UtcNow, DateTimeKind.Utc);
                soRecord.OTPAttempts = sectorOfficerMaster.OTPAttempts;
                soRecord.RefreshToken = sectorOfficerMaster.RefreshToken;
                soRecord.RefreshTokenExpiryTime = sectorOfficerMaster.RefreshTokenExpiryTime;

                _context.Update(soRecord);
                await _context.SaveChangesAsync();

                return new ServiceResponse { IsSucceed = true };
            }

            return new ServiceResponse { IsSucceed = false };
        }
        #endregion

        #region CreateSO Pin
        public async Task<ServiceResponse> CreateSOPin(CreateSOPin createSOPin, string soId)
        {
            var soRecord = _context.SectorOfficerMaster.Where(d => d.SOMasterId == Convert.ToInt32(soId)).FirstOrDefault();
            if (soRecord == null)
            {
                return new ServiceResponse()
                {
                    IsSucceed = false,
                    Message = "SO User Not Exist"
                };
            }
            else
            {
                soRecord.AppPin = createSOPin.ConfirmSOPin;
                _context.SectorOfficerMaster.Update(soRecord);
                _context.SaveChanges();
                return new ServiceResponse()
                {
                    IsSucceed = true,
                    Message = "PIN Created SuccessFully"
                };

            }
        }

        #endregion


        #region BLO

        public async Task<BLOMaster> GetBLOById(int bloId)
        {
            var bloRecord = _context.BLOMaster.Where(d => d.BLOMasterId == bloId).FirstOrDefault();
            if (bloRecord is not null)
            {
                return bloRecord;
            }
            else
            {
                return null;
            }
        }
        public async Task<BLOMaster> GetBLO(ValidateMobile validateMobile)
        {
            var bloRecord = await _context.BLOMaster.FirstOrDefaultAsync(d => d.BLOMobile == validateMobile.MobileNumber && d.BLOStatus == true);
            return bloRecord;
        }
        public async Task<ServiceResponse> AddUpdateBLOMaster(BLOMaster bLOMaster)
        {
            var bloRecord = await _context.BLOMaster
                .FirstOrDefaultAsync(d => d.BLOMobile == bLOMaster.BLOMobile);

            if (bloRecord != null && !bloRecord.IsLocked)
            {
                bloRecord.OTP = bLOMaster.OTP;
                bloRecord.OTPGeneratedTime = DateTime.SpecifyKind(bLOMaster.OTPGeneratedTime ?? DateTime.UtcNow, DateTimeKind.Utc);
                bloRecord.OTPExpireTime = DateTime.SpecifyKind(bLOMaster.OTPExpireTime ?? DateTime.UtcNow, DateTimeKind.Utc);
                bloRecord.OTPAttempts = bLOMaster.OTPAttempts;
                bloRecord.RefreshToken = bLOMaster.RefreshToken;
                bloRecord.RefreshTokenExpiryTime = bLOMaster.RefreshTokenExpiryTime;

                _context.Update(bloRecord);
                await _context.SaveChangesAsync();

                return new ServiceResponse { IsSucceed = true };
            }

            return new ServiceResponse { IsSucceed = false };
        }
        #endregion

        #region GetSOByID
        public async Task<SectorOfficerMaster> GetSOById(int soId)
        {
            var soRecord = _context.SectorOfficerMaster.Where(d => d.SOMasterId == soId).FirstOrDefault();
            if (soRecord is not null)
            {
                return soRecord;
            }
            else
            {
                return null;
            }
        }
        #endregion

        #region GetDashboardProfile && UpdateDashboardProfile

        public async Task<DashBoardProfile> GetDashboardProfile(string userId, int? stateMasterId)
        {
            var userRecord = await _userManager.FindByIdAsync(userId);

            if (userRecord == null)
            {
                return null; // Return null when userRecord is null
            }

            var roles = await _userManager.GetRolesAsync(userRecord);
            var rolesList = roles.ToList();
            var state = _context.StateMaster.Include(d => d.DistrictMasters)
      .Include(d => d.DistrictMasters.Select(dm => dm.AssemblyMaster))
      .Include(d => d.DistrictMasters.Select(dm => dm.AssemblyMaster.Select(am => am.FourthLevelH)))
      .FirstOrDefault(d => d.StateMasterId == userRecord.StateMasterId &&
                           d.DistrictMasters.Any(dm => dm.DistrictMasterId == userRecord.DistrictMasterId));

            return new DashBoardProfile
            {
                Name = userRecord.UserName,
                MobileNumber = userRecord.PhoneNumber,
                UserEmail = userRecord.Email,
                UserType = "DashBoard",
                Roles = rolesList,
                StateMasterId = state?.StateMasterId,
                StateName = state?.StateName,
                DistrictMasterId = state?.DistrictMasters?.FirstOrDefault()?.DistrictMasterId, // Handle potential null state or DistrictMasters collection
                DistrictName = state?.DistrictMasters?.FirstOrDefault()?.DistrictName,
                AssemblyMasterId = state?.DistrictMasters?.FirstOrDefault()?.AssemblyMaster?.FirstOrDefault()?.AssemblyMasterId, // Nested null checks
                AssemblyName = state?.DistrictMasters?.FirstOrDefault()?.AssemblyMaster?.FirstOrDefault()?.AssemblyName,
                FourthLevelHMasterId = state?.DistrictMasters?.FirstOrDefault()?.AssemblyMaster?.FirstOrDefault()?.FourthLevelH?.FirstOrDefault()?.FourthLevelHMasterId,
                FourthLevelHName = state?.DistrictMasters?.FirstOrDefault()?.AssemblyMaster?.FirstOrDefault()?.FourthLevelH?.FirstOrDefault()?.HierarchyName
            };


        }


        public async Task<List<AssemblyMaster>> GetAssemblyByDistrictId(string stateMasterId, string districtMasterId)
        {

            var asemData = await _context.AssemblyMaster
    .Where(d => d.StateMasterId == Convert.ToInt32(stateMasterId) && d.DistrictMasterId == Convert.ToInt32(districtMasterId))
    .OrderBy(d => d.PCMasterId)
    .Select(d => new AssemblyMaster
    {
        PCMasterId = d.PCMasterId,
        StateMasterId = d.StateMasterId,
        AssemblyCode = d.AssemblyCode,
        AssemblyName = d.AssemblyName,
        AssemblyType = d.AssemblyType,
        AssemblyStatus = d.AssemblyStatus,
        AssemblyCreatedAt = d.AssemblyCreatedAt
    })
    .ToListAsync();

            return asemData;
        }
        public async Task<List<ParliamentConstituencyMaster>> GetPCList(string stateMasterId)
        {

            var pcData = await _context.ParliamentConstituencyMaster
    .Where(d => d.StateMasterId == Convert.ToInt32(stateMasterId))
    .OrderBy(d => d.PCMasterId)
    .Select(d => new ParliamentConstituencyMaster
    {
        PCMasterId = d.PCMasterId,
        StateMasterId = d.StateMasterId,
        PcCodeNo = d.PcCodeNo,
        PcName = d.PcName,
        PcType = d.PcType,
        PcStatus = d.PcStatus
    })
    .ToListAsync();

            return pcData;
        }
        public async Task<List<AssemblyMaster>> GetAssemblyByPCId(string stateMasterid, string PcMasterId)
        {

            var asemData = await _context.AssemblyMaster
    .Where(d => d.PCMasterId == Convert.ToInt32(PcMasterId) && d.StateMasterId == Convert.ToInt32(stateMasterid))
    .OrderBy(d => d.PCMasterId)
    .Select(d => new AssemblyMaster
    {
        PCMasterId = d.PCMasterId,
        StateMasterId = d.StateMasterId,
        AssemblyCode = d.AssemblyCode,
        AssemblyName = d.AssemblyName,
        AssemblyType = d.AssemblyType,
        AssemblyStatus = d.AssemblyStatus,
        AssemblyCreatedAt = d.AssemblyCreatedAt
    })
    .ToListAsync();

            return asemData;
        }
        public async Task<AssemblyMaster> GetAssemblyById(string assemblyMasterId)
        {
            var assemblyRecord = await _context.AssemblyMaster.Include(d => d.StateMaster).Include(d => d.DistrictMaster).Include(d => d.ParliamentConstituencyMaster).Where(d => d.AssemblyMasterId == Convert.ToInt32(assemblyMasterId)).FirstOrDefaultAsync();
            return assemblyRecord;
        }

        public async Task<ServiceResponse> UpdateDashboardProfile(string UserID, UpdateDashboardProfile updateDashboardProfile)
        {
            var userDetail = await _userManager.FindByIdAsync(UserID);
            if (userDetail == null)
            {
                return new ServiceResponse()
                {
                    IsSucceed = false,
                    Message = "User not found."
                };
            }

            userDetail.PhoneNumber = updateDashboardProfile.MobileNumber;

            var result = await _userManager.UpdateAsync(userDetail);

            if (result.Succeeded)
            {
                return new ServiceResponse()
                {
                    IsSucceed = true,
                    Message = "Mobile number updated successfully."
                };
            }
            else
            {
                return new ServiceResponse()
                {
                    IsSucceed = false,
                    Message = "Failed to update mobile number."
                };
            }


        }
        #endregion

        #region ForgetPassword & Reset Password
        public Task<ServiceResponse> ForgetPassword(ForgetPasswordModel forgetPasswordModel)
        {
            throw new NotImplementedException();
        }

        public Task<ServiceResponse> ResetPassword(ResetPasswordModel resetPasswordModel)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region GetUser List

        public async Task<Dictionary<string, object>> GetUserList(GetUser getUser)
        {
            IQueryable<UserRegistration> query = _userManager.Users;

            // Pagination
            int page = getUser.Page == 0 ? 1 : getUser.Page;
            int pageSize = getUser.PageSize == 0 ? 10 : getUser.PageSize;
            int skip = (page - 1) * pageSize;
            var filteredQuery = query.Skip(skip).Take(pageSize);

            var userList = await filteredQuery.Select(d => new GetUserList
            {
                UserName = d.UserName,
                Email = d.Email,
                PhoneNumber = d.PhoneNumber,
                LockoutEnabled = d.LockoutEnabled,
                UserId = d.Id

            }).ToListAsync();

            var totalCount = await query.CountAsync();
            int pageCount = (int)Math.Ceiling((double)totalCount / pageSize);

            var result = new Dictionary<string, object>
    {
        { "pageCount", pageCount },
        { "pageSize", pageSize },
        { "page", page },
        { "data", userList }
    };

            return result;
        }
        #endregion
    }
}