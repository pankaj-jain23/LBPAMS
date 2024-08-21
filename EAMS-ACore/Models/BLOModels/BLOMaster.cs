using System.ComponentModel.DataAnnotations;

namespace EAMS_ACore.Models.BLOModels
{
    public class BLOMaster
    {
        [Key]
        public int BLOMasterId { get; set; }
        public int StateMasterId { get; set; }
        public int DistrictMasterId { get; set; }
        public int? PCMasterId { get; set; }
        public int AssemblyMasterId { get; set; }
        public string BLOMobile { get; set; }
        public string BLOName { get; set; }
        public bool BLOStatus { get; set; }
        public DateTime? BLOCreatedAt { get; set; }

        public DateTime? BLOUpdatedAt { get; set; }
        public DateTime? OTPGeneratedTime { get; set; }
        public string? OTP { get; set; }
        public DateTime? OTPExpireTime { get; set; }
        public int OTPAttempts { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime RefreshTokenExpiryTime { get; set; }
        public int AppPin { get; set; }
        public bool IsLocked { get; set; }

    }
}
