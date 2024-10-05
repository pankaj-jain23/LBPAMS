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
                    _logger.LogInformation("Mobile user connected");
                    _mobileUserCount++; // Increment mobile user count

                    // Notify the connected client that they are a Mobile user
                    await Clients.Client(Context.ConnectionId).SendAsync("UserType", "MobileUser");
                }
                else
                {
                    _logger.LogInformation("Dashboard user connected");
                    _dashboardUserCount++; // Increment dashboard user count

                    // Notify the connected client that they are a Dashboard user
                    await Clients.Client(Context.ConnectionId).SendAsync("UserType", "DashboardUser");
                }

                // Send updated counts to all clients
                await SendUserCounts();
                await Ping(); // Ping method to send a heartbeat message
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in OnConnectedAsync");
            }
        }

        public async Task Ping()
        {
            await Clients.Client(Context.ConnectionId).SendAsync("Ping", "HeartBeat");
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            try
            {
                var role = Context.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

                if (role.Contains("FO"))
                {
                    _logger.LogInformation("Mobile user disconnected");
                    _mobileUserCount--; // Decrement mobile user count
                }
                else
                {
                    _logger.LogInformation("Dashboard user disconnected");
                    _dashboardUserCount--; // Decrement dashboard user count
                }

                // Send updated counts to all clients
                await SendUserCounts();
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

        // Method to send the current counts to all connected clients
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
    }
}
