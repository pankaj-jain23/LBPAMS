namespace LBPAMS.ViewModels.PublicModels
{

    public class ResultDeclarationViewModel
    {
        public  List<ResultDeclarationListViewModel> resultDeclarationLists { get; set; }
    }
    public class ResultDeclarationListViewModel
    {
        public int KycMasterId { get; set; } 

        public int? BoothMasterId { get; set; }
        public int FourthLevelHMasterId // GPSarpanch // MCorp //MC //NP

        {
            get;
            set;
        }
        public int? GPPanchayatWardsMasterId { get; set; } 
        public int? VoteMargin
        {
            get;
            set;
        }
        public bool IsReCounting { get; set; }
        public bool IsWinner { get; set; }
        public bool IsDraw { get; set; }
        public bool IsDrawLottery { get; set; }
        public bool IsNOTA
        {
            get;
            set;
        }
    }
    public class UpdateResultDeclarationViewModel
    {
        public List<UpdateResultDeclarationListViewModel> updateResultDeclarationLists { get; set; }
    }
    public class UpdateResultDeclarationListViewModel
    {
        public int ResultDeclarationMasterId { get; set; } // Existing record identifier
        public int KycMasterId { get; set; }
        public int? BoothMasterId { get; set; }
        public int FourthLevelHMasterId { get; set; }
        public int? GPPanchayatWardsMasterId { get; set; }
        public int? VoteMargin { get; set; } // Optional, since it could be null
        public bool IsReCounting { get; set; }
        public bool IsWinner { get; set; }
        public bool IsDraw { get; set; }
        public bool IsDrawLottery { get; set; }
        public bool IsNOTA
        {
            get;
            set;
        }

    }
    public class ResultDeclarationReportListViewModel
    {
        
        public int StateMasterId
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
        
    }
}
