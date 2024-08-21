using System.ComponentModel.DataAnnotations;

namespace EAMS_ACore.Models.QueueModel
{
    public class QIS
    {
        [Key]
        public int QueueMasterId { get; set; }
        public int StateMasterId { get; set; }
        public int DistrictMasterId { get; set; }
        public int? PCMasterId { get; set; }
        public int AssemblyMasterId { get; set; }
        public int BLOMasterId { get; set; }
        public string BLOMobileNumber { get; set; }
        public int BoothMasterId { get; set; }
        public string? BoothCode { get; set; }
        public string? QueueEnterTime { get; set; }
        public DateTime? QueueEnterDateTime { get; set; }
        public int QueueCount { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
