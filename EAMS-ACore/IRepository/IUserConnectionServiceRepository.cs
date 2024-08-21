using EAMS_ACore.SignalRModels;
using System.Security.Claims;

namespace EAMS_ACore.IRepository
{
    public interface IUserConnectionServiceRepository
    {
        void AddUser(string connectionId, ClaimsIdentity claimsIdentity);
        Task<int?> GetDashboardConnectedUserCountByStateId(int stateMasterId);
        Task<int?> GetMobileConnectedUserCountByStateId(int stateMasterId);
        Task<DashboardConnectedUser> GetConnectedUser(string connectionId);
        Task<string> RemoveUser(string connectionId);

    }
}
