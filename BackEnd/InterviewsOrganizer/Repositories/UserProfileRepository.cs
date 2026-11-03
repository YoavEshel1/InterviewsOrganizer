using InterviewsOrganizer.Data;
using InterviewsOrganizer.Models.Entities;
using InterviewsOrganizer.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace InterviewsOrganizer.Repositories
{
    public class UserProfileRepository : IUserProfileRepository
    {
        private readonly AppDbContext _context;

        public UserProfileRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<UserProfile?> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default)
        {
            return await _context.UserProfiles.FindAsync([userId], cancellationToken);
        }

        public async Task AddAsync(UserProfile profile, CancellationToken cancellationToken = default)
        {
            await _context.UserProfiles.AddAsync(profile, cancellationToken);
        }

        public async Task<UserProfile> CreateOrUpdateAsync(string userId, string firstName, string lastName, CancellationToken cancellationToken = default)
        {
            var existing = await _context.UserProfiles
                .FirstOrDefaultAsync(p => p.UserId == userId, cancellationToken);

            if (existing is null)
            {
                existing = new UserProfile
                {
                    UserId = userId,
                    FirstName = firstName,
                    LastName = lastName
                };
                await _context.UserProfiles.AddAsync(existing, cancellationToken);
            }
            else
            {
                existing.FirstName = firstName;
                existing.LastName = lastName;
            }

            return existing;
        }

        public async Task AnonymizeAsync(string userId, CancellationToken cancellationToken = default)
        {
            var existing = await _context.UserProfiles
                .FirstOrDefaultAsync(p => p.UserId == userId, cancellationToken);

            if (existing is null) return;

            // Overwrite PII in place before the row is removed, so the anonymization step
            // itself is visible/auditable even if the subsequent delete is ever skipped.
            existing.FirstName = "[REDACTED]";
            existing.LastName = "[REDACTED]";
        }

        public async Task DeleteByUserIdAsync(string userId, CancellationToken cancellationToken = default)
        {
            await _context.UserProfiles
                .Where(p => p.UserId == userId)
                .ExecuteDeleteAsync(cancellationToken);
        }

        public async Task Save(CancellationToken cancellationToken = default)
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
