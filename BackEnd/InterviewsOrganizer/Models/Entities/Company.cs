namespace InterviewsOrganizer.Models.Entities
{
    public class Company : IAuditableEntity
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public string? CompanyInfo { get; set; }
        public required string UserId { get; set; }
        public List<Position>? Positions { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
    }
}
