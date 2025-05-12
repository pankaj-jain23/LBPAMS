using System.ComponentModel.DataAnnotations;

namespace LBPAMS.ResponseDTOs
{
    public class SMSConfigurationResponse
    {

        public int Id { get; set; }
        public int StateMasterId { get; set; }
        public string? StateName { get; set; }
        public string? DistrictName { get; set; }
        public int? DistrictMasterId { get; set; }
        public string? UserName { get; set; }
        public string? Password { get; set; }
        public string? SenderId { get; set; }
        public string? TemplateId { get; set; }
        public string? EntityId { get; set; }
        public string? Message { get; set; }

    }
}
