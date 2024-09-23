﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAMS_ACore.Models
{
    public class FieldOfficerMasterList
    {
        public int FieldOfficerMasterId { get; set; }
        public int StateMasterId
        {
            get;
            set;
        }
        public string StateName
        {
            get;
            set;
        }
        public int DistrictMasterId
        {
            get;
            set;
        }
        public string DistrictName
        {
            get;
            set;
        }
        public int? AssemblyMasterId
        {
            get;
            set;
        }
        public string AssemblyName
        {
            get;
            set;
        }
        public int AssemblyCode { get; set; }
        public string FieldOfficerName { get; set; }
        public string FieldOfficerDesignation { get; set; }
        public string FieldOfficerOfficeName { get; set; }
        public string FieldOfficerMobile { get; set; }
        public bool FieldOfficerStatus { get; set; }
        public DateTime? OTPGeneratedTime { get; set; }
        public string? OTP { get; set; }
        public DateTime? OTPExpireTime { get; set; }
        public int OTPAttempts { get; set; }
        public bool IsLocked { get; set; }
        public int ElectionTypeMasterId { get; set; }
        public string ElectionTypeName { get; set; }
    }
}