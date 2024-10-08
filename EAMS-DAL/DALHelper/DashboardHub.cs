using EAMS_ACore.Interfaces;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace EAMS.Hubs
{
    public sealed class DashBoardHub : Hub
    {
        private readonly IEamsService _eamsService;
        private readonly ILogger<DashBoardHub> _logger;

        // Static counters to track connected users
        private static int _dashboardUserCount = 0;
        private static int _mobileUserCount = 0;

        public DashBoardHub(IEamsService eamsService, ILogger<DashBoardHub> logger)
        {
            _eamsService = eamsService;
            _logger = logger;
        }

        public override async Task OnConnectedAsync()
        {
            try
            {
                var role = Context.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

                if (role.Contains("FO"))
                {
                    await HandleMobileUserConnected();
                }
                else
                {
                    await HandleDashboardUserConnected();
                }

                await SendUserCounts(); 
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in OnConnectedAsync");
            }
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            try
            {
                var role = Context.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

                if (role.Contains("FO"))
                {
                    await HandleMobileUserDisconnected();
                }
                else
                {
                    await HandleDashboardUserDisconnected();
                }

                await SendUserCounts(); // Send updated user counts to all clients
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

        private async Task HandleMobileUserConnected()
        {
            _mobileUserCount++;
            await Clients.Client(Context.ConnectionId).SendAsync("UserType", "MobileUser");
        }

        private async Task HandleDashboardUserConnected()
        {
            _dashboardUserCount++;
            await Clients.Client(Context.ConnectionId).SendAsync("UserType", "DashboardUser");
        }
        private async Task HandleMobileUserDisconnected()
        {
            _mobileUserCount--;
        }

        private async Task HandleDashboardUserDisconnected()
        {
            _dashboardUserCount--;
        }

        public async Task SendUserCounts()
        {
            var counts = new
            {
                MobileUserCount = _mobileUserCount,
                DashboardUserCount = _dashboardUserCount
            };

            // Broadcast the updated counts to all connected clients
            await Clients.All.SendAsync("ReceiveUserCounts", counts);
        }
        public async Task SendDashBoardCount()
        {
            ClaimsIdentity claimsIdentity = Context.User.Identity as ClaimsIdentity;
            var rolesClaim = claimsIdentity.Claims.FirstOrDefault(c => c.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role");
            var roles = rolesClaim?.Value;

            var stateMasterIdString = claimsIdentity.Claims.FirstOrDefault(c => c.Type == "StateMasterId")?.Value;
            int stateMasterId = int.Parse(stateMasterIdString);

            var electionTypeMasterIdString = claimsIdentity.Claims.FirstOrDefault(c => c.Type == "ElectionTypeMasterId")?.Value;
            int electionTypeMasterId = int.Parse(electionTypeMasterIdString);

            var districtMasterIdString = claimsIdentity.Claims.FirstOrDefault(c => c.Type == "DistrictMasterId")?.Value;
            int? districtMasterId = !string.IsNullOrEmpty(districtMasterIdString) ? int.Parse(districtMasterIdString) : (int?)null;

            var assemblyMasterIdString = claimsIdentity.Claims.FirstOrDefault(c => c.Type == "AssemblyMasterId")?.Value;
            int? assemblyMasterId = !string.IsNullOrEmpty(assemblyMasterIdString) ? int.Parse(assemblyMasterIdString) : (int?)null;

            var fourthLevelHMasterIdString = claimsIdentity.Claims.FirstOrDefault(c => c.Type == "FourthLevelHMasterId")?.Value;
            int? fourthLevelHMasterId = !string.IsNullOrEmpty(fourthLevelHMasterIdString) ? int.Parse(fourthLevelHMasterIdString) : (int?)null;
            var eventDashboardCount = await _eamsService.GetEventActivityDashBoardCount(roles, electionTypeMasterId, stateMasterId, districtMasterId, assemblyMasterId, fourthLevelHMasterId);
            
            await Clients.Client(Context.ConnectionId).SendAsync("GetDashBoardCount", eventDashboardCount);
        }
        public async Task SendDashBoardPollInterruptionCount()
        {
            ClaimsIdentity claimsIdentity = Context.User.Identity as ClaimsIdentity;
            var rolesClaim = claimsIdentity.Claims.FirstOrDefault(c => c.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role");
            var roles = rolesClaim?.Value;

            var stateMasterIdString = claimsIdentity.Claims.FirstOrDefault(c => c.Type == "StateMasterId")?.Value;
            int stateMasterId = int.Parse(stateMasterIdString);

            var electionTypeMasterIdString = claimsIdentity.Claims.FirstOrDefault(c => c.Type == "ElectionTypeMasterId")?.Value;
            int electionTypeMasterId = int.Parse(electionTypeMasterIdString);

            var districtMasterIdString = claimsIdentity.Claims.FirstOrDefault(c => c.Type == "DistrictMasterId")?.Value;
            int? districtMasterId = !string.IsNullOrEmpty(districtMasterIdString) ? int.Parse(districtMasterIdString) : (int?)null;

            var assemblyMasterIdString = claimsIdentity.Claims.FirstOrDefault(c => c.Type == "AssemblyMasterId")?.Value;
            int? assemblyMasterId = !string.IsNullOrEmpty(assemblyMasterIdString) ? int.Parse(assemblyMasterIdString) : (int?)null;

            var fourthLevelHMasterIdString = claimsIdentity.Claims.FirstOrDefault(c => c.Type == "FourthLevelHMasterId")?.Value;
            int? fourthLevelHMasterId = !string.IsNullOrEmpty(fourthLevelHMasterIdString) ? int.Parse(fourthLevelHMasterIdString) : (int?)null;
            var eventDashboardCount = await _eamsService.GetPollInterruptionDashboardCount(roles, electionTypeMasterId, stateMasterId, districtMasterId, assemblyMasterId, fourthLevelHMasterId);

            await Clients.Client(Context.ConnectionId).SendAsync("GetPollInterruptionCount", eventDashboardCount);
        }
        public async Task Ping()
        {
            await SendDashBoardCount();
            await SendDashBoardPollInterruptionCount();
            await Clients.Client(Context.ConnectionId).SendAsync("Ping", "HeartBeat");
        }
    }
}
