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
        public bool IsCC { get; set; }//Court Case
        public bool IsNN { get; set; }//No nomination
        public bool IsWinner { get; set; }
        public bool IsResultDeclared { get; set; }
        public bool IsDraw { get; set; }//For Draw Candidates
        public bool IsDrawLottery { get; set; }//Choosen  of candidate by blindfolded  box
        public bool IsReCounting { get; set; } // ReCounting in case of draw candidates
        public int? VoteMargin
        {
            get;
            set;
        }
        public string? PartyName
        {
            get;
            set;
        }
        public string Message
        {
            get;
            set;
        }
    }
}
