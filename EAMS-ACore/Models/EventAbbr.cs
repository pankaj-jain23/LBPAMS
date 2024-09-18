using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAMS_ACore.Models
{
    public class EventAbbr
    {
        [Key]
        public int EventAbbrId { get; set; }
        public string EventAbbrName { get; set; }
        public string EventDescription { get; set; }
    }
}
