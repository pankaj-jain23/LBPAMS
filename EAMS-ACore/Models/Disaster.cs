using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAMS_ACore.Models
{
    public class Disaster
    {
        public int FieldOfficerMasterId
        {
            get;
            set;
        }
        public int StateMasterId
        {
            get;
            set;
        }
        public int DistrictMasterId
        {
            get;
            set;
        }

        public int AssemblyMasterId
        {
            get;
            set;
        }
        public int FourthLevelMasterId
        {
            get;
            set;
        }
        public int ElectionTypeMasterId
        {
            get;
            set;
        }
        public List<int> BoothMasterId { get; set; }
    }





}
