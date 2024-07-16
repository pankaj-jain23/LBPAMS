using EAMS_ACore.Models.ElectionType;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAMS_ACore.Models
{
    public class Level4
    {
        [Key]
        public int Level4MasterId { get; set; }
        public string Level4Name { get; set; }
        public int Level4Code { get; set; }

        public string? Level4Type { get; set; }

        public int ElectionTypeMasterId
        {
            get;
            set;
        }
        [ForeignKey("ElectionTypeMasterId")]
        public virtual ElectionTypeMaster? ElectionTypeMaster
        {
            get;
            set;
        }

        public int StateMasterId
        {
            get;
            set;
        }
        [ForeignKey("StateMasterId")]
        public virtual StateMaster? StateMaster
        {
            get;
            set;
        }

        public int DistrictMasterId
        {
            get;
            set;
        }
        [ForeignKey("DistrictMasterId")]
        public virtual DistrictMaster? DistrictMaster
        {
            get;
            set;
        }
        public int? PCMasterId
        {
            get;
            set;
        }
        [ForeignKey("PCMasterId")]
        public virtual ParliamentConstituencyMaster? ParliamentConstituencyMaster
        {
            get;
            set;
        }

        public int TotalLevel4 { get; set; }
        public DateTime? Level4CreatedAt { get; set; }

        public DateTime? Level4UpdatedAt { get; set; }

        public DateTime? Level4DeletedAt { get; set; }
        public bool Level4Status { get; set; }
        public string? SecondLanguage { get; set; }
        public virtual List<BoothMaster> BoothMaster
        {
            get;
            set;
        }
    }
}
