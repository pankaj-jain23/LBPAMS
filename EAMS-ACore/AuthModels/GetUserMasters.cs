namespace EAMS_ACore.AuthModels
{
    public class GetUserMasters
    {
        public List<int> StateMasterId { get; set; }
        public List<int> PCMasterId { get; set; }
        public List<int> DistrictMasterId { get; set; }
        public List<int> AssemblyMasterId { get; set; }
    }
}
