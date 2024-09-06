namespace EAMS_ACore.Models
{
    public class DashBoardProfile
    {
        public string? Name { get; set; }
        public string? MobileNumber { get; set; }
        public string? UserType { get; set; }
        public string? UserEmail { get; set; }
        public List<string> Roles { get; set; }
         
        public int? ElectionTypeMasterId { get; set; }
        public string? ElectionName { get; set; }
        public int? StateMasterId { get; set; }
        public string? StateName { get; set; }
        public int? DistrictMasterId { get; set; }
        public string? DistrictName { get; set; }
        public int? AssemblyMasterId { get; set; }
        public string? AssemblyName { get; set; }
        public int? FourthLevelHMasterId { get; set; }
        public string? FourthLevelHName { get; set; }

    }
}
