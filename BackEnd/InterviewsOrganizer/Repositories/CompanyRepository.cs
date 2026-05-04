using InterviewsOrganizer.Data;
using InterviewsOrganizer.Models.Entities;
using InterviewsOrganizer.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace InterviewsOrganizer.Repositories
{
    public class CompanyRepository : ICompanyRepository
    {
        private readonly AppDbContext _context;

        public CompanyRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task Add(Company company)
        {
            await _context.Companies.AddAsync(company);
        }

        public async Task<List<Company>> GetAll()
        {
            return await _context.Companies.ToListAsync();
        }

        public async Task<Company?> GetById(Guid id)
        {
            return await _context.Companies.FindAsync(id);
        }

        public async Task Save()
        {
            await _context.SaveChangesAsync();
        }
    }
}
