using System.ComponentModel.DataAnnotations;

namespace EAMS_ACore.Models.ElectionType
{
    public class ElectionTypeMaster
    {
        [Key]
        public int ElectionTypeMasterId { get; set; }

        public string ElectionType { get; set; }
        public bool ElectionStatus { get; set; }

        public string? Hierarchy1 { get; set; }
        public string? Hierarchy2 { get; set; }

        public string ElectionArea { get; set; }


    }

    
}
