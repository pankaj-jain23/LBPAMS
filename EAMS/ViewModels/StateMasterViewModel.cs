﻿namespace EAMS.ViewModels
{
    public class StateMasterViewModel
    {
        public int StateId { get; set; }
        public string StateName { get; set; }
        public string? SecondLanguage { get; set; }
        public string StateCode { get; set; }

        public bool IsStatus { get; set; }
        public bool IsGenderCapturedinVoterTurnOut { get; set; }
     
    }
}
