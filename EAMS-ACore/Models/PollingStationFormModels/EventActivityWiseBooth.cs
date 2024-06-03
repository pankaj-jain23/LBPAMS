using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAMS_ACore.Models.PollingStationFormModels
{
    public class EventActivityWiseBooth
    {
        public int StateMasterId { get; set; }
        public int DistrictMasterId { get; set; }
        public int AssemblyMasterId { get; set; }
        public int BoothMasterId { get; set; }
        public string BoothCode { get; set; }
        public List<int> EventsCompleted { get; set; }
        public string BoothName { get; set; }
        public string Color { get; set; }
        public bool? UpdateStatus { get; set; }

    }
     
}
