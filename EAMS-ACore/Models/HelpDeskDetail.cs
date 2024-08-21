using System.ComponentModel.DataAnnotations;

namespace EAMS_ACore.Models
{
    public class HelpDeskDetail
    {
        [Key]
        public int HelpDeskMasterId { get; set; }

        public int StateMasterId { get; set; }
        public int? DistrictMasterId { get; set; }
        public int? PCMasterId { get; set; }
        public int? AssemblyMasterId { get; set; }
        public int? BoothMasterId { get; set; }
        public string ContactName { get; set; }
        public string MobileNumber { get; set; }
        public int? LandlineNumber { get; set; }
        public bool IsStatus { get; set; }

        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? DeletedAt { get; set; } = DateTime.UtcNow;

        public bool HelpDeskStatus { get; set; }


    }
}
