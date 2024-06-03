namespace EAMS.ViewModels.ReportViewModel
{
    public class SendSMSViewModel
    {
        public int? EventId { get; set; }
        public int? TemplateMasterId { get; set; }
        public string? Type { get; set; }
        public int? StateMasterId { get; set; }
    }
}
