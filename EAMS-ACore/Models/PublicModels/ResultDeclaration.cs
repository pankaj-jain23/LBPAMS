using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
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
        public int KycMasterId { get; set; }
      
        public int? VoteMargin
        {
            get;
            set;
        }
        public bool IsWinner { get; set; }
        public bool IsResultDeclared { get; set; }
        public string? ResultDeclaredByMobile
        {
            get;
            set;
        }
        public string? ResultDeclaredByPortal
        {
            get;
            set;
        }
        public bool IsDraw { get; set; }//For Draw Candidates
        public bool IsDrawLottery { get; set; }//Choosen  of candidate by blindfolded  box
        public bool IsReCounting { get; set; } // ReCounting in case of draw candidates
        public DateTime? ResultDecCreatedAt { get; set; }
        public DateTime? ResultDecUpdatedAt { get; set; }
        public DateTime? ResultDecDeletedAt { get; set; }

        public bool ResultDecStatus { get; set; }
        public bool IsNOTA
        {
            get;
            set;
        }
        public virtual List<ResultDeclarationHistory> ResultDeclarationHistory
        {
            get;
            set;
        }

    }

    public class ResultDeclarationBoothWardList
    {
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
        public string ElectionTypeName
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
        public string FourthLevelHName
        {
            get;
            set;

        }
        public int? GPPanchayatWardsMasterId //GPpanch-Ward
        {
            get;
            set;
        }
        public string? GPPanchayatWardsName
        {
            get;
            set;

        }
        public int? BoothMasterId //GPpanch-Ward
        {
            get;
            set;
        }
        public string? BoothName
        {
            get;
            set;

        }
        public List<ResultCandidate> ResultCandidates{ get; set; }
    }
    public class ResultCandidate
    {
        public int ResultDeclarationMasterId
        {
            get;
            set;
        }
        public int KycMasterId { get; set; }
        public bool IsUnOpposed {  get; set; }
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
        public int? VoteMargin
        {
            get;
            set;
        }
        public bool IsWinner { get; set; }
        public bool IsResultDeclared { get; set; }
        public string? ResultDeclaredByMobile
        {
            get;
            set;
        }
        public string? ResultDeclaredByPortal
        {
            get;
            set;
        }
        public bool IsDraw { get; set; }//For Draw Candidates
        public bool IsDrawLottery { get; set; }//Choosen  of candidate by blindfolded  box
        public bool IsReCounting { get; set; } // ReCounting in case of draw candidates

        public string? PartyName
        {
            get;
            set;
        }
        public bool ResultDecStatus { get; set; }
        public bool IsNOTA
        {
            get;
            set;
        }
        public int ResultDeclarationHistoryMasterId
        {
            get;
            set;
        }
    }

    public class BoothResultList
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

        public int? AssemblyMasterId //ZP
        {
            get;
            set;
        } 
        public int? FourthLevelHMasterId // GPSarpanch // MCorp //MC //NP

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
        public int? BoothMasterId //GPpanch-Ward
        {
            get;
            set;
        }
        public string BoothName
        {
            get;
            set;

        }
         
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
        public int? BoothMasterId //GPpanch-Ward
        {
            get;
            set;
        }
        public string BoothName
        {
            get;
            set;

        }
        public int KycMasterId { get; set; }
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
        public int? VoteMargin
        {
            get;
            set;
        }
        public bool IsWinner { get; set; }
        public bool IsResultDeclared { get; set; }
        public string? ResultDeclaredByMobile
        {
            get;
            set;
        }
        public string? ResultDeclaredByPortal
        {
            get;
            set;
        }
        public bool IsDraw { get; set; }//For Draw Candidates
        public bool IsDrawLottery { get; set; }//Choosen  of candidate by blindfolded  box
        public bool IsReCounting { get; set; } // ReCounting in case of draw candidates
  

        public bool ResultDecStatus { get; set; }
    }
}
