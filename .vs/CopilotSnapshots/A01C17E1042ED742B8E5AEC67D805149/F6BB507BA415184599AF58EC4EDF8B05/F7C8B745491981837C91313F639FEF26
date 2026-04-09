using InterviewsOrganizer.Models.Entities;

namespace InterviewsOrganizer.Repositories.Interfaces
{
    public interface IInterviewRepository
    {
        Task Add(Interview interview);
        Task<List<Interview>> GetAll();
        Task<Interview> GetById(Guid id);
        Task Save();
    }
}
