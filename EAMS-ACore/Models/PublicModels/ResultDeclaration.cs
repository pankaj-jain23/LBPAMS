using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAMS_ACore.Models.PublicModels
{
    public class ResultDeclaration
    {
        [Key]
        public int ResultDeclarationMasterId
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
        public int ElectionTypeMasterId
        {
            get;
            set;
        }
        public int AssemblyMasterId //ZP
        {
            get;
            set;
        }
        public int FourthLevelHMasterId // GPSarpanch // MCorp //MC //NP

        {
            get;
            set;
        }
        public int? BoothMasterId  
        {
            get;
            set;
        }
        public int? GPPanchayatWardsMasterId //GPpanch-Ward
        {
            get;
            set;
        }
        public int CandidateId { get; set; }
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
        public int CandidateType { get; set; }
        public bool IsWinner { get; set; }
        public DateTime? ResultDecCreatedAt { get; set; }
        public DateTime? ResultDecUpdatedAt { get; set; }
        public DateTime? ResultDecDeletedAt { get; set; }

        public bool ResultDecStatus { get; set; }
    }
    public class ResultDeclarationList
    {
        public int ResultDeclarationMasterId
        {
            get;
            set;
        }
        public int? StateMasterId
        {
            get;
            set;
        }
        public string StateName
        {
            get;
            set;

        }
        public int? DistrictMasterId
        {
            get;
            set;
        }
        public string DistrictName
        {
            get;
            set;

        }
        public int? ElectionTypeMasterId
        {
            get;
            set;
        }
        public string ElectionType
        { 
            get; 
            set;
        }

        public int? AssemblyMasterId //ZP
        {
            get;
            set;
        }
        public string AssemblyName
        {
            get;
            set;

        }
        public int? FourthLevelHMasterId // GPSarpanch // MCorp //MC //NP

        {
            get;
            set;
        }
        public string FourthLevelName
        {
            get;
            set;

        }
        public int? GPPanchayatWardsMasterId //GPpanch-Ward
        {
            get;
            set;
        }
        public string GPPanchayatWardsName
        {
            get;
            set;

        }
        public int CandidateId { get; set; }
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
        public int CandidateType { get; set; }
        public bool IsWinner { get; set; }
        public bool ResultDecStatus { get; set; }
    }
}
