using System.ComponentModel.DataAnnotations;

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
