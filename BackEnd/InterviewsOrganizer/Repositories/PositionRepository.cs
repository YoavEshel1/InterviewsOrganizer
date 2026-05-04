using InterviewsOrganizer.Data;
using InterviewsOrganizer.Models.Entities;
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

        public async Task Save()
        {
            await _context.SaveChangesAsync();
        }
    }
}