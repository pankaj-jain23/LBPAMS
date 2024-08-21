using EAMS_ACore.AuthModels;
using EAMS_ACore.HelperModels;
using EAMS_ACore.Models;
using EAMS_ACore.Models.BLOModels;

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
        Task<List<SectorOfficerMaster>> ValidateMobile(ValidateMobile validateMobile);
        Task<BLOMaster> GetBLO(ValidateMobile validateMobile);
        Task<ServiceResponse> AddUpdateBLOMaster(BLOMaster bloMaster);
        Task<BLOMaster> GetBLOById(int bloId);
        Task<ServiceResponse> SectorOfficerMasterRecord(SectorOfficerMaster sectorOfficerMaster);
        Task<ServiceResponse> FindUserByName(UserRegistration userRegistration);
        Task<List<UserRegistration>> FindUserListByName(string userName);
        Task<UserRegistration> CheckUserLogin(Login login);
        Task<UserRegistration> GetUserById(string userId);
        Task<ServiceResponse> CreateUser(UserRegistration userRegistration, List<string> roleIds);
        Task<ServiceResponse> UpdateUser(UserRegistration userRegistration);
        Task<List<Role>> GetRoleByUser(UserRegistration user);
        Task<ServiceResponse> CreateSOPin(CreateSOPin createSOPin, string soId);
        Task<SectorOfficerMaster> GetSOById(int soId);
        Task<DashBoardProfile> GetDashboardProfile(string userId, int? stateMasterId);
        Task<List<UserState>> GetUserMaster(string userId);
        Task<ServiceResponse> UpdateDashboardProfile(string userId, UpdateDashboardProfile updateDashboardProfile);
        Task<List<AssemblyMaster>> GetAssemblyByDistrictId(string stateMasterid, string districtMasterId);
        Task<List<ParliamentConstituencyMaster>> GetPCList(string stateMasterId);

        Task<List<AssemblyMaster>> GetAssemblyByPCId(string stateMasterid, string PcMasterId);
        Task<AssemblyMaster> GetAssemblyById(string assemblyId);
        Task<ServiceResponse> ForgetPassword(ForgetPasswordModel forgetPasswordModel);
        Task<ServiceResponse> ResetPassword(ResetPasswordModel resetPasswordModel);
        Task<Dictionary<string, object>> GetUserList(GetUser getUser);


    }
}
