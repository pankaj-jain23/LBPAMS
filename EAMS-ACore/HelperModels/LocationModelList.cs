namespace EAMS_ACore.HelperModels
{
    public class LocationModelList
    {
        public int? LocationMasterId { get; set; }
        public int? StateMasterId { get; set; }
        public int? PCMasterId { get; set; }
        public int? DistrictMasterId { get; set; }
        public int? AssemblyMasterId { get; set; }

        public string StateName { get; set; }
        public string DistrictName { get; set; }
        public string AssemblyName { get; set; }
        public string PCName { get; set; }
        public string LocationName { get; set; }
        public string LocationCode { get; set; }
        public string LocationLatitude { get; set; }
        public string LocationLongitude { get; set; }
        public bool IsStatus { get; set; }

        public DateTime? CreatedOn { get; set; }
        public string BoothNumbers { get; set; }

    }
}
