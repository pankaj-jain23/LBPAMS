using System.ComponentModel.DataAnnotations;

namespace EAMS_ACore.Models
{
    public class PollingLocationMaster
    {
        [Key]
        public int LocationMasterId { get; set; }
        [Required(ErrorMessage = "StateMasterId is required")]
        public int? StateMasterId { get; set; }
        [Required(ErrorMessage = "DistrictMasterId is required")]
        public int? DistrictMasterId { get; set; }
        [Required(ErrorMessage = "AssemblyMasterId is required")]
        public int? AssemblyMasterId { get; set; }
        public int? PCMasterId { get; set; }
        public string LocationName { get; set; }
        public string LocationCode { get; set; }
        public string? LocationLatitude { get; set; }
        public string? LocationLongitude { get; set; }
        public bool Status { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime? UpdatedOn { get; set; }
    }
}
