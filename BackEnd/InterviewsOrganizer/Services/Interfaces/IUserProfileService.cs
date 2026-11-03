using InterviewsOrganizer.Models.DTOs;

namespace InterviewsOrganizer.Services.Interfaces
{
    public interface IUserProfileService
    {
        Task<UserProfileDto?> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default);
        Task<UserProfileDto> CreateOrUpdateAsync(string userId, UpdateUserProfileDto dto, CancellationToken cancellationToken = default);

        /// <summary>
        /// GDPR Article 17 (Right to be Forgotten) cleanup: anonymizes the user's PII
        /// (FirstName/LastName) and then permanently removes the UserProfile row.
        /// Safe to call even if no profile exists (no-op).
        /// </summary>
        Task DeleteUserDataGdprAsync(string userId, CancellationToken cancellationToken = default);
    }
}
