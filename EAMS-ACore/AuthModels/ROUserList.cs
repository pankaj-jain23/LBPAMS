﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAMS_ACore.AuthModels
{
    public class ROUserList
    {
        public int? StateMasterId { get; set; }
        public int? DistrictMasterId { get; set; }
        public int? AssemblyMasterId { get; set; }
        public int? FourthLevelHMasterId { get; set; }
        public int? ElectionTypeMasterId { get; set; }
        public string UserName { get; set; }
        public string Id { get; set; }
        public string? PhoneNumber { get; set; }
    }
}
