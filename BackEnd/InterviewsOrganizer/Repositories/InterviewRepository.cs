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

        public async Task Add(Interview interview)
        {
            await _context.Interviews.AddAsync(interview);
        }

        public async Task<List<Interview>> GetAll()
        {
            return await _context.Interviews.ToListAsync();
        }

        public async Task<Interview?> GetById(Guid id)
        {
            return await _context.Interviews.FindAsync(id);
        }

        public async Task Save()
        {
            await _context.SaveChangesAsync();
        }
    }
}
