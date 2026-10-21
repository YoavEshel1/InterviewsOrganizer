using InterviewsOrganizer.Models.DTOs;
using InterviewsOrganizer.Models.Entities;

namespace InterviewsOrganizer.Services.Interfaces
{
    public interface IPositionService
    {
        Task<List<Position>> GetAllAsync(Guid companyId);
        Task<Position?> GetByIdAsync(Guid id);
        Task CreateAsync(Position position, Guid companyId);
        Task DeleteByUserIdAsync(string userId, CancellationToken cancellationToken = default);
        Task<UpdatePositionDto> UpdateAsync(Guid id, Guid companyId, UpdatePositionDto dto, CancellationToken cancellationToken = default);
    }
}