﻿namespace EAMS_ACore.NotificationModels
{
    public class SMSTemplateModel
    {
        public int? SMSTemplateMasterId { get; set; }
        public string? SMSName { get; set; }
        public string? Message { get; set; }
        public string? EntityId { get; set; }
        public string? TemplateId { get; set; }
        public bool? IsStatus { get; set; }

        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
