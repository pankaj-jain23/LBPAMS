using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAMS_ACore.Models
{
    public class CandidateListForResultDeclaration
    {
        public int? CandidateId
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
        public string CandidateType
        {
            get;
            set;
        }
    }
}
