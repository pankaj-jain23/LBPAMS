using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAMS_ACore.Models
{
    public class EventByBooth
    {
        public int BoothMasterId { get; set; }
        public int EventMasterId { get; set; }
        public string EventName { get; set; }
        public int EventSequence { get; set; }
        public string EventABBR { get; set; }
        public bool Status { get; set; }
    }
}
