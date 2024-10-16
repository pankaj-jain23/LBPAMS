using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAMS_ACore.Models.PublicModels
{
    public class ResultDeclarationReportListModel
    {
        public int StateMasterId
        {
            get;
            set;
        }

        public int? DistrictMasterId
        {
            get;
            set;
        }

        public int? ElectionTypeMasterId
        {
            get;
            set;
        }

        public int? AssemblyMasterId //ZP
        {
            get;
            set;
        }

        public int? FourthLevelHMasterId // GPSarpanch // MCorp //MC //NP

        {
            get;
            set;
        }
    }
}
