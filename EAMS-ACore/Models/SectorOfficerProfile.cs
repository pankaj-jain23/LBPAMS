namespace EAMS_ACore.Models
{
    public class SectorOfficerProfile
    {
        public string StateName { get; set; }
        public string DistrictName { get; set; }
        public string AssemblyName { get; set; }
        public string AssemblyCode { get; set; }
        public string SoName { get; set; }
        public string ElectionType { get; set; }
        public string OfficerRole { get; set; }
        public List<string> BoothNo { get; set; }
    }
}
