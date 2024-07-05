namespace EAMS.ViewModels
{
    public class SectorOfficerBoothViewModel
    {

        public int StateMasterId { get; set; }
        public int DistrictMasterId { get; set; }
        public int AssemblyMasterId { get; set; }
        public int SoId { get; set; }
        public int BoothMasterId { get; set; }
        public string BoothName { get; set; }
        public string BoothCode_No { get; set; }
        public string? BoothAuxy { get; set; }
        public bool IsAssigned { get; set; }
        public int ElectionTypeMasterId { get; set; }

    }
}
