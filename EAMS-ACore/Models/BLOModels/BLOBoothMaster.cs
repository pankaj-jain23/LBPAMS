using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EAMS_ACore.Models.BLOModels
{
    public class BLOBoothMaster
    {
        [Key]
        public int BLOBoothMasterId { get; set; }
        public int BoothMasterId { get; set; }
        public string PSBuildingName { get; set; }
        //public string BLOBoothName { get; set; }
        //public string BLOBoothCode { get; set; }
        //public bool BLOStatus { get; set; }
        public int BLOMasterId
        {
            get;
            set;
        }
        [ForeignKey("BLOMasterId")]
        public virtual BLOMaster BLOMaster
        {
            get;
            set;
        }
    }

}
