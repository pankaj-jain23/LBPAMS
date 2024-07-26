﻿using EAMS_ACore.Models.CountingDayModels;
using EAMS_ACore.Models.ElectionType;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EAMS_ACore.AuthModels
{
    public class UserRegistration : IdentityUser
    {
        public int? ElectionTypeMasterId { get; set; }
        public virtual List<UserState> UserStates { get; set; }
        public DateTime? OTPGeneratedTime { get; set; }
        public string? OTP { get; set; }
        public DateTime? OTPExpireTime { get; set; }
        public int OTPAttempts { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime RefreshTokenExpiryTime { get; set; }
    }

    public class UserState
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserStateId { get; set; }
        public int? StateMasterId { get; set; }
        public string Id { get; set; }
        [ForeignKey("Id")]
        public virtual UserRegistration UserRegistration { get; set; }

        public virtual List<UserDistrict> UserDistrict { get; set; }
        public virtual List<UserPCConstituency> UserPCConstituency { get; set; }

    }

    public class UserDistrict
    {
        [Key]
        public int UserDistrictId { get; set; }
        public int? DistrictMasterId { get; set; }
        public int UserStateId { get; set; }
        [ForeignKey("UserStateId")]
        public virtual UserState UserState { get; set; }
        public virtual List<UserAssembly> UserAssembly { get; set; }

    }
    public class UserPCConstituency
    {
        [Key]
        public int UserPCConstituencyId { get; set; }
        public int? PCMasterId { get; set; }
        public int UserStateId { get; set; }
        [ForeignKey("UserStateId")]
        public virtual UserState UserState { get; set; }
        public virtual List<UserAssembly> UserAssembly { get; set; }

    }
    public class UserAssembly
    {
        [Key]
        public int UserAssemblyId { get; set; }
        public int? AssemblyMasterId { get; set; }
        public int? UserDistrictId { get; set; }
        [ForeignKey("UserDistrictId")]
        public virtual UserDistrict UserDistrict { get; set; }
        public int? UserPCConstituencyId { get; set; }
        [ForeignKey("UserPCConstituencyId")]
        public virtual UserPCConstituency UserPCConstituency { get; set; }
        public virtual List<UserPSZone> UserPSZone { get; set; }
    }
    public class UserPSZone
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserPSZoneId { get; set; }
        public int? PSZoneMasterId { get; set; }
        public int? UserAssemblyId { get; set; }
        [ForeignKey("UserAssemblyId")]
        public virtual UserAssembly UserAssembly { get; set; }

    }



}
