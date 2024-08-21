namespace EAMS.ViewModels
{
    public class HelpDeskViewModel
    {

        public int StateMasterId { get; set; }
        public int? DistrictMasterId { get; set; }
        public int? PCMasterId { get; set; }
        public int? AssemblyMasterId { get; set; }
        public int? BoothMasterId { get; set; }
        public string ContactPersonName { get; set; }
        public string MobileNumber { get; set; }
        public int? LandlineNumber { get; set; }
        public bool IsStatus { get; set; }
    }
}
