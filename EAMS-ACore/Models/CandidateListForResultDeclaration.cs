using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAMS_ACore.Models
{
    public class CandidateListForResultDeclaration
    {
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
        public string FatherName
        {
            get;
            set;
        }
        public bool IsUnOppossed
        {
            get;
            set;
        }
    }
}
