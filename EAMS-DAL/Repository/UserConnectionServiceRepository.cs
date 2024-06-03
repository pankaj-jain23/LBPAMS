using EAMS_ACore.IRealTime;
using EAMS_ACore.IRepository;
using EAMS_ACore.SignalRModels;
using EAMS_DAL.DBContext;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace EAMS_DAL.Repository
{
    public class UserConnectionServiceRepository: IUserConnectionServiceRepository
    {
        private readonly EamsContext _context;
        public UserConnectionServiceRepository(EamsContext context)
        {
            _context = context;
        }
        public async void AddUser(string connectionId, ClaimsIdentity claimsIdentity)
        {
            var dashboardConnectedUser = new DashboardConnectedUser
            {
                ConnectionId = connectionId,
                Role = claimsIdentity.FindFirst(ClaimTypes.Role)?.Value,
                StateMasterId = GetClaimValue<int>(claimsIdentity, "StateMasterId"),
                DistrictMasterId = GetClaimValue<int>(claimsIdentity, "DistrictMasterId"),
                PCMasterId = GetClaimValue<int>(claimsIdentity, "PCMasterId"),
                AssemblyMasterId = GetClaimValue<int>(claimsIdentity, "AssemblyMasterId"),
                BoothMasterId = GetClaimValue<int>(claimsIdentity, "BoothMasterId"),
                UserConnectedTime=DateTime.UtcNow

            };

            _context.DashboardConnectedUser.Add(dashboardConnectedUser);
            _context.SaveChanges();
            // Add dashboardConnectedUser to database or any other operation you need to perform
        }
        public async Task<int?> GetDashboardConnectedUserCountByStateId(int stateMasterId)
        {
            DateTime today = DateTime.UtcNow.Date; // Get today's date

            var connectedUserConnect = await _context.DashboardConnectedUser
                .Where(d => d.StateMasterId == stateMasterId &&
                            d.UserConnectedTime.HasValue &&
                            d.UserConnectedTime.Value.Date == today&& d.Role!="SO" && d.Role != "BLO")
                .CountAsync();

            return connectedUserConnect;
        }
        public async Task<int?> GetMobileConnectedUserCountByStateId(int stateMasterId)
        {
            DateTime today = DateTime.UtcNow.Date; // Get today's date

            var connectedUserConnect = await _context.DashboardConnectedUser
                .Where(d => d.StateMasterId == stateMasterId &&
                            d.UserConnectedTime.HasValue &&
                            d.UserConnectedTime.Value.Date == today && d.Role == "SO" || d.Role == "BLO")
                .CountAsync();

            return connectedUserConnect;
        }

        private T GetClaimValue<T>(ClaimsIdentity claimsIdentity, string claimType)
        {
            var claim = claimsIdentity.FindFirst(claimType);
            if (claim != null && !string.IsNullOrEmpty(claim.Value))
            {
                try
                {
                    return (T)Convert.ChangeType(claim.Value, typeof(T));
                }
                catch (InvalidCastException)
                {
                    // Handle conversion errors
                }
            }

            // Default value if claim not found or conversion fails
            return default(T);
        }

        public async Task <DashboardConnectedUser> GetConnectedUser(string connectionId)
        {
            var getConnectedUser=_context.DashboardConnectedUser.Where(d=>d.ConnectionId== connectionId).FirstOrDefault();
            if (getConnectedUser == null)
            {
                return null;
            }
            else
            {
                return getConnectedUser;
            }
        }
        public async Task<string> RemoveUser(string connectionId)
        {
            var userConnection = _context.DashboardConnectedUser.FirstOrDefault(u => u.ConnectionId == connectionId);
            if (userConnection != null)
            {
                _context.DashboardConnectedUser.Remove(userConnection);
                _context.SaveChanges();
                return "User Deleted";
            }
            else
            {
                return "User not Deleted";
            }
        }
    }
}
