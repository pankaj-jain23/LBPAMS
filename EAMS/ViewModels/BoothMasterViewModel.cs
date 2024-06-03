using System.ComponentModel.DataAnnotations;

namespace EAMS.ViewModels
{
    public class BoothMasterViewModel
    {
        [Required(ErrorMessage = "State Master Id is required")]
        public int StateMasterId { get; set; }

        [Required(ErrorMessage = "District Master Id is required")]
        public int DistrictMasterId { get; set; }
        [Required(ErrorMessage = "Assembly Master Id is required")]
        public int AssemblyMasterId { get; set; }

        public int BoothMasterId { get; set; }
        [Required(ErrorMessage = "Booth Code No. is required")]
        public string BoothCode_No { get; set; }
        [Required(ErrorMessage = "Total Voters field is required")]
        public int? TotalVoters { get; set; }

        [Required(ErrorMessage = "Total No. of Male is required")]
        public int? Male { get; set; }
        [Required(ErrorMessage = "Total No. of Female is required")]
        public int? Female { get; set; }
        [Required(ErrorMessage = "Total No. of Transgender is required")]
        public int? Transgender { get; set; }

        [Required(ErrorMessage = "Booth Name is required")]
        public string? BoothName { get; set; }

        public string? BoothNoAuxy { get; set; }

        public double? Latitude { get; set; }

        public double? Longitude { get; set; }

        [Required(ErrorMessage = "Status is required")]
        public bool IsStatus { get; set; }

        public int? LocationMasterId { get; set; }


    }
}
