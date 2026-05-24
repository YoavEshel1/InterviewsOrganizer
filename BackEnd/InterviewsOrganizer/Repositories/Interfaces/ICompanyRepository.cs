using InterviewsOrganizer.Models.Entities;

namespace InterviewsOrganizer.Repositories.Interfaces
{
    public interface ICompanyRepository
    {
        Task Add(Company company);
        Task<List<Company>> GetAll(CancellationToken cancellationToken = default);
        Task<Company?> GetById(Guid id, CancellationToken cancellationToken = default); 
        Task Save();
    }
}
