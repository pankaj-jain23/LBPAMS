using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAMS_ACore.Models.PublicModels
{
    public class VoterType
    {
        [Key]
        public int VoterTypeMasterId { get; set; }

        public string VoterTypeName { get; set; }

    }
}
