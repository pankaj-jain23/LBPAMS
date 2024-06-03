using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace EAMS_ACore
{
    public class StateMaster
    {
        [Key]
        public int StateMasterId { get; set; }

        public string StateName { get; set; }

        public string StateCode { get; set; }
        public int ElectionTypeId { get; set; }

        public DateTime? StateCreatedAt { get; set; }

        public DateTime? StateUpdatedAt { get; set; }

        public DateTime? StateDeletedAt { get; set; }
        public string? SecondLanguage { get; set; }

        public bool StateStatus { get; set; } = true;
        public bool IsGenderCapturedinVoterTurnOut { get; set; }

        [JsonIgnore]
        public virtual List<DistrictMaster> DistrictMasters { get; set; } = new List<DistrictMaster>();

        [JsonIgnore]
        public virtual List<AssemblyMaster> AssemblyMaster
        {
            get;
            set;
        }

        [JsonIgnore]
        public virtual List<ParliamentConstituencyMaster> ParliamentConstituencyMaster
        {
            get;
            set;
        }

        [JsonIgnore]
        public virtual List<BoothMaster> BoothMaster
        {
            get;
            set;
        }
    }
}
