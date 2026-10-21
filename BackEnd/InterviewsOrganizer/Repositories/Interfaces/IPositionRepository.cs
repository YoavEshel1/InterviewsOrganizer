using InterviewsOrganizer.Models.Entities;
using InterviewsOrganizer.Models.Enums;

namespace InterviewsOrganizer.Repositories.Interfaces
{
    public interface IPositionRepository
    {
        Task Add(Position position, Guid companyId);
        Task<List<Position>> GetAll(Guid companyId);
        Task<Position?> GetById(Guid id);
        Task DeleteByUserIdAsync(string userId, CancellationToken cancellationToken = default);
        Task<Position> Update(Guid id, Guid companyId, string title, DateTime? applyDate, PositionStatus status, string? notes, CancellationToken cancellationToken = default);
        Task Save(CancellationToken cancellationToken = default);
    }
}