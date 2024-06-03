using EAMS_ACore.HelperModels;
using EAMS_ACore.Models;
using System.Security.Claims;

namespace EAMS_ACore.IRealTime
{
    public interface IRealTime
    {
        Task<DashBoardRealTimeCount> GetDashBoardCount(ClaimsIdentity claimsIdentity);
    }
}
