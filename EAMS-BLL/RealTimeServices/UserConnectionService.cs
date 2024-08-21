using EAMS_ACore.IRealTime;
using EAMS_ACore.IRepository;
using EAMS_ACore.SignalRModels;
using System.Security.Claims;

namespace EAMS_BLL.RealTimeServices
{
    public class UserConnectionService : IUserConnectionService
    {
        private static List<UserConnection> userConnections = new List<UserConnection>();
        private readonly IUserConnectionServiceRepository _userConnectionServiceRepository;
        public UserConnectionService(IUserConnectionServiceRepository userConnectionServiceRepository)
        {
            _userConnectionServiceRepository = userConnectionServiceRepository;
        }

        public ClaimsIdentity GetUserClaimsIdentity(string connectionId)
        {
            var userConnection = userConnections.FirstOrDefault(u => u.ConnectionId == connectionId);
            return userConnection?.ClaimsIdentity;
        }

        public async void AddUser(string connectionId, ClaimsIdentity claimsIdentity)
        {
            _userConnectionServiceRepository.AddUser(connectionId, claimsIdentity);
        }


        public async Task<string> RemoveUser(string connectionId)
        {
            return await _userConnectionServiceRepository?.RemoveUser(connectionId);
        }

        public List<UserConnection> GetConnections()
        {
            return userConnections.ToList();
        }
        public async Task<int?> GetDashboardConnectedUserCountByStateId(int stateMasterId)
        {
            return await _userConnectionServiceRepository.GetDashboardConnectedUserCountByStateId(stateMasterId);
        }
        public async Task<int?> GetMobileConnectedUserCountByStateId(int stateMasterId)
        {
            return await _userConnectionServiceRepository.GetMobileConnectedUserCountByStateId(stateMasterId);
        }
        public Task<DashboardConnectedUser> GetConnectionIdByUserId(string userId)
        {

            return _userConnectionServiceRepository.GetConnectedUser(userId);
        }
    }


}
