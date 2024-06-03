using EAMS_ACore.Models.Polling_Personal_Randomisation_Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAMS_ACore.Models.Polling_Personal_Randomization_Models
{
    public class RandomizationTaskDetail
    {
        [Key]
        public int RandomizationTaskDetailMasterId { get; set; } 
        public int StateMasterId { get; set; }
        public string TaskName { get; set; }
        public int NumberOfRound { get; set; }
    }
}
