using InterviewsOrganizer.Models.Entities;

namespace InterviewsOrganizer.Repositories.Interfaces
{
    public interface IUserProfileRepository
    {
        Task<UserProfile?> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default);
        Task AddAsync(UserProfile profile, CancellationToken cancellationToken = default);
        Task<UserProfile> CreateOrUpdateAsync(string userId, string firstName, string lastName, CancellationToken cancellationToken = default);
        Task AnonymizeAsync(string userId, CancellationToken cancellationToken = default);
        Task DeleteByUserIdAsync(string userId, CancellationToken cancellationToken = default);
        Task Save(CancellationToken cancellationToken = default);
    }
}
