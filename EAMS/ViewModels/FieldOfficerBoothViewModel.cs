namespace EAMS.ViewModels
{
    public class FieldOfficerBoothViewModel
    {

        public int StateMasterId { get; set; }
        public int DistrictMasterId { get; set; }
        public int AssemblyMasterId { get; set; }
        public int FieldOfficerMasterId { get; set; }
        public int? FourthLevelHMasterId
        {
            get;
            set;
        }
        public string? FourthLevelHName { get; set; }
        public int BoothMasterId { get; set; }
        public string BoothName { get; set; }
        public string BoothCode_No { get; set; }
        public string? BoothAuxy { get; set; }
        public bool IsAssigned { get; set; }
        public int ElectionTypeMasterId { get; set; }
        public int EventMasterId { get; set; }
        public string? EventName { get; set; }
        public string? EventABBR { get; set; }
        public int EventSequence { get; set; }
        public bool EventStatus { get; set; } 
        public bool IsBoothInterrupted { get; set; }
        public bool IsVTInterrupted { get; set; }//Voter Turnout Event 

    }
}
