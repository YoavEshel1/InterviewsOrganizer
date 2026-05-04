using InterviewsOrganizer.Models.Entities;

namespace InterviewsOrganizer.Repositories.Interfaces
{
    public interface IPositionRepository
    {
        Task Add(Position position, Guid companyId);
        Task<List<Position>> GetAll(Guid companyId);
        Task<Position?> GetById(Guid id);
        Task Save();
    }
}