using InterviewsOrganizer.Models.Entities;
using InterviewsOrganizer.Models.Enums;

namespace InterviewsOrganizer.Repositories.Interfaces
{
    public interface IInterviewRepository
    {
        Task Add(Interview interview, Guid positionId);
        Task<List<Interview>> GetAll(Guid positionId);
        Task<Interview?> GetById(Guid id);
        Task DeleteByUserIdAsync(string userId, CancellationToken cancellationToken = default);
        Task<Interview> Update(Guid id, Guid positionId, DateTime date, InterviewFeeling feeling, string? interviewer, string? notes, CancellationToken cancellationToken = default);
        Task Save(CancellationToken cancellationToken = default);
        Task DeleteAsync(Guid id, Guid positionId, CancellationToken cancellationToken = default);
    }
}
