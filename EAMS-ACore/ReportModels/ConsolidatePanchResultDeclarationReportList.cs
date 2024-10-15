using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAMS_ACore.ReportModels
{
    public class ConsolidatePanchResultDeclarationReportList
    {
        public string Header { get; set; }
        public string Title { get; set; }
        public string Type { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }

        public string StateName
        {
            get;
            set;
        }

        public string DistrictName
        {
            get;
            set;
        }

        public string ElectionTypeName
        {
            get;
            set;
        }

        public string AssemblyName
        {
            get;
            set;
        }

        public string FourthLevelHName
        {
            get;
            set;
        }

        public string GPPanchayatWardsName
        {
            get;
            set;
        }
        public string GPPanchayatWardsType { get; set; }

        public int? ResultDeclarationMasterId
        {
            get;
            set;
        }
        public int? KycMasterId
        {
            get;
            set;
        }
        public string CandidateName
        {
            get;
            set;
        }
        public string VotesGained
        {
            get;
            set;
        }
        public string VotesGainedPercentage
        {
            get;
            set;
        }
    }
}
