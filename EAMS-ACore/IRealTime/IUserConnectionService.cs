using EAMS_ACore.SignalRModels;
using System.Security.Claims;

namespace EAMS_ACore.IRealTime
{
    public interface IUserConnectionService
    {
        ClaimsIdentity GetUserClaimsIdentity(string connectionId);
        void AddUser(string connectionId, ClaimsIdentity claimsIdentity);
        Task<string> RemoveUser(string connectionId);
        Task<DashboardConnectedUser> GetConnectionIdByUserId(string userId);
        Task<int?> GetDashboardConnectedUserCountByStateId(int stateMasterId);
        Task<int?> GetMobileConnectedUserCountByStateId(int stateMasterId);
        List<UserConnection> GetConnections();

    }
    public class UserConnection
    {
        public string ConnectionId { get; set; }
        public ClaimsIdentity ClaimsIdentity { get; set; }
    }
}
