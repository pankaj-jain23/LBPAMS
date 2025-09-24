using EAMS_ACore.AuthModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAMS_ACore.Models.PublicModels
{
    public class ResultDeclarationRoundFormation
    {
        [Key]
        public Guid RDRoundFormationId { get; set; } = Guid.NewGuid();

       
        public Guid RDTableConfigId { get; set; }

        public string? RoundName { get; set; }
        public int? RoundNumber { get; set; }

        public bool IsFinalized { get; set; }
        public bool IsLocked { get; set; }
        public bool IsDeleted { get; set; }

        public DateTime? CreatedAt { get; set; }
        public string? CreatedBy { get; set; }

        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? DeletedAt { get; set; }
        public string? DeletedBy { get; set; }


        [ForeignKey("RDTableConfigId")]
        public virtual ResultDeclarationTableConfiguration ResultDeclarationTableConfigurations { get; set; }
        public virtual ICollection<ResultDeclarationRoundWise> ResultDeclarationRoundWises { get; set; }

    }
}
