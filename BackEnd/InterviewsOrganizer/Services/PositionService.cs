using InterviewsOrganizer.Models.DTOs;
using InterviewsOrganizer.Models.Entities;
using InterviewsOrganizer.Repositories.Interfaces;
using InterviewsOrganizer.Services.Interfaces;

namespace InterviewsOrganizer.Services
{
    public class PositionService : IPositionService
    {
        private readonly IPositionRepository _positionRepository;
        private readonly ILogger<PositionService> _logger;

        public PositionService(IPositionRepository positionRepository, ILogger<PositionService> logger)
        {
            _positionRepository = positionRepository;
            _logger = logger;
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

        public async Task<UpdatePositionDto> UpdateAsync(Guid id, Guid companyId, UpdatePositionDto dto, CancellationToken cancellationToken = default)
        {
            if (dto is null)
            {
                _logger.LogWarning("UpdateAsync called with a null dto.");
                throw new ArgumentNullException(nameof(dto));
            }
                

            var updatedPosition = await _positionRepository.Update(id, companyId, dto.Title, dto.ApplyDate, dto.Status, dto.Notes, cancellationToken);
            await _positionRepository.Save(cancellationToken);
            return new UpdatePositionDto(updatedPosition.Title, updatedPosition.ApplyDate, updatedPosition.Status, updatedPosition.Notes);
        }

        public async Task DeleteByUserIdAsync(string userId, CancellationToken cancellationToken = default) =>
            await _positionRepository.DeleteByUserIdAsync(userId, cancellationToken);

        public async Task DeleteAsync(Guid id, Guid companyId, CancellationToken cancellationToken = default)
        {
            if (id == Guid.Empty)
            {
                _logger.LogWarning("DeleteAsync called with an empty Id.");
                throw new ArgumentException("Id cannot be empty.", nameof(id));
            }

            await _positionRepository.DeleteAsync(id, companyId, cancellationToken);
            await _positionRepository.Save(cancellationToken);
        }
    }
}