using InterviewsOrganizer.Models.DTOs;
using InterviewsOrganizer.Models.Entities;
using InterviewsOrganizer.Repositories.Interfaces;
using InterviewsOrganizer.Services.Interfaces;


namespace InterviewsOrganizer.Services
{
    public class InterviewService : IInterviewService
    {
        private readonly IInterviewRepository _interviewRepository;
        private readonly ILogger<InterviewService> _logger;

        public InterviewService(IInterviewRepository interviewRepository, ILogger<InterviewService> logger)
        {
            _interviewRepository = interviewRepository;
            _logger = logger;
        }

        public async Task<List<Interview>> GetAllAsync(Guid positionId) =>
            await _interviewRepository.GetAll(positionId);

        public async Task<Interview?> GetByIdAsync(Guid id) =>
            await _interviewRepository.GetById(id);

        public async Task CreateAsync(Interview interview, Guid positionId)
        {
            await _interviewRepository.Add(interview, positionId);
            await _interviewRepository.Save();
        }

        public async Task DeleteByUserIdAsync(string userId, CancellationToken cancellationToken = default) =>
            await _interviewRepository.DeleteByUserIdAsync(userId, cancellationToken);

        public async Task<UpdateInterviewDto> UpdateAsync(Guid id, Guid positionId, UpdateInterviewDto dto, CancellationToken cancellationToken = default)
        {
            if (dto is null) 
            {
                _logger.LogWarning("UpdateAsync called with a null dto.");
                throw new ArgumentNullException(nameof(dto));
            }

            var updatedInterview = await _interviewRepository.Update(id, positionId, dto.Date, dto.Feeling, dto.Interviewer, dto.Notes, cancellationToken);
            await _interviewRepository.Save(cancellationToken);
            return new UpdateInterviewDto(updatedInterview.Date, updatedInterview.Feeling, updatedInterview.Interviewer, updatedInterview.Notes);
        }

        public async Task DeleteAsync(Guid id, Guid positionId, CancellationToken cancellationToken = default)
        {
            if (id == Guid.Empty)
            { 
                _logger.LogWarning("DeleteAsync called with an empty Id.");
                throw new ArgumentException("Id cannot be empty.", nameof(id)); 
            }

            await _interviewRepository.DeleteAsync(id, positionId, cancellationToken);
            await _interviewRepository.Save(cancellationToken);
        }
    }
}


