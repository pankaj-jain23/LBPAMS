using EAMS_ACore.AuthModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAMS_ACore.Models.ElectionType
{
    public class ElectionTypeMaster
    {
        [Key] 
        public int ElectionTypeMasterId { get; set; }

        public string ElectionType { get; set; }
        public bool ElectionStatus { get; set; }

        public string? Hierarchy1 {  get; set; }
        public string? Hierarchy2 {  get; set; }
        

    }
}
