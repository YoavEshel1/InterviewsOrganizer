using InterviewsOrganizer.Models.Entities;

namespace InterviewsOrganizer.Models.DTOs
{
    public class CreateCompanyDto
    {
        public required string Name { get; set; }
        public string? CompanyInfo { get; set; }
    }
}
