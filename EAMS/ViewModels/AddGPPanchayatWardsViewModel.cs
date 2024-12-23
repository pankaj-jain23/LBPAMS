﻿namespace LBPAMS.ViewModels
{
    public class AddGPPanchayatWardsViewModel
    {
        public string GPPanchayatWardsName { get; set; }
        public int GPPanchayatWardsCode { get; set; }

        public string? GPPanchayatWardsType { get; set; }

        public int ElectionTypeMasterId
        {
            get;
            set;
        }


        public int StateMasterId
        {
            get;
            set;
        }


        public int DistrictMasterId
        {
            get;
            set;
        }


        public int AssemblyMasterId
        {
            get;
            set;
        }
        public int? FourthLevelHMasterId
        {
            get;
            set;
        }


        public string? GPPanchayatWardsCategory { get; set; }

        public bool IsStatus { get; set; }
        public bool IsCC { get; set; }
        public bool IsNN { get; set; }
        public int? Male { get; set; }
        public int? Female { get; set; }
        public int? Transgender { get; set; }
        public int? TotalVoters { get; set; }

    }
    public class UpdateGPPanchayatWardsViewModel
    {
        public int GPPanchayatWardsMasterId { get; set; }

        public string GPPanchayatWardsName { get; set; }
        public int GPPanchayatWardsCode { get; set; }

        public string? GPPanchayatWardsType { get; set; }

        public int ElectionTypeMasterId
        {
            get;
            set;
        }


        public int StateMasterId
        {
            get;
            set;
        }


        public int DistrictMasterId
        {
            get;
            set;
        }


        public int AssemblyMasterId
        {
            get;
            set;
        }


        public int? FourthLevelHMasterId
        {
            get;
            set;
        }

        public string? GPPanchayatWardsCategory { get; set; }

        public bool IsStatus { get; set; }
        public bool IsCC { get; set; }
        public bool IsNN { get; set; }
        public int? Male { get; set; }
        public int? Female { get; set; }
        public int? Transgender { get; set; }
        public int? TotalVoters { get; set; }
    }

    public class ListGPPanchayatWardsViewModel
    {
        public int GPPanchayatWardsMasterId { get; set; }
        public string GPPanchayatWardsName { get; set; }
        public int GPPanchayatWardsCode { get; set; }

        public string? GPPanchayatWardsType { get; set; }

        public int ElectionTypeMasterId
        {
            get;
            set;
        }
        public string ElectionTypeName { get; set; }

        public int StateMasterId
        {
            get;
            set;
        }
        public string StateName { get; set; }

        public int DistrictMasterId
        {
            get;
            set;
        }

        public string DistrictName { get; set; }
        public int AssemblyMasterId
        {
            get;
            set;
        }
        public string AssemblyName { get; set; }
        public int? FourthLevelHMasterId
        {
            get;
            set;
        }
        public string FourthLevelHName { get; set; }

        public string? GPPanchayatWardsCategory { get; set; }

        public bool IsStatus { get; set; }
        public bool IsCC { get; set; }
        public bool IsNN { get; set; }
        public int? Male { get; set; }
        public int? Female { get; set; }
        public int? Transgender { get; set; }
        public int? TotalVoters { get; set; }
    }
    public class GPPanchListViewModel
    {
        public int GPPanchayatWardsMasterId { get; set; }
        public string GPPanchayatWardsName { get; set; }
        public int GPPanchayatWardsCode { get; set; }
    }
}
