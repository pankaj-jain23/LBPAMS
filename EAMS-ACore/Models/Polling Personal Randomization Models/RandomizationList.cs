namespace EAMS_ACore.Models.Polling_Personal_Randomization_Models
{

    public class RandomizationTaskRounds
    {
        public string TaskName { get; set; }
        public List<RoundDetails> Rounds { get; set; }
    }

    public class RandomizationTableList
    {
        public int StateMasterId { get; set; }
        public string StateName { get; set; }
        public int DistrictMasterId { get; set; }
        public string DistrictName { get; set; }
        public List<RandomizationTaskRounds> Tasks { get; set; }
    }
    public class RandomizationTableListState
    {
        public int StateMasterId { get; set; }
        public int? DistrictMasterId { get; set; }
        public string TaskName { get; set; }
        public List<RoundDetails> Rounds { get; set; }
    }
    public class RoundDetails
    {
        public int RoundNumber { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? PostponedDate { get; set; }
    }

    public class RandomizationList
    {
        public int PPRMasterId { get; set; }
        public int? StateMasterId { get; set; }
        public string? StateName { get; set; }
        public int? DistrictMasterId { get; set; }
        public string? DistrictName { get; set; }
        public int? TaskId { get; set; }
        public string? TaskName { get; set; }

        public int? TotalRound { get; set; }
        public int? RoundNumber { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? PostponedDate { get; set; }

    }

}
