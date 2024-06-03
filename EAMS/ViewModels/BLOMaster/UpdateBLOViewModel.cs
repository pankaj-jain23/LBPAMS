namespace EAMS.ViewModels.BLOMaster
{
    public class UpdateBLOViewModel
    {
        public int BLOMasterId { get; set; }
        public int StateMasterId { get; set; }
        public int DistrictMasterId { get; set; } 
        public int AssemblyMasterId { get; set; }
        public string BLOMobile { get; set; }
        public string BLOName { get; set; }
        public bool IsStatus { get; set; }
    }
}
