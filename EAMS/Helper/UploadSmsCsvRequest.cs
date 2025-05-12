namespace LBPAMS.Helper
{
    public class UploadSmsCsvRequest
    {
        public IFormFile File { get; set; }
        public int StateMasterId { get; set; }
        public int DistrictMasterId { get; set; }
    }

}
