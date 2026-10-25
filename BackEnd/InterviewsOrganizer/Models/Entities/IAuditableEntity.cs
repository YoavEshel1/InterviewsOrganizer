namespace InterviewsOrganizer.Models.Entities
{
    /// <summary>
    /// Marks an entity as having automatic creation/update auditing timestamps, populated by
    /// <see cref="InterviewsOrganizer.Data.AppDbContext.SaveChangesAsync"/>.
    /// </summary>
    public interface IAuditableEntity
    {
        DateTimeOffset CreatedAt { get; set; }
        DateTimeOffset? UpdatedAt { get; set; }
    }
}
