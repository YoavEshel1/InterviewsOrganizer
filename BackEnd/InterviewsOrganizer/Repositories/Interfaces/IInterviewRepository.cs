using InterviewsOrganizer.Models.Entities;

namespace InterviewsOrganizer.Repositories.Interfaces
{
    public interface IInterviewRepository
    {
        Task Add(Interview interview, Guid positionId);
        Task<List<Interview>> GetAll(Guid positionId);
        Task<Interview?> GetById(Guid id);
        Task Save();
    }
}
