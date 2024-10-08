﻿using System.ComponentModel.DataAnnotations;

namespace EAMS_ACore.Models.PublicModels
{
    public class UnOpposed
    {
        [Key]
        public int? UnOpposedMasterId
        {
            get;
            set;
        }
        public int StateMasterId
        {
            get;
            set;
        }
        public int? DistrictMasterId
        {
            get;
            set;
        }
        public int? ElectionTypeMasterId
        {
            get;
            set;
        }
        public int? AssemblyMasterId
        {
            get;
            set;
        }
        public int FourthLevelHMasterId

        {
            get;
            set;
        }

        public int PSZonePanchayatMasterId
        {
            get;
            set;
        }
        public int GPPanchayatWardsMasterId
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
        public string NominationPdfPath
        {
            get;
            set;
        }
        public string? Option1
        {

            get; set;
        }
        public string? Option2
        {

            get; set;
        }
    }
    public class UnOpposedList
    {

        public int? UnOpposedMasterId
        {
            get;
            set;
        }
        public int? StateMasterId
        {
            get;
            set;
        }
        public string? StateName
        {
            get;
            set;

        }
        public int? DistrictMasterId
        {
            get;
            set;
        }
        public string? DistrictName
        {
            get;
            set;

        }
        public int? ElectionTypeMasterId
        {
            get;
            set;
        }
        public string? ElectionTypeName
        {
            get;
            set;
        }
        public int? AssemblyMasterId
        {
            get;
            set;
        }
        public string? AssemblyName
        {
            get;
            set;

        }
        public int? FourthLevelHMasterId

        {
            get;
            set;
        }
        public string? FourthLevelHName
        {
            get;
            set;

        }
        public int? PSZonePanchayatMasterId
        {
            get;
            set;
        }

        public string? PSZonePanchayatName
        {
            get;
            set;

        }
        public int? GPPanchayatWardsMasterId
        {
            get;
            set;
        }
        public string? GPPanchayatWardsName
        {
            get;
            set;

        }
        public string CandidateType
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
        public string NominationPdfPath
        {
            get;
            set;
        }
        public string? Option1
        {

            get; set;
        }
        public string? Option2
        {

            get; set;
        }

    }
}
