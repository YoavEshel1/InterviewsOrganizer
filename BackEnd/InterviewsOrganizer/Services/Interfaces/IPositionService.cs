using InterviewsOrganizer.Models.Entities;

namespace InterviewsOrganizer.Services.Interfaces
{
    public interface IPositionService
    {
        Task<List<Position>> GetAllAsync(Guid companyId);
        Task<Position?> GetByIdAsync(Guid id);
        Task CreateAsync(Position position, Guid companyId);
    }
}