using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace EAMS_ACore.SMSModels
{
    public class SMSNumbers
    {
        [Key]
        public int Id { get; set; }
        public int StateMasterId { get; set; }
        public int? DistrictMasterId { get; set; }
        public string? Name { get; set; }
        public string? Number { get; set; }
        public DateTime? SendTime { get; set; }
        public string? Response { get; set; }
        public int? SendCount { get; set; }
    }
    public class SMSConfiguration
    {
        [Key] 
        public int Id { get; set; }
        public int StateMasterId { get; set; }
        public int? DistrictMasterId { get; set; }  
        public string? UserName { get; set; }
        public string? Password { get; set; }
        public string? SenderId { get; set; }
        public string? TemplateId { get; set; }
        public string? EntityId { get; set; } 
        public string? Message { get; set; }
    }
}
