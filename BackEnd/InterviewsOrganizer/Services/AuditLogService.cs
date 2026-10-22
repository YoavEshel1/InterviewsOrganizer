using InterviewsOrganizer.Data;
using InterviewsOrganizer.Models.Entities;
using InterviewsOrganizer.Services.Interfaces;

namespace InterviewsOrganizer.Services
{
    public class AuditLogService : IAuditLogService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<AuditLogService> _logger;

        public AuditLogService(AppDbContext context, ILogger<AuditLogService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task LogAsync(string adminUserId, string targetUserId, string action, string ipAddress, CancellationToken cancellationToken = default)
        {
            var auditLog = new AuditLog
            {
                Id = Guid.NewGuid(),
                AdminUserId = adminUserId,
                TargetUserId = targetUserId,
                Action = action,
                IpAddress = ipAddress,
                TimestampUtc = DateTime.UtcNow
            };

            try
            {
                await _context.AuditLogs.AddAsync(auditLog, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);

                _logger.LogInformation(
                    "Audit log {AuditLogId} recorded: {Action} by admin {AdminUserId} on target user {TargetUserId} from {IpAddress}",
                    auditLog.Id, action, adminUserId, targetUserId, ipAddress);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Failed to persist audit log for action {Action} by admin {AdminUserId} on target user {TargetUserId}",
                    action, adminUserId, targetUserId);
                throw;
            }
        }
    }
}
