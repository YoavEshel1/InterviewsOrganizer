using InterviewsOrganizer.Models.Entities;
using InterviewsOrganizer.Repositories.Interfaces;
using InterviewsOrganizer.Services.Interfaces;

namespace InterviewsOrganizer.Services
{
    public class PositionService : IPositionService
    {
        private readonly IPositionRepository _positionRepository;

        public PositionService(IPositionRepository positionRepository)
        {
            _positionRepository = positionRepository;
        }

        public async Task<List<Position>> GetAllAsync(Guid companyId) =>
            await _positionRepository.GetAll(companyId);

        public async Task<Position?> GetByIdAsync(Guid id) =>
            await _positionRepository.GetById(id);

        public async Task CreateAsync(Position position, Guid companyId)
        {
            await _positionRepository.Add(position, companyId);
            await _positionRepository.Save();
        }
    }
}