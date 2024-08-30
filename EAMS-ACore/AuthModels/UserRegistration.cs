using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EAMS_ACore.AuthModels
{
    public class UserRegistration : IdentityUser
    {
        public int? ElectionTypeMasterId { get; set; }
        public int? StateMasterId
        {
            get;
            set;
        }
        public int? DistrictMasterId
        {
            get;
            set;
        }
        public int? AssemblyMasterId
        {
            get;
            set;
        }
        public int? FourthLevelHMasterId
        {
            get;
            set;
        }
        public DateTime? OTPGeneratedTime { get; set; }
        public string? OTP { get; set; }
        public DateTime? OTPExpireTime { get; set; }
        public int OTPAttempts { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime RefreshTokenExpiryTime { get; set; }
    }



}
