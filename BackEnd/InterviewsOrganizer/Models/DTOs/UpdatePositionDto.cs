using InterviewsOrganizer.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace InterviewsOrganizer.Models.DTOs
{
    public sealed record UpdatePositionDto(
        [Required, StringLength(200, MinimumLength = 1)] string Title,
        DateTime? ApplyDate,
        PositionStatus Status,
        [StringLength(2000)] string? Notes
    );
}
