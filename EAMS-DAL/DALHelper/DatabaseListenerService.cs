using EAMS.Hubs;
using EAMS_ACore.Interfaces;
using EAMS_ACore.IRepository;
using EAMS_ACore.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Npgsql;
using System.Security.Claims;

public class DatabaseListenerService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IConfiguration _configuration;
    private readonly IHubContext<DashBoardHub> _hubContext;
    string dahboardTrigger = "";
    string pollTrigger = "";
    public DatabaseListenerService(IHubContext<DashBoardHub> hubContext,
                                   IServiceScopeFactory scopeFactory,
                                   IConfiguration configuration)
    {
        _hubContext = hubContext;
        _scopeFactory = scopeFactory;
        _configuration = configuration;
    }


    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        DahboardMastersId dahboardMastersIds = new DahboardMastersId();
        PollInterruptionMastersId pollInterruptionMastersId = new PollInterruptionMastersId();
        using var conn = new NpgsqlConnection(_configuration.GetConnectionString("Postgres"));
        await conn.OpenAsync();
        using var cmd1 = new NpgsqlCommand("LISTEN dashboard_change", conn);
        using var cmd2 = new NpgsqlCommand("LISTEN pollinterruption_change", conn);
        await cmd1.ExecuteNonQueryAsync();
        await cmd2.ExecuteNonQueryAsync();

        while (!stoppingToken.IsCancellationRequested)
        {
            // Wait for a notification or cancellation token
            var notification = conn.WaitAsync(stoppingToken);

            conn.Notification += async (sender, args) =>
            {
                if (args.Channel == "dashboard_change")
                {
                    dahboardTrigger = args.Channel;
                    var affectedRecord = await GetAffectedDashboardRecord(args.Payload);
                    dahboardMastersIds = affectedRecord;

                }
                else if (args.Channel == "pollinterruption_change")
                {
                    pollTrigger = args.Channel;
                    var affectedRecord = await GetAffectedPollInterutptionRecord(args.Payload);
                    pollInterruptionMastersId = affectedRecord;
                }
            };

            using (var scope = _scopeFactory.CreateScope())
            {
                var scopedService = scope.ServiceProvider.GetRequiredService<IEamsService>();
                if (dahboardTrigger == "dashboard_change" || pollTrigger == "pollinterruption_change")
                {
                    try
                    {

                        // Fetch the latest record
                        var userConnectionServiceRepository = scope.ServiceProvider.GetRequiredService<IUserConnectionServiceRepository>();
                        string aroType = "ARO";
                        string districtPCType = "DistrictPC";
                        string stateAdminType = "StateAdmin";
                        var aroRecords = await scopedService.DashboardConnectedUser(dahboardMastersIds, aroType);
                        var districtPCWiseRecords = await scopedService.DashboardConnectedUser(dahboardMastersIds, districtPCType);
                        var stateRecords = await scopedService.DashboardConnectedUser(dahboardMastersIds, stateAdminType);
                     
                        foreach (var aro in aroRecords)
                        {

                            var newClaims = new List<Claim>
                        {
                                     new Claim("StateMasterId",aro.StateMasterId.ToString()),
                                    new Claim("DistrictMasterId",aro.DistrictMasterId.ToString()),
                                    new Claim("PCMasterId",aro.DistrictMasterId.ToString()),
                                    new Claim("AssemblyMasterId",aro.AssemblyMasterId.ToString()),
                                    new Claim("BoothMasterId",aro.AssemblyMasterId.ToString()),
                                    new Claim(ClaimTypes.Role,aro.Role)
                                };
                            var newClaimsIdentity = new ClaimsIdentity(newClaims, "CustomAuthenticationType");

                            if (newClaimsIdentity is not null)
                            {
                                if (dahboardTrigger == "dashboard_change")
                                {
                                    var latestRecord = await scopedService.GetDashBoardCount(newClaimsIdentity);
                                    if (latestRecord is not null)
                                    {
                                        await NotifyClientsIfChangedDashBoard(latestRecord, aro.ConnectionId);
                                        dahboardTrigger = "";
                                    }

                                }
                                else if (pollTrigger == "pollinterruption_change")
                                {

                                    var dataRecord = await scopedService.GetPollInterruptionDashboardCount(newClaimsIdentity);
                                     
                                        await NotifyClientsIfChangedPollInterruption(dataRecord, aro.ConnectionId);
                                        pollTrigger = "";
                                    

                                }


                            }
                        }
                        foreach (var district in districtPCWiseRecords)
                        {

                            var newClaims = new List<Claim>
                        {
                                     new Claim("StateMasterId",district.StateMasterId.ToString()),
                                    new Claim("DistrictMasterId",district.DistrictMasterId.ToString()),
                                    new Claim("PCMasterId",district.DistrictMasterId.ToString()),
                                    new Claim("AssemblyMasterId",district.AssemblyMasterId.ToString()),
                                    new Claim("BoothMasterId",district.AssemblyMasterId.ToString()),
                                    new Claim(ClaimTypes.Role,district.Role)
                                };
                            var newClaimsIdentity = new ClaimsIdentity(newClaims, "CustomAuthenticationType");

                            if (newClaimsIdentity is not null)
                            {
                                if (dahboardTrigger == "dashboard_change")
                                {
                                    var latestRecord = await scopedService.GetDashBoardCount(newClaimsIdentity);
                                    if (latestRecord is not null)
                                    {
                                        await NotifyClientsIfChangedDashBoard(latestRecord, district.ConnectionId);
                                        dahboardTrigger = "";
                                    }

                                }
                                else if (pollTrigger == "pollinterruption_change")
                                {

                                    var dataRecord = await scopedService.GetPollInterruptionDashboardCount(newClaimsIdentity);
                                   
                                        await NotifyClientsIfChangedPollInterruption(dataRecord, district.ConnectionId);
                                        pollTrigger = "";
                                     

                                }


                            }
                        }
                        foreach (var state in stateRecords)
                        {

                            var newClaims = new List<Claim>
                        {
                                     new Claim("StateMasterId",state.StateMasterId.ToString()),
                                    new Claim("DistrictMasterId",state.DistrictMasterId.ToString()),
                                    new Claim("PCMasterId",state.DistrictMasterId.ToString()),
                                    new Claim("AssemblyMasterId",state.AssemblyMasterId.ToString()),
                                    new Claim("BoothMasterId",state.AssemblyMasterId.ToString()),
                                    new Claim(ClaimTypes.Role,state.Role)
                                };
                            var newClaimsIdentity = new ClaimsIdentity(newClaims, "CustomAuthenticationType");

                            if (newClaimsIdentity is not null)
                            {
                                if (dahboardTrigger == "dashboard_change")
                                {
                                    var latestRecord = await scopedService.GetDashBoardCount(newClaimsIdentity);
                                    if (latestRecord is not null)
                                    {
                                        await NotifyClientsIfChangedDashBoard(latestRecord, state.ConnectionId);
                                        dahboardTrigger = "";
                                    }

                                }
                                else if (pollTrigger == "pollinterruption_change")
                                {

                                    var dataRecord = await scopedService.GetPollInterruptionDashboardCount(newClaimsIdentity);
                                    
                                        await NotifyClientsIfChangedPollInterruption(dataRecord, state.ConnectionId);
                                        pollTrigger = "";
                                     

                                }


                            }
                        }

                    }
                    catch (Exception ex)
                    {
                        // Handle exceptions
                        Console.WriteLine($"Error processing database change: {ex.Message}");
                    }

                }
            }

            // Introduce a delay to avoid a tight loop
            await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);

            // Start waiting for the next notification
            await notification;
        }
    }

    private async Task NotifyClientsIfChangedDashBoard(DashBoardRealTimeCount latestRecord, string connectionId)
    {
        try
        {
            // You may want to handle exceptions and errors here
            await _hubContext.Clients.Client(connectionId).SendAsync("GetDashboardCount", latestRecord);
        }
        catch (Exception ex)
        {
            // Handle exceptions
            Console.WriteLine($"Error notifying clients: {ex.Message}");
        }
    }
    private async Task NotifyClientsIfChangedPollInterruption(object data, string connectionId)
    {
        try
        {

            await _hubContext.Clients.Client(connectionId).SendAsync("GetPollIntreuption", data);
        }
        catch (Exception ex)
        {
            // Handle exceptions
            Console.WriteLine($"Error notifying clients: {ex.Message}");
        }
    }
    private async Task<DahboardMastersId> GetAffectedDashboardRecord(string payload)
    {
        string[] payloadValues = payload.Split(',');

        // Assuming the payload contains values in the order of ElectionMasterId, StateMasterId, DistrictMasterId, PCMasterId, AssemblyMasterId, BoothMasterId
        // Access the elements of the array to retrieve the values of the columns
        var electionMasterId = payloadValues[0];
        var stateMasterId = payloadValues[1];
        var districtMasterId = payloadValues[2];
        var pcMasterId = payloadValues[3];
        var assemblyMasterId = payloadValues[4];
        var boothMasterId = payloadValues[5];
        DahboardMastersId dahboardMastersId = new DahboardMastersId()
        {
            ElectionMasterId = electionMasterId,
            StateMasterId = stateMasterId,
            DistrictMasterId = districtMasterId,
            PCMasterId = pcMasterId,
            AssemblyMasterId = assemblyMasterId,
            BoothMasterId = boothMasterId


        };
        return dahboardMastersId;
    }

    private async Task<PollInterruptionMastersId> GetAffectedPollInterutptionRecord(string payload)
    {
        string[] payloadValues = payload.Split(',');

        // Assuming the payload contains values in the order of ElectionMasterId, StateMasterId, DistrictMasterId, PCMasterId, AssemblyMasterId, BoothMasterId
        // Access the elements of the array to retrieve the values of the columns
        var pollInterruptionMasterId = payloadValues[0];
        var stateMasterId = payloadValues[1];
        var districtMasterId = payloadValues[2];
        var pcMasterId = payloadValues[3];
        var assemblyMasterId = payloadValues[4];
        var boothMasterId = payloadValues[5];
        PollInterruptionMastersId pollInterruptionMastersId = new PollInterruptionMastersId()
        {
            PollInterruptionMasterId = pollInterruptionMasterId,
            StateMasterId = stateMasterId,
            DistrictMasterId = districtMasterId,
            PCMasterId = pcMasterId,
            AssemblyMasterId = assemblyMasterId,
            BoothMasterId = boothMasterId


        };
        return pollInterruptionMastersId;
    }


}
