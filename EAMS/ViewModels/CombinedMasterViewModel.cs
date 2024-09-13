namespace EAMS.ViewModels
{
    public class CombinedMasterViewModel
    {
        public int StateMasterId { get; set; }
        public int DistrictMasterId { get; set; }
        public int AssemblyMasterId { get; set; }
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
        public string? SecondLanguage { get; set; }
    }
}
