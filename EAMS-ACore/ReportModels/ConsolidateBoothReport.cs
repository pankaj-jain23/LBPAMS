namespace EAMS_ACore.ReportModels
{
    public class ConsolidateBoothReport
    {
        public string Header { get; set; }
        public string Title { get; set; }
        public string Type { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string DistrictName { get; set; }
        public string LocationName { get; set; }
        public int? TotalNumberOfBooths { get; set; }
        public int? TotalNumberOfBoothsEntered { get; set; }
        public int? Male { get; set; }
        public int? Female { get; set; }
        public int? Trans { get; set; }
        public int? Total { get; set; }
        public bool IsStatus { get; set; }
    }

}
