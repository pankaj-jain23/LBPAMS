using System.ComponentModel.DataAnnotations;

namespace LBPAMS.ViewModels
{
    public class PSPanchayatMappingViewModel
    {
       
            [Required(ErrorMessage = "State Master Id is required")]
            public int StateMasterId { get; set; }

            [Required(ErrorMessage = "District Master Id is required")]
            public int DistrictMasterId { get; set; }

            [Required(ErrorMessage = "Assembly Master Id is required")]
            public int AssemblyMasterId { get; set; }

            [Required(ErrorMessage = "FourthLevelHMasterId     is required")]
            public int FourthLevelHMasterId { get; set; } 
            public List<int> PSZonePanchayatMasterId { get; set; } 
            public bool IsAssigned { get; set; }

            [Required(ErrorMessage = "Election Type Master Id is required")]
            public int ElectionTypeMasterId { get; set; }

            public string AssginedType { get; set; } 
       
    }
    public class ZPPanchayatMappingViewModel
    {

        [Required(ErrorMessage = "State Master Id is required")]
        public int StateMasterId { get; set; }

        [Required(ErrorMessage = "District Master Id is required")]
        public int DistrictMasterId { get; set; }

        [Required(ErrorMessage = "Assembly Master Id is required")]
        public int AssemblyMasterId { get; set; }

        [Required(ErrorMessage = "FourthLevelHMasterId is required")]
        public List<int> FourthLevelHMasterId { get; set; }
        public bool IsAssigned { get; set; }

        [Required(ErrorMessage = "Election Type Master Id is required")]
        public int ElectionTypeMasterId { get; set; }

        public string AssginedType { get; set; }

    }

    public class RqZPPanchayatMappingViewModel
    {

        [Required(ErrorMessage = "State Master Id is required")]
        public int StateMasterId { get; set; }

        [Required(ErrorMessage = "District Master Id is required")]
        public int DistrictMasterId { get; set; }

        [Required(ErrorMessage = "Assembly Master Id is required")]
        public int AssemblyMasterId { get; set; }

       
        [Required(ErrorMessage = "Election Type Master Id is required")]
        public int ElectionTypeMasterId { get; set; }
 

    }
    public class RqPSPanchayatMappingViewModel
    {

        [Required(ErrorMessage = "State Master Id is required")]
        public int StateMasterId { get; set; }

        [Required(ErrorMessage = "District Master Id is required")]
        public int DistrictMasterId { get; set; }

        [Required(ErrorMessage = "Assembly Master Id is required")]
        public int AssemblyMasterId { get; set; }

        [Required(ErrorMessage = "FourthLevelHMasterId is required")]
        public int FourthLevelHMasterId { get; set; }


        [Required(ErrorMessage = "Election Type Master Id is required")]
        public int ElectionTypeMasterId { get; set; }


    }
}
