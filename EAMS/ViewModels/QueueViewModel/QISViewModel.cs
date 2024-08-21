using System.Text.Json.Serialization;

namespace EAMS.ViewModels.QueueViewModel
{
    public class QISViewModel
    {
        [JsonIgnore]
        public int StateMasterId { get; set; }

        [JsonIgnore]
        public int DistrictMasterId { get; set; }

        [JsonIgnore]
        public int AssemblyMasterId { get; set; }
        public int BoothMasterId { get; set; }

        public string? BoothCode { get; set; }

        [JsonIgnore]
        public int BLOMasterId { get; set; }

        [JsonIgnore]
        public string? BLOMobileNumber { get; set; }
        [JsonIgnore]
        public string QueueEnterTime { get; set; } = DateTime.Now.ToString("HH:mm");

        [JsonIgnore]
        public DateTime? QueueEnterDateTime { get; set; } = DateTime.UtcNow;

        public int QueueCount { get; set; }


    }
}
