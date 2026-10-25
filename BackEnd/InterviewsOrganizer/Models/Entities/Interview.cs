using InterviewsOrganizer.Models.Enums;

namespace InterviewsOrganizer.Models.Entities
{
    public class Interview : IAuditableEntity
    {
        public Guid Id { get; set; }
        public DateTime Date { get; set; }
        public InterviewFeeling Feeling { get; set; }
        public Guid PositionId { get; set; }
        public string? Interviewer { get; set; }
        public string? Notes { get; set; }
        public required string UserId { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
    }
}
