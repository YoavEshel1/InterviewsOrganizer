using InterviewsOrganizer.Models.DTOs;
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

        public async Task<UpdatePositionDto> UpdateAsync(Guid id, Guid companyId, UpdatePositionDto dto, CancellationToken cancellationToken = default)
        {
            if (dto is null) throw new ArgumentNullException(nameof(dto));

            var updatedPosition = await _positionRepository.Update(id, companyId, dto.Title, dto.ApplyDate, dto.Status, dto.Notes, cancellationToken);
            await _positionRepository.Save(cancellationToken);
            return new UpdatePositionDto(updatedPosition.Title, updatedPosition.ApplyDate, updatedPosition.Status, updatedPosition.Notes);
        }

        public async Task DeleteByUserIdAsync(string userId, CancellationToken cancellationToken = default) =>
            await _positionRepository.DeleteByUserIdAsync(userId, cancellationToken);
    }
}