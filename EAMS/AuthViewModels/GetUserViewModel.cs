namespace EAMS.AuthViewModels
{
    public class GetUserViewModel
    {
        public int StateMasterId { get; set; }
        public int DistrictMasterId { get; set; }
        public int PCMasterId { get; set; }
        public int AssemblyMasterId { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
    }
}
