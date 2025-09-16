using System.ComponentModel.DataAnnotations;

namespace EAMS_ACore.Models.FOPanchayatZPAndPSMapping
{
    public class ZpPsFOMapping
    {
        [Key]
        public int Id
        {
            get;
            set;
        }
        public int FieldOfficerMasterId
        {
            get;
            set;
        }
        public int PanchayatMappingId
        {
            get;
            set;
        }
        public int BoothMasterId
        {
            get;
            set;
        }
        public int ElectionTypeMasterId
        {
            get;
            set;
        }
        public string AssignedBy { get; set; }
    }
}
