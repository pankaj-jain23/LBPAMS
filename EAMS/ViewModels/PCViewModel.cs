using System.ComponentModel.DataAnnotations;

namespace EAMS.ViewModels
{
    public class PCViewModel
    {
        public int? PCMasterId { get; set; }
        public int? StateMasterId { get; set; }
        [Required(ErrorMessage = "PC Code is required")]
        public string? PcCodeNo { get; set; }
        [Required(ErrorMessage = "PC Name is required")]
        public string? PcName { get; set; }
        public string? SecondLanguage { get; set; }
        [Required(ErrorMessage = "PC Type is required")]

        public string? PcType { get; set; }
        [Required(ErrorMessage = "PC Status is required")]
        public bool IsStatus { get; set; }
    }
}
