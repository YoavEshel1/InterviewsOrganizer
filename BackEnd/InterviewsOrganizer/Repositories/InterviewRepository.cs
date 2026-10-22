using InterviewsOrganizer.Data;
using InterviewsOrganizer.Models.Entities;
using InterviewsOrganizer.Models.Enums;
using InterviewsOrganizer.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace InterviewsOrganizer.Repositories
{
    public class InterviewRepository : IInterviewRepository
    {
        private readonly AppDbContext _context;

        public InterviewRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task Add(Interview interview, Guid positionId)
        {
            interview.PositionId = positionId;
            await _context.Interviews.AddAsync(interview);
        }

        public async Task<List<Interview>> GetAll(Guid positionId)
        {
            return await _context.Interviews
                .Where(i => i.PositionId == positionId)
                .ToListAsync();
        }

        public async Task<Interview?> GetById(Guid id)
        {
            return await _context.Interviews.FindAsync(id);
        }

        public async Task DeleteByUserIdAsync(string userId, CancellationToken cancellationToken = default)
        {
            await _context.Interviews
                .IgnoreQueryFilters() // ignore "Global query filter" in AppDbContext.cs for Admin users
                .Where(i => i.UserId == userId)
                .ExecuteDeleteAsync(cancellationToken);
        }

        public async Task<Interview> Update(Guid id, Guid positionId, DateTime date, InterviewFeeling feeling, string? interviewer, string? notes, CancellationToken cancellationToken = default)
        {
            // Global query filter in AppDbContext.cs restricts this to the current user's own interviews.
            // Filtering by PositionId too guards against a mismatched route positionId.
            var existing = await _context.Interviews
                .FirstOrDefaultAsync(i => i.Id == id && i.PositionId == positionId, cancellationToken);

            if (existing is null)
            {
                throw new KeyNotFoundException($"Interview with id {id} was not found.");
            }

            // Only update mutable fields; UserId/PositionId (ownership) must never change.
            existing.Date = date;
            existing.Feeling = feeling;
            existing.Interviewer = interviewer;
            existing.Notes = notes;
            return existing;
        }

        public async Task Save(CancellationToken cancellationToken = default)
        {
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteAsync(Guid id, Guid positionId, CancellationToken cancellationToken = default)
        {
            // Global query filter in AppDbContext.cs restricts this to the current user's own interviews.
            // Filtering by PositionId too guards against a mismatched route positionId.
            var existing = await _context.Interviews
                .FirstOrDefaultAsync(i => i.Id == id && i.PositionId == positionId, cancellationToken);

            if (existing is null)
            {
                throw new KeyNotFoundException($"Interview with id {id} was not found.");
            }

            _context.Interviews.Remove(existing);
        }
    }
}
