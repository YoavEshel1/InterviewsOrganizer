using InterviewsOrganizer.Data;
using InterviewsOrganizer.Models.Entities;
using InterviewsOrganizer.Services.Interfaces;

namespace InterviewsOrganizer.Services
{
    public class AuditLogService : IAuditLogService
    {
        private readonly AppDbContext _context;

        public AuditLogService(AppDbContext context)
        {
            _context = context;
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

            await _context.AuditLogs.AddAsync(auditLog, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
