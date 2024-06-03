using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAMS_ACore.AuthModels
{
    public class GetUser
    {
        public int StateMasterId { get; set; }
        public int DistrictMasterId { get; set; }
        public int PCMasterId { get; set; }
        public int AssemblyMasterId { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
    }
}
