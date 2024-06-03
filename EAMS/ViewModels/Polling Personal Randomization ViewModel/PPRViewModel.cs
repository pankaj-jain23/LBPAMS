namespace EAMS.ViewModels.Polling_Personal_Randomization_ViewModel
{
    public class PPRViewModel
    {
        public int StateMasterId { get; set; }
        public int? DistrictMasterId { get; set; }
        public int? PCMasterId { get; set; }
        public int? AssemblyMasterId { get; set; }
        public int? RandomizationTaskDetailMasterId { get; set; } 
        public int NumberOfRounds { get; set; }
        public DateTime? DateOfRound { get; set; }
        public DateTime? DateOfCompletedRound { get; set; }
        public DateTime? DateOfPostponedRound { get; set; }
    }
}
