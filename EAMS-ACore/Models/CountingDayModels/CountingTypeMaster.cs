using System.ComponentModel.DataAnnotations;

namespace EAMS_ACore.Models.CountingDayModels
{
    public class CountingTypeMaster
    {
        [Key]
        public int CountingTypeMasterId { get; set; }
        public string CountingType { get; set; }
        public DateTime? CountingDateTime { get; set; }
        public bool CountingStatus { get; set; }
    }
}
