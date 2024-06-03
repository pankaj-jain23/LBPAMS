using EAMS.ViewModels.PSFormViewModel;

namespace EAMS_ACore.ReportModels
{
    public class VTPSReportReportModel
    {
        public string Header { get; set; }
        public string Title { get; set; }
        public string Type { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string StateName { get; set; }
        public string StateCode { get; set; }
        public string DistrictName { get; set; }
        public string DistrictCode { get; set; }
        public string AssemblyName { get; set; }
        public string AssemblyCode { get; set; }
        public string PCName { get; set; }
        public string PCCode { get; set; }
        public string BoothName { get; set; }
        public string BoothCode { get; set; }
        
        public int? TotalEVMS { get; set; }
        public int Male { get; set; }
        public int Female { get; set; }
        public int ThirdGender { get; set; }
        //public int Type { get; set; }
        public int Total { get; set; }
        public int? OverseasElectoral { get; set; }
        public int? PWdEelectoral { get; set; }
        public int? YoungElectoral { get; set; }
        public int? TenderedVotes { get; set; }
        public int? EPIC { get; set; }
        public int? VotePolledOtherDocument { get; set; }
        public bool IsStatus { get; set; }
        //public List<VTPSReportModel> VTPSReportModel { get; set; }
    }
    //public class VTPSReportModel
    //{
      
    //    public int Male { get; set; }
    //    public int Female { get; set; }
    //    public int ThirdGender { get; set; }
    //    //public int Type { get; set; }
    //    public int Total { get; set; }
    //}
}
