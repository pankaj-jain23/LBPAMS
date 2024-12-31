using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAMS_ACore.Models
{
    public class CompletedVTList
    {
        public string Header 
        { 
            get;
            set;
        }
        public string Title 
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
        public int TotalWards 
        {
            get;
            set;
        }
        public int EnteredWards
        {
            get;
            set;
        }
        public string? Type 
        {
            get;
            set; 
        }
    }
}
