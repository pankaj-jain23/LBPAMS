using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAMS_ACore.Models.PublicModels
{
    public class GPVoter
    {
        [Key]
        public int GPVoterMasterId
        {
            get;
            set;
        }
        public int? StateMasterId
        {
            get;
            set;
        }
        public int? DistrictMasterId
        {
            get;
            set;
        }
        public int? AssemblyMasterId
        {
            get;
            set;
        }
        public int? FourthLevelHMasterId

        {
            get;
            set;
        }
        public string? GPVoterPdfPath
        {
            get;
            set;
        }
        public DateTime? GPVoterCreatedAt { get; set; }

        public DateTime? GPVoterUpdatedAt { get; set; }

        public DateTime? GPVoterDeletedAt { get; set; }
        public bool GPVoterStatus { get; set; }

    }
    public class GPVoterList
    {
        public int GPVoterMasterId
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
        public int? AssemblyMasterId
        {
            get;
            set;
        }
        public string AssemblyName
        {
            get;
            set;

        }
        public int? FourthLevelHMasterId

        {
            get;
            set;
        }
        public string FourthLevelName
        {
            get;
            set;

        }
        public string? GPVoterPdfPath
        {
            get;
            set;
        }
        public bool GPVoterStatus { get; set; }

    }
}
