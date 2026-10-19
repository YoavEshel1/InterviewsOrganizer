namespace InterviewsOrganizer.Models.Entities
{
    public class AuditLog
    {
        public Guid Id { get; set; }
        public required string AdminUserId { get; set; }
        public required string TargetUserId { get; set; }
        public required string Action { get; set; }
        public required string IpAddress { get; set; }
        public DateTime TimestampUtc { get; set; }
    }
}
