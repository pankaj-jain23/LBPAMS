using EAMS_ACore.SignalRModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

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
