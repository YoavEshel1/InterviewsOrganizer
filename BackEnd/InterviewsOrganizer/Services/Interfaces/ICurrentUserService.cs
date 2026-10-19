namespace InterviewsOrganizer.Services.Interfaces
{
    public interface ICurrentUserService
    {
        string? UserId { get; }
        bool IsImpersonated { get; }
        string? OriginalAdminId { get; }
    }
}
