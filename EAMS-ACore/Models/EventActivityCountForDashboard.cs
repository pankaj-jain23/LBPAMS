using EAMS_ACore.Models.ElectionType;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAMS_ACore.Models
{
   
    public class EventActivityCountForDashboard
    {
        public int TotalBoothCount { get; set; }
        public List<EventActivityForDashboard> eventActivityForDashboardHeader { get; set; }
        public List<EventActivityForDashboardData> eventActivityForDashboardData { get; set; }
        
    }
    public class EventActivityForDashboard
    {
        public string EventName { get; set; }
        public string EventABBR { get; set; }
        public bool Status { get; set; }
    }
    public class EventActivityForDashboardData
    {
        public string TotalBoothCount { get; set; }
        public string EventName { get; set; }
        public string TotalBoothActivity { get; set; }
        public string ActivityPercentage { get; set; }
    }
}
