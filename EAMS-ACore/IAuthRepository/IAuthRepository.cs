﻿using EAMS_ACore.AuthModels;
using EAMS_ACore.HelperModels;
using EAMS_ACore.Models;
using EAMS_ACore.Models.BLOModels;
using EAMS_ACore.Models.ElectionType;

namespace EAMS_ACore.IAuthRepository
{
    public interface IAuthRepository
    {
        Task<ServiceResponse> RegisterAsync(UserRegistration userRegistration);
        Task<ServiceResponse> LoginAsync(Login login);
        Task<ServiceResponse> DeleteUser(string userId);

        Task<ServiceResponse> AddDynamicRole(Role role);
        Task<List<Role>> GetRoles();
        Task<List<UserRegistration>> GetUsersByRoleId(string roleId);
        Task<FieldOfficerMaster> ValidateMobile(ValidateMobile validateMobile);
        Task<AROResultMaster> ValidateMobileForARO(ValidateMobile validateMobile);
        Task<BLOMaster> GetBLO(ValidateMobile validateMobile);
        Task<ServiceResponse> AddUpdateBLOMaster(BLOMaster bloMaster);
        Task<BLOMaster> GetBLOById(int bloId);
        Task<ServiceResponse> SectorOfficerMasterRecord(FieldOfficerMaster sectorOfficerMaster);
        Task<AuthServiceResponse> FindUserByName(UserRegistration userRegistration);
        Task<UserRegistration> FindUserByName(string userName);
        Task<List<UserRegistration>> FindUserListByName(string userName);
        Task<UserRegistration> CheckUserLogin(Login login);
        Task<UserRegistration> GetUserById(string userId);
        Task<AuthServiceResponse> CreateUser(UserRegistration userRegistration, List<string> roleIds);
        Task<ServiceResponse> SwitchDashboardUser(string userId, int electionTypeMasterId);

        Task<ServiceResponse> UpdateUser(UserRegistration userRegistration);
        Task<List<Role>> GetRoleByUser(UserRegistration user);
     
        Task<ServiceResponse> CreateSOPin(CreateSOPin createSOPin, string soId);
        Task<FieldOfficerMaster> GetFOById(int foId);
        Task<AROResultMaster> GetAROById(int roId);
        Task<DashBoardProfile> GetDashboardProfile(string userId, int? stateMasterId);
   
        Task<ServiceResponse> UpdateDashboardProfile(string userId, UpdateDashboardProfile updateDashboardProfile);
        Task<List<AssemblyMaster>> GetAssemblyByDistrictId(string stateMasterid, string districtMasterId);
        Task<List<ParliamentConstituencyMaster>> GetPCList(string stateMasterId);

        Task<List<AssemblyMaster>> GetAssemblyByPCId(string stateMasterid, string PcMasterId);
        Task<AssemblyMaster> GetAssemblyById(string assemblyId);
        Task<ServiceResponse> ForgetPassword(ForgetPasswordModel forgetPasswordModel);
        Task<ServiceResponse> ResetPassword(ResetPasswordModel resetPasswordModel);
        Task<Dictionary<string, object>> GetUserList(GetUser getUser);
        Task<bool> UpdateLockoutUser(UpdateLockoutUser updateLockoutUser);
        Task<int> UpdateLockoutUserInBulk(UpdateLockoutUserInBulk updateLockoutUserInBulk);
        Task<ElectionTypeMaster> GetElectionTypeById(int? electionTypeMasterId);
        Task<ServiceResponse> LoginWithTwoFactorCheckAsync(Login login);

        #region UpdateUserDetail
        Task<bool> UpdateUserDetail(string userId, string mobileNumber);
        #endregion
    }
}
