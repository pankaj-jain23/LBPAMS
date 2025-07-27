using EAMS_ACore.Models;
using EAMS_ACore.Models.PublicModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAMS_ACore.ServiceModels
{
    public class RoleBasedFilterResult
    {
        public IQueryable<ElectionInfoMaster> ElectionQuery { get; set; }
        public IQueryable<BoothMaster> Booths { get; set; }
        public IQueryable<FourthLevelH> FourthLevel { get; set; }
        public IQueryable<GPPanchayatWards> GpWards { get; set; }
        public IQueryable<Kyc> UnOpposed { get; set; }
        public IQueryable<ResultDeclaration> Winners { get; set; }
        public int SarpanchContesting { get; set; }
        public int PanchContesting { get; set; }
    }

}
