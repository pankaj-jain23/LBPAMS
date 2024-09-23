﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAMS_ACore.Models.EventActivityModels
{
    public class UpdateEventActivity
    {
        public int StateMasterId { get; set; }
        public int DistrictMasterId { get; set; }
        public int AssemblyMasterId { get; set; }
        public int ElectionTypeMasterId { get; set; }
        public int BoothMasterId { get; set; }
        public int EventMasterId { get; set; }
        public string EventABBR { get; set; }
        public int EventSequence { get; set; }
        public bool EventStatus { get; set; }
    }
    public class CheckEventActivity
    {
        public int StateMasterId { get; set; }
        public int DistrictMasterId { get; set; }
        public int AssemblyMasterId { get; set; }
        public int ElectionTypeMasterId { get; set; }
        public int BoothMasterId { get; set; }
        public int EventMasterId { get; set; }
        public string EventABBR { get; set; }
        public int EventSequence { get; set; }
        public bool EventStatus { get; set; }
    }
}