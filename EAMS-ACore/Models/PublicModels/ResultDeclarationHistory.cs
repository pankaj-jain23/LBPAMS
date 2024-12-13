using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAMS_ACore.Models.PublicModels
{
    public class ResultDeclarationHistory
    {
        [Key]
        public int ResultDeclarationHistoryMasterId
        {
            get;
            set;
        }
        public int ResultDeclarationMasterId
        {
            get;
            set;
        }
        [ForeignKey("ResultDeclarationMasterId")]
        public virtual ResultDeclaration ResultDeclaration { get; set; }

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
        public bool IsReCounting { get; set; }
        public bool IsDraw { get; set; }
        public bool IsDrawLottery { get; set; }
        public DateTime? ResultDecCreatedAt { get; set; }
        public DateTime? ResultDecUpdatedAt { get; set; }
        public DateTime? ResultDecDeletedAt { get; set; }

        public bool ResultDecStatus { get; set; }
       

    }
}
