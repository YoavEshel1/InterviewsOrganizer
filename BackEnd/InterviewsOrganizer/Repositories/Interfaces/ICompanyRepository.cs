using InterviewsOrganizer.Models.Entities;

namespace InterviewsOrganizer.Repositories.Interfaces
{
    public interface ICompanyRepository
    {
        Task Add(Company company);
        Task<List<Company>> GetAll();
        Task<Company> GetById(Guid id);
        Task Save();
    }
}
