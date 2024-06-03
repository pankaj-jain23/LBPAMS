using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
