﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EAMS_ACore
{
    public class ParliamentConstituencyMaster
    {
        [Key]
        public int PCMasterId { get; set; }

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
        public int ElectionTypeId { get; set; }

        public string? PcCodeNo { get; set; }

        public string? PcName { get; set; }

        public string? PcType { get; set; }

        public DateTime? PcCreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? PcUpdatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? PcDeletedAt { get; set; } = DateTime.UtcNow;

        public bool PcStatus { get; set; }
        public string? SecondLanguage { get; set; }

        public virtual List<AssemblyMaster>? AssemblyMaster
        {
            get;
            set;
        }
    }
}
