using EAMS_ACore.IRealTime;
using EAMS_ACore.IRepository;
using EAMS_ACore.Models;
using System.Security.Claims;

namespace EAMS_BLL.RealTimeServices
{
    public class RealTimeService : IRealTime
    {
        private readonly IEamsRepository _eamsRepository;
        public RealTimeService(IEamsRepository eamsRepository)
        {
            _eamsRepository = eamsRepository;
        }
        public async Task<DashBoardRealTimeCount> GetDashBoardCount(ClaimsIdentity claimsIdentity)
        {
            return await _eamsRepository.GetDashBoardCount(claimsIdentity);
        }
    }
}
