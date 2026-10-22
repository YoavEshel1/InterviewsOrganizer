using InterviewsOrganizer.Models.DTOs;
using InterviewsOrganizer.Models.Entities;

namespace InterviewsOrganizer.Services.Interfaces
{
    public interface ICompanyService
    {
        Task<List<Company>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<Company?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task CreateAsync(Company company);
        Task DeleteByUserIdAsync(string userId, CancellationToken cancellationToken = default);
        Task<UpdateCompanyDto> UpdateAsync(UpdateCompanyDto dto, CancellationToken cancellationToken = default);
        Task DeleteAsync(Guid id, CancellationToken cancellationToken);
    }
}