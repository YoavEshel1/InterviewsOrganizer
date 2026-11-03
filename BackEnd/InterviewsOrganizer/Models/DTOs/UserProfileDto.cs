namespace InterviewsOrganizer.Models.DTOs
{
    public record UserProfileDto(string FirstName, string LastName, string? Email, DateTimeOffset? UpdatedAtDateTimeUtc);
}
