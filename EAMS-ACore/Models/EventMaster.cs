﻿using System.ComponentModel.DataAnnotations;

namespace EAMS_ACore
{
    public class EventMaster
    {
        [Key]
        public int EventMasterId { get; set; }

        public string EventName { get; set; }

        public int EventSequence { get; set; }

        public DateTime? StartDateTime { get; set; }

        public DateTime? EndDateTime { get; set; }

        public bool Status { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public DateTime? DeletedAt { get; set; }
    }
}
