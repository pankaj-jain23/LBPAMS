namespace LBPAMS.ViewModels
{
    public class PSZonePanchayatViewModel
    {

        public string PSZonePanchayatName { get; set; }
        public int PSZonePanchayatCode { get; set; }

        public string? PSZonePanchayatType { get; set; }

        public int ElectionTypeMasterId
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


        public int AssemblyMasterId
        {
            get;
            set;
        }

        public int FourthLevelHMasterId
        {
            get;
            set;
        }


        public int PSZonePanchayatBooths { get; set; }
        public string? PSZonePanchayatCategory { get; set; }
        public DateTime? PSZonePanchayatCreatedAt { get; set; }

        public DateTime? PSZonePanchayatUpdatedAt { get; set; }

        public DateTime? PSZonePanchayatDeletedAt { get; set; }

        public bool IsStatus { get; set; }


    }
    public class UpdatePSZonePanchayatViewModel
    {
        public int PSZonePanchayatMasterId { get; set; }
        public string PSZonePanchayatName { get; set; }
        public int PSZonePanchayatCode { get; set; }

        public string? PSZonePanchayatType { get; set; }

        public int ElectionTypeMasterId
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


        public int AssemblyMasterId
        {
            get;
            set;
        }

        public int FourthLevelHMasterId
        {
            get;
            set;
        }


        public int PSZonePanchayatBooths { get; set; }
        public string? PSZonePanchayatCategory { get; set; }

        public bool IsStatus { get; set; }

    }

    public class ListPSZonePanchayatViewModel
    {
        public int PSZonePanchayatMasterId { get; set; }
        public string PSZonePanchayatName { get; set; }
        public int PSZonePanchayatCode { get; set; }

        public string? PSZonePanchayatType { get; set; }

        public int ElectionTypeMasterId
        {
            get;
            set;
        }
        public string ElectionTypeName { get; set; }

        public int StateMasterId
        {
            get;
            set;
        }
        public string StateName { get; set; }

        public int DistrictMasterId
        {
            get;
            set;
        }

        public string DistrictName { get; set; }
        public int AssemblyMasterId
        {
            get;
            set;
        }
        public string AssemblyName { get; set; }

        public int FourthLevelHMasterId
        {
            get;
            set;
        }
        public string FourthLevelHName { get; set; }

        public int PSZonePanchayatBooths { get; set; }
        public string? PSZonePanchayatCategory { get; set; }

        public bool IsStatus { get; set; }
    }
}
