using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAMS_ACore.SignalRModels
{
    public class DashboardConnectedUser
    {
        [Key]
        public int DashboardConnectedUserId { get; set; }
         public string ConnectionId { get; set; }
        public string Role { get; set; }
        public int StateMasterId { get; set; }
        public int DistrictMasterId { get; set; }
        public int PCMasterId { get; set; }
        public int AssemblyMasterId { get; set; }
        public int BoothMasterId { get; set; }
        public DateTime? UserConnectedTime { get; set; }  

    }
}
