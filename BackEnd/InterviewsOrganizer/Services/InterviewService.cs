using InterviewsOrganizer.Models.DTOs;
using InterviewsOrganizer.Models.Entities;
using InterviewsOrganizer.Repositories.Interfaces;
using InterviewsOrganizer.Services.Interfaces;


namespace InterviewsOrganizer.Services
{
    public class InterviewService : IInterviewService
    {
        private readonly IInterviewRepository _interviewRepository;

        public InterviewService(IInterviewRepository interviewRepository)
        {
            _interviewRepository = interviewRepository;
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

        public async Task<bool> DeleteAsync(Guid id)
        {
            var interview = await _interviewRepository.GetById(id);
            if (interview is null) return false;
            // Add Delete to repository if needed
            return false;
        }

        public async Task<UpdateInterviewDto> UpdateAsync(Guid id, Guid positionId, UpdateInterviewDto dto, CancellationToken cancellationToken = default)
        {
            if (dto is null) throw new ArgumentNullException(nameof(dto));

            var updatedInterview = await _interviewRepository.Update(id, positionId, dto.Date, dto.Feeling, dto.Interviewer, dto.Notes, cancellationToken);
            await _interviewRepository.Save(cancellationToken);
            return new UpdateInterviewDto(updatedInterview.Date, updatedInterview.Feeling, updatedInterview.Interviewer, updatedInterview.Notes);
        }

        public async Task DeleteByUserIdAsync(string userId, CancellationToken cancellationToken = default) =>
            await _interviewRepository.DeleteByUserIdAsync(userId, cancellationToken);
    }
}


