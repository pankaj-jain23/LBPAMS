using System.ComponentModel.DataAnnotations;

namespace EAMS_ACore.Models
{
    public class PanchayatMapping
    {
        [Key]
        public int  Id
        {
            get;
            set;
        }


        public string?  Type
        {
            get;
            set;
        }

        public int ElectionTypeMasterId
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
       
        public int FourthLevelHMasterId
        {
            get;
            set;
        }

        public int PSZonePanchayatMasterId
        {
            get;
            set;
        }

        public bool  Status { get; set; }


       
    }
}
