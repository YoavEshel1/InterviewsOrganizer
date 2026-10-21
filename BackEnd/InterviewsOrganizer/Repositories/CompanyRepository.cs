using InterviewsOrganizer.Data;
using InterviewsOrganizer.Models.DTOs;
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

        public async Task<List<Company>> GetAll(CancellationToken cancellationToken = default)
        {
            return await _context.Companies.ToListAsync(cancellationToken);
        }

        public async Task<Company?> GetById(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.Companies.FindAsync([id], cancellationToken);  
        }

        public async Task DeleteByUserIdAsync(string userId, CancellationToken cancellationToken = default)
        {
            await _context.Companies
                .IgnoreQueryFilters() // ignore "Global query filter" in AppDbContext.cs for Admin users    
                .Where(c => c.UserId == userId)
                .ExecuteDeleteAsync(cancellationToken);
        }

        public async Task<Company> Update(Guid id, string name, string? companyInfo, CancellationToken cancellationToken = default)
        {
            // Global query filter in AppDbContext.cs restricts this to the current user's own company,
            // so a non-owned or non-existent Id simply won't be found.
            var existing = await _context.Companies
                .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

            if (existing is null)
            {
                throw new KeyNotFoundException($"Company with id {id} was not found.");
            }

            // Only update mutable fields; UserId (ownership) must never change.
            existing.Name = name;
            existing.CompanyInfo = companyInfo;
            return existing;
        }

        public async Task Save(CancellationToken cancellationToken = default)
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
