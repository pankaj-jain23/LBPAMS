using System.ComponentModel.DataAnnotations;

namespace EAMS_ACore.NotificationModels
{
    public class SMSTemplate
    {
        [Key]
        public int SMSTemplateMasterId { get; set; }
        public string? SMSName { get; set; }
        public string? Message { get; set; }
        public string? EntityId { get; set; }
        public string? TemplateId { get; set; }
        public bool? Status { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public virtual SMSSent SMSSent { get; set; }
    }
}
