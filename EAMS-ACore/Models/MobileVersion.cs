using System.ComponentModel.DataAnnotations;

namespace EAMS_ACore.Models
{
    public class MobileVersion
    {
        [Key]
        public int MobileVersionId { get; set; }
        public int StateMasterId { get; set; }
        public string CurrentMobileVersion { get; set; }
    }
}
