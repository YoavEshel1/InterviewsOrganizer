using InterviewsOrganizer.Models.Enums;

namespace InterviewsOrganizer.Models.Entities
{
    public class Position : IAuditableEntity
    {
        public Guid Id { get; set; }
        public required string Title { get; set; }
        public DateTime? ApplyDate { get; set; }
        public PositionStatus Status { get; set; }
        public string? Notes { get; set; }
        public Guid CompanyId { get; set; }
        public required string UserId { get; set; }
        public List<Interview>? Interviews { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
    }
}
