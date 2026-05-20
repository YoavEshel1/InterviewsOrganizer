using InterviewsOrganizer.Models.Entities;

namespace InterviewsOrganizer.Services.Interfaces
{
    public interface ICompanyService
    {
        Task<List<Company>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<Company?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task CreateAsync(Company company);
    }
}