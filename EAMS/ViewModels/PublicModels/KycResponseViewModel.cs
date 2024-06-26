namespace EAMS.ViewModels.PublicModels
{
    public class KycResponseViewModel
    {
        public int KycMasterId { get; set; }
        public int StateMasterId { get; set; }
        public int? DistrictMasterId { get; set; }
        public int? ElectionType { get; set; }
        public int? BlockMasterId { get; set; }
        public int? ZPMasterId { get; set; }
        public int? PSMasterId { get; set; }
        public int? MCorporationMasterId { get; set; }
        public int? MCouncilMasterId { get; set; }
        public int? NagaraPanchayatMasterId { get; set; }
        public string CandidateName { get; set; }
        public string FatherName { get; set; }
        public string NominationPdfPath { get; set; }
        public string? Option1 { get; set; }
        public string? Option2 { get; set; }
    }

}
