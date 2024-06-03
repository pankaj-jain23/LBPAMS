using EAMS.ViewModels.PSFormViewModel;

namespace EAMS_ACore.ReportModels
{
    public class VTReportModel
    {
        public string Header { get; set; }
        public string Title { get; set; }
        public string Type { get; set; }
        
        public string DistrictName { get; set; }
        public string DistrictCode { get; set; }
        public string AssemblyName { get; set; }
        public string AssemblyCode { get; set; }
        public string PCName { get; set; }
        public string PCCode { get; set; }
        public string BoothName { get; set; }
        public string BoothCode { get; set; }

        
        public int? MaleElectoral { get; set; }
        public int? FemaleElectoral { get; set; }
        public int? ThirdGenderElectoral { get; set; }
        public int? TotalElectoral { get; set; }

        
        public int? MaleVoters { get; set; }
        public int? FemaleVoters { get; set; }
        public int? ThirdGenderVoters { get; set; }
        public int? TotalVoters { get; set; }

       
        public int? EPIC { get; set; }
        public int? VotePolledOtherDocument { get; set; }


        public double MalePercentage { get; set; }
        public double FemalePercentage { get; set; }
        public double ThirdGenderPercentage { get; set; }
        public double TotalPercentage { get; set; }

        public int TotalCUsUsed { get; set; }
        public int TotalBUsUsed { get; set; }
        public int TotalVVPATUsed { get; set; }

        public int? OverseasElectoral { get; set; }
        public int? PWdEelectoral { get; set; }
        public int? YoungElectoral { get; set; }
        public int? FortyNineVoters { get; set; }
        public int? TenderedVotes { get; set; }

    }

}
