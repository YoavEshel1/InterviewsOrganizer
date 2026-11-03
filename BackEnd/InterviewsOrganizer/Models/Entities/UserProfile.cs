namespace InterviewsOrganizer.Models.Entities
{
    /// <summary>
    /// GDPR-sensitive PII extension for a user account. 1:1 with <see cref="AppUser"/>;
    /// UserId is both the primary key and the foreign key to AspNetUsers.
    /// </summary>
    public class UserProfile : IAuditableEntity
    {
        public required string UserId { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public AppUser? User { get; set; }

        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
    }
}
