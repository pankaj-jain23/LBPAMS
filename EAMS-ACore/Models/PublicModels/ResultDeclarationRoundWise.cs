using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAMS_ACore.Models.PublicModels
{
    public class ResultDeclarationRoundWise
    {

        [Key]
        public Guid RDRoundWiseId { get; set; } = Guid.NewGuid();
        public Guid? RDTableConfigId { get; set; }
        public Guid? RDRoundFormationId { get; set; }
        public int KycMasterId { get; set; } // Candidate Ids
        public int? TotalVotes { get; set; } 
        public int? TotalCommulativeVotes { get; set; } 
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }

        [ForeignKey("RDTableConfigId")]
        public virtual ResultDeclarationTableConfiguration ResultDeclarationTableConfigurations { get; set; }

        [ForeignKey("RDRoundFormationId")]
        public virtual ResultDeclarationRoundFormation ResultDeclarationRoundFormations { get; set; }

    }
}
