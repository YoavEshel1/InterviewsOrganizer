using InterviewsOrganizer.Models.Entities;

namespace InterviewsOrganizer.Services.Interfaces
{
    public interface IInterviewService  
    {
        Task<List<Interview>> GetAllAsync(Guid positionId);
        Task<Interview?> GetByIdAsync(Guid id);
        Task CreateAsync(Interview interview, Guid positionId);
        Task<bool> DeleteAsync(Guid id);
    }
}
