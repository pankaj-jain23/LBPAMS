﻿using System.ComponentModel.DataAnnotations.Schema;

namespace LBPAMS.ViewModels.PublicModels
{
    public class SarpanchResultViewModel
    {
        public int StateMasterId
        {
            get;
            set;
        }
        [ForeignKey("StateMasterId")]

        public int? DistrictMasterId
        {
            get;
            set;
        }
        [ForeignKey("DistrictMasterId")]

        public int? ElectionTypeMasterId
        {
            get;
            set;
        }
        [ForeignKey("ElectionTypeMasterId")]
        public int? AssemblyMasterId
        {
            get;
            set;
        }
        [ForeignKey("AssemblyMasterId")]
        public int? PsZoneMasterId
        {
            get;
            set;
        }

        public int? BoothMasterId
        {
            get;
            set;
        }
        public string? BoothCode
        {
            get;
            set;
        }
        public int? SarpanchWardsMasterId
        {
            get;
            set;
        }
        public string CandidateName
        {
            get;
            set;
        }
        public string FatherName
        {
            get;
            set;
        }
        public string? VoteMargin
        {
            get;
            set;
        }
        public bool IsSarpanch { get; set; }
        public DateTime? ResultDecCreatedAt { get; set; }
        //public DateTime? ResultDecUpdatedAt { get; set; }

        public bool ResultDecStatus { get; set; }
    }
}
