using System.ComponentModel.DataAnnotations;

namespace EAMS.ViewModels
{
    public class AddStateMasterViewModel
    {
        [Required(ErrorMessage = "State Name is required")]
        
        public string StateName { get; set; }

        [Required(ErrorMessage = "State Code is required")]
        public string StateCode { get; set; }         
                
        [Required(ErrorMessage = "ElectionTypeId is required")]
        public int ElectionTypeId { get; set; }

        [Required(ErrorMessage = "Sttaus is required")]
        public bool IsStatus { get; set; }

        public bool IsGenderCapturedinVoterTurnOut { get; set; }
    }
}
