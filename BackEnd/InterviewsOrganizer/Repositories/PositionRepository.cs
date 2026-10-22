using InterviewsOrganizer.Data;
using InterviewsOrganizer.Models.Entities;
using InterviewsOrganizer.Models.Enums;
using InterviewsOrganizer.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace InterviewsOrganizer.Repositories
{
    public class PositionRepository : IPositionRepository
    {
        private readonly AppDbContext _context;

        public PositionRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task Add(Position position, Guid companyId)
        {
            position.CompanyId = companyId;
            await _context.Positions.AddAsync(position);
        }

        public async Task<List<Position>> GetAll(Guid companyId)
        {
            return await _context.Positions
                .Where(p => p.CompanyId == companyId)
                .ToListAsync();
        }

        public async Task<Position?> GetById(Guid id)
        {
            return await _context.Positions.FindAsync(id);
        }

        public async Task DeleteByUserIdAsync(string userId, CancellationToken cancellationToken = default)
        {
            await _context.Positions
                .IgnoreQueryFilters()  // ignore "Global query filter" in AppDbContext.cs for Admin users
                .Where(p => p.UserId == userId)
                .ExecuteDeleteAsync(cancellationToken);
        }

        public async Task<Position> Update(Guid id, Guid companyId, string title, DateTime? applyDate, PositionStatus status, string? notes, CancellationToken cancellationToken = default)
        {
            // Global query filter in AppDbContext.cs restricts this to the current user's own positions.
            // Filtering by CompanyId too guards against a mismatched route companyId.
            var existing = await _context.Positions
                .FirstOrDefaultAsync(p => p.Id == id && p.CompanyId == companyId, cancellationToken);

            if (existing is null)
            {
                throw new KeyNotFoundException($"Position with id {id} was not found.");
            }

            // Only update mutable fields; UserId/CompanyId (ownership) must never change.
            existing.Title = title;
            existing.ApplyDate = applyDate;
            existing.Status = status;
            existing.Notes = notes;
            return existing;
        }

        public async Task Save(CancellationToken cancellationToken = default)
        {
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteAsync(Guid id, Guid companyId, CancellationToken cancellationToken = default)
        {
            // Global query filter in AppDbContext.cs restricts this to the current user's own positions.
            // Filtering by CompanyId too guards against a mismatched route companyId.
            var existing = await _context.Positions
                .FirstOrDefaultAsync(p => p.Id == id && p.CompanyId == companyId, cancellationToken);

            if (existing is null)
            {
                throw new KeyNotFoundException($"Position with id {id} was not found.");
            }

            _context.Positions.Remove(existing);
        }
    }
}