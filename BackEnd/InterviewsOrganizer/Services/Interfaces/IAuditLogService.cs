namespace InterviewsOrganizer.Services.Interfaces
{
    public interface IAuditLogService
    {
        Task LogAsync(string adminUserId, string targetUserId, string action, string ipAddress, CancellationToken cancellationToken = default);
    }
}
