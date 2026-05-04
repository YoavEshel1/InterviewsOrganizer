using InterviewsOrganizer.Models.Enums;

namespace InterviewsOrganizer.Models.DTOs
{
    public class CreatePositionDto
    {
        public required string Title { get; set; }

        public  Guid CompanyId { get; set; }
        public DateTime? ApplyDate { get; set; }
        public PositionStatus Status { get; set; }
        public string? Notes { get; set; }
    }
}