using InterviewsOrganizer.Models.DTOs;
using InterviewsOrganizer.Models.Entities;

namespace InterviewsOrganizer.Services.Interfaces
{
    public interface IInterviewService  
    {
        Task<List<Interview>> GetAllAsync(Guid positionId);
        Task<Interview?> GetByIdAsync(Guid id);
        Task CreateAsync(Interview interview, Guid positionId);
        Task DeleteByUserIdAsync(string userId, CancellationToken cancellationToken = default);
        Task<UpdateInterviewDto> UpdateAsync(Guid id, Guid positionId, UpdateInterviewDto dto, CancellationToken cancellationToken = default);
        Task DeleteAsync(Guid id, Guid positionId, CancellationToken cancellationToken = default);
    }
}
