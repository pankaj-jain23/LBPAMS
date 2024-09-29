namespace EAMS_ACore.Models
{
    public class FieldOfficerProfile
    {
        public int? StateMasterId
        {
            get;
            set;
        }
        public string StateName { get; set; }
        public int? DistrictMasterId
        {
            get;
            set;
        }
        public string DistrictName { get; set; }
        public int? AssemblyMasterId //ZP
        {
            get;
            set;
        }

        public string AssemblyName { get; set; }
        public int? FourthLevelHMasterId
        {
            get;
            set;
        }
        public string? HierarchyName { get; set; }
        public string FoName { get; set; }
        public string Role { get; set; }
        public List<string> BoothNo { get; set; }
        public int ElectionTypeMasterId { get; set; }
        public string ElectionTypeName { get; set; }
        public bool IsPrimaryBooth { get; set; }
    }
}
