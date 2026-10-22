using InterviewsOrganizer.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace InterviewsOrganizer.Models.DTOs
{
    public sealed record UpdateInterviewDto(
        DateTime Date,
        InterviewFeeling Feeling,
        [StringLength(200)] string? Interviewer,
        [StringLength(2000)] string? Notes
    );
}
