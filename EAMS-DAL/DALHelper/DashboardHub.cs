using EAMS_ACore.Interfaces;
using EAMS_ACore.IRealTime;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace EAMS.Hubs
{
    [Authorize]
    public class DashBoardHub : Hub
    {
        private readonly IEamsService _eamsService;
        private readonly IUserConnectionService _userConnectionService;
        private readonly ILogger<DashBoardHub> _logger;
        private readonly IRealTime _realTime;

        public DashBoardHub(IEamsService eamsService, IUserConnectionService userConnectionService,
            ILogger<DashBoardHub> logger, IRealTime realTime)
        {
            _eamsService = eamsService;
            _userConnectionService = userConnectionService;
            _logger = logger;
            _realTime = realTime;
        }

        public async Task GetDashboardCount()
        {
            var newClaimsIdentity = new ClaimsIdentity(Context.User.Identity);
            var latestRecord = await _realTime.GetDashBoardCount(newClaimsIdentity);
            await Clients.Client(Context.ConnectionId).SendAsync("GetDashboardCount", latestRecord);
        }

        public async Task GetDashboardCountByContextId()
        {
            var newClaimsIdentity = new ClaimsIdentity(Context.User.Identity);
            var latestRecord = await _realTime.GetDashBoardCount(newClaimsIdentity);
            await Clients.Client(Context.ConnectionId).SendAsync("GetDashboardCount", latestRecord);
        }

        public async Task GetPollIntreuption()
        {
            var newClaimsIdentity = new ClaimsIdentity(Context.User.Identity);
            var dataCount = await _eamsService.GetPollInterruptionDashboardCount(newClaimsIdentity);
            await Clients.Client(Context.ConnectionId).SendAsync("GetPollIntreuption", dataCount);
        }

        public async Task GetPollIntreuptionByContextId()
        {
            var newClaimsIdentity = new ClaimsIdentity(Context.User.Identity);
            var dataCount = await _eamsService.GetPollInterruptionDashboardCount(newClaimsIdentity);
            await Clients.Client(Context.ConnectionId).SendAsync("GetPollIntreuption", dataCount);
        }

        public override async Task OnConnectedAsync()
        {
            try
            {
                var newClaimsIdentity = new ClaimsIdentity(Context.User.Identity);
                var role = Context.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
                _userConnectionService.AddUser(Context.ConnectionId, newClaimsIdentity);
                var dashboardUserCount = await GetDashboardConnectedUserStateWise();
                //  var mobileUserCount = await GetMobileConnectedUserStateWise();

                _logger.LogInformation("Dashboard Client connected to DashBoardHub. ConnectionId: {ConnectionId}", Context.ConnectionId);
                await GetDashboardCount();
                await GetPollIntreuption();
                await Clients.All.SendAsync("ActiveUser", dashboardUserCount);



                await Ping();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in OnConnectedAsync");
            }
        }

        public async Task Ping()
        {
            await GetDashboardCountByContextId();
            await GetPollIntreuptionByContextId();
            await Clients.Client(Context.ConnectionId).SendAsync("ping", "HeartBeat");
        }

        //public async Task MobileActiveUserConnected()
        //{
        //    var mobileUserCount = await GetMobileConnectedUserStateWise();
        //    await Clients.All.SendAsync("MobileActiveUser", mobileUserCount);
        //}

        //public async Task MobileActiveUserDisConnected()
        //{
        //    var mobileUserCount = await GetMobileConnectedUserStateWise();
        //    await Clients.All.SendAsync("MobileActiveUser", mobileUserCount);
        //}

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            try
            {
                var getUserConnection = await _userConnectionService.GetConnectionIdByUserId(Context.ConnectionId);
                _logger.LogInformation($"User Found IN DB {getUserConnection.ConnectionId} and {getUserConnection.Role}");
                var mobileUserCount = await GetMobileConnectedUserStateWise();

                if (getUserConnection.Role != "SO" && getUserConnection.Role != "BLO")
                {
                    _logger.LogInformation("Dashboard Client Disconnected from DashBoardHub. ConnectionId: {ConnectionId}", Context.ConnectionId);
                    var dashboardUserCount = await GetDashboardConnectedUserStateWise();
                    await Clients.All.SendAsync("ActiveUser", dashboardUserCount);
                    await Clients.All.SendAsync("GetMobileActiveUser", mobileUserCount);
                }
                else
                {
                    _logger.LogInformation("SO/BLO Client Disconnected from DashBoardHub. ConnectionId: {ConnectionId}", Context.ConnectionId);
                    await Clients.All.SendAsync("GetMobileActiveUser", mobileUserCount);

                }

                var isRemoved = await _userConnectionService.RemoveUser(Context.ConnectionId);
                _logger.LogInformation($"{isRemoved}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in OnDisconnectedAsync");
            }
            finally
            {
                await base.OnDisconnectedAsync(exception);
            }
        }

        private async Task<int?> GetDashboardConnectedUserStateWise()
        {
            var stateMasterId = Context.User.Claims.FirstOrDefault(c => c.Type == "StateMasterId")?.Value;
            return stateMasterId != null
                ? await _userConnectionService.GetDashboardConnectedUserCountByStateId(Convert.ToInt32(stateMasterId))
                : null;
        }

        private async Task<int?> GetMobileConnectedUserStateWise()
        {
            var stateMasterId = Context.User.Claims.FirstOrDefault(c => c.Type == "StateMasterId")?.Value;
            return stateMasterId != null
                ? await _userConnectionService.GetMobileConnectedUserCountByStateId(Convert.ToInt32(stateMasterId))
                : null;
        }
    }
}