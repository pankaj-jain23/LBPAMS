﻿namespace LBPAMS.ViewModels
{
    public class EventByBoothViewModel
    {
        public int? BoothMasterId { get; set; }
        public int? EventMasterId { get; set; }
        public string? EventName { get; set; }
        public int? EventSequence { get; set; }
        public string? EventABBR { get; set; }
        public bool? Status { get; set; }
    }
}
