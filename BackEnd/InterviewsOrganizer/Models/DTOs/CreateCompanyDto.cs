using InterviewsOrganizer.Models.Entities;
using System.ComponentModel.DataAnnotations;

namespace InterviewsOrganizer.Models.DTOs
{
    public class CreateCompanyDto
    {
        [Required, StringLength(200, MinimumLength = 1)]
        public required string Name { get; set; }

        [StringLength(2000)]
        public string? CompanyInfo { get; set; }
    }
}
