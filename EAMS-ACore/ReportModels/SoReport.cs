namespace EAMS_ACore.ReportModels
{
    public class SoReport
    {
        public int SoMasterId { get; set; }
        public string Header { get; set; }
        public string Title { get; set; }
        public string Type { get; set; }
        public int TotalPollingLocationCount { get; set; }
        public int TotalBoothCount { get; set; }
        public int TotalSOAppointedCount { get; set; }
        public string SOName { get; set; }
        public string SODesignation { get; set; }
        public string SOMobileNo { get; set; }
        public string Office { get; set; }
        public int BoothAllocatedCount { get; set; }
        public string AllocatedBoothNumbers { get; set; }
        public List<string> BoothAllocatedName { get; set; }
        public List<string> BoothLocationName { get; set; }
    }
}
