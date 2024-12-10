using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAMS_ACore.Models
{
    public class IsMasterEditable
    {
        public int MasterId { get; set; }
        public string Type { get; set; }
        public bool IsEditable { get; set; }
        public int ElectionTypeMasterId { get; set; }
        public string Message { get; set; }
    }
}
