namespace EAMS.ViewModels
{
    public class LocationViewModel
    {
        public int? LocationMasterId { get; set; }
        public int? StateMasterId { get; set; }
        public int? PCMasterId { get; set; }
        public int? DistrictMasterId { get; set; }
        public int? AssemblyMasterId { get; set; }
        public string LocationName { get; set; }
        public string LocationCode { get; set; }
        public string LocationLatitude { get; set; }
        public string LocationLongitude { get; set; }
        public List<int> BoothMasterId { get; set; }
        public bool IsStatus { get; set; }
        public DateTime? CreatedOn { get; set; }

    }
}
