﻿namespace EAMS.ViewModels
{
    public class DistrictMasterViewModel
    {
        public int StateMasterId { get; set; }
        public int DistrictMasterId { get; set; }
        public string? SecondLanguage { get; set; }
        public string DistrictName { get; set; }
        public string DistrictCode { get; set; }
        //public bool DistrictStatus { get; set; }
        public bool IsStatus { get; set; }


    }
}
