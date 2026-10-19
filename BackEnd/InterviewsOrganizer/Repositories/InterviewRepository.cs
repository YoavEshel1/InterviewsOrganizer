using InterviewsOrganizer.Data;
using InterviewsOrganizer.Models.Entities;
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

        public async Task Save()
        {
            await _context.SaveChangesAsync();
        }
    }
}
