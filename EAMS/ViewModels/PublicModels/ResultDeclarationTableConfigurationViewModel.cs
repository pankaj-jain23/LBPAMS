namespace LBPAMS.ViewModels.PublicModels
{
    public class AddResultDeclarationTableConfigurationViewModel
    {
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
    }

    public class UpdateResultDeclarationTableConfigurationViewModel
    {
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
    }
}
