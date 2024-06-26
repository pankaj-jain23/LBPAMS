using EAMS_ACore.Models.Polling_Personal_Randomization_Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAMS_ACore.Models.Polling_Personal_Randomisation_Models
{
    public class PPR
    {
        //PPR -Polling Personal Randomization  
        [Key]
        public int PPRMasterId { get; set; }
        public int StateMasterId { get; set; }
        public int? DistrictMasterId { get; set; }      
        public int RandomizationTaskDetailMasterId { get; set; }    
        public int CurrentRound { get; set; }
        public DateTime? DateOfRound { get; set; }
        public DateTime? DateOfCompletedRound { get; set; }
        public DateTime? DateOfPostponedRound { get; set; } 
    }
}
