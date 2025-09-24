using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAMS_ACore.Models.PublicModels
{
    public class ResultDeclarationTableConfiguration
    {
        [Key]
        public Guid RDTableConfigId { get; set; } = Guid.NewGuid();

        public int? StateMasterId { get; set; }
        public int? DistrictMasterId { get; set; }
        public int? AssemblyMasterId { get; set; }
        public int? FourthLevelHMasterId { get; set; }
        public int? ElectionTypeMasterId { get; set; }

        public int? NoOfBooth { get; set; }
        public int? NoOfRound { get; set; }
        public int? NoOfTable { get; set; }

        public bool IsFinalized { get; set; }
        public bool IsLocked { get; set; }
        public bool IsDeleted { get; set; }

        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }

        public virtual ICollection<ResultDeclarationRoundFormation> ResultDeclarationRoundFormations { get; set; }
        public virtual ICollection<ResultDeclarationRoundWise> ResultDeclarationRoundWises { get; set; }
    }
}
