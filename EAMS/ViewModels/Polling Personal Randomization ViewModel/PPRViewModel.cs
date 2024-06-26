namespace EAMS.ViewModels.Polling_Personal_Randomization_ViewModel
{
    public class PPRViewModel
    {
        public int StateMasterId { get; set; }
        public int? DistrictMasterId { get; set; }       
        public int? RandomizationTaskDetailMasterId { get; set; } 
        public int CurrentRound { get; set; }
        public string? DateOfRound { get; set; }
        public string? DateOfCompletedRound { get; set; }
        public string? DateOfPostponedRound { get; set; }
    }
    public class PPRViewUpdateModel
    {
        public int PPRMasterId { get; set; }
        public int StateMasterId { get; set; }
        public int? DistrictMasterId { get; set; }
        public int? RandomizationTaskDetailMasterId { get; set; }
        public int CurrentRound { get; set; }
        public string? DateOfRound { get; set; }
        public string? DateOfCompletedRound { get; set; }
        public string? DateOfPostponedRound { get; set; }
    }
}
