using EAMS.Helper;
using EAMS_ACore.AuthModels;
using EAMS_ACore.HelperModels;
using EAMS_ACore.Models;

namespace EAMS_ACore.AuthInterfaces
{
    public interface IAuthService
    {
        Task<AuthServiceResponse> RegisterAsync(UserRegistration userRegistration, List<string> roleIds);
        Task<ServiceResponse> SwitchDashboardUser(string userId, int electionTypeMasterId);
        Task<Token> LoginAsync(Login login);
        

        Task<ServiceResponse> DeleteUser(string userId);
        Task<ServiceResponse> AddDynamicRole(Role role);
        Task<List<Role>> GetRoles();
        Task<List<UserRegistration>> GetUsersByRoleId(string roleId);
        Task<Response> ValidateMobile(ValidateMobile validateMobile);
        Task<Token> GetRefreshToken(GetRefreshToken getRefreshToken);
        Task<ServiceResponse> CreateSOPin(CreateSOPin createSOPin, string soID);
        Task<DashBoardProfile> GetDashboardProfile(string userId, int? stateMasterId);
        Task<ServiceResponse> UpdateDashboardProfile(string userId, UpdateDashboardProfile updateDashboardProfile);

        Task<ServiceResponse> ForgetPassword(ForgetPasswordModel forgetPasswordModel);
        Task<ServiceResponse> ResetPassword(ResetPasswordModel resetPasswordModel);
        Task<Dictionary<string, object>> GetUserList(GetUser getUser);
        Task<bool> UpdateLockoutUser(UpdateLockoutUser updateLockoutUser);
        Task<int> UpdateLockoutUserInBulk(UpdateLockoutUserInBulk updateLockoutUserInBulk);

        Task<ServiceResponse> UpdateUserDetail(string userId, string mobileNumber, string? otp);
        Task<ServiceResponse> UpdateFieldOfficerDetail(string mobileNumber, int? otp);

        Task<List<ROUserList>> GetROUserListByAssemblyId(int stateMasterId, int districtMasterId, int assemblyMasterId);
    }
}
