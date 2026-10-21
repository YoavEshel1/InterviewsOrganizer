using InterviewsOrganizer.Models.Entities;
using System.ComponentModel.DataAnnotations;

namespace InterviewsOrganizer.Models.DTOs
{
    public sealed record UpdateCompanyDto(
        Guid Id,
        [Required, StringLength(200, MinimumLength = 1)] string Name,
        [StringLength(2000)] string? CompanyInfo
    );
}
