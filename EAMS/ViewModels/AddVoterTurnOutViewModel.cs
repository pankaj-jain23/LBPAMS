﻿namespace EAMS_ACore.Models
{
    public class AddVoterTurnOutViewModel
    {

        public string boothMasterId { get; set; }

        public string eventid { get; set; }
        public string? voterValue { get; set; }
        public string? Male { get; set; }
        public string? Female { get; set; }
        public string? Transgender { get; set; }
        public int ElectionTypeMasterId { get; set; }


    }
}
