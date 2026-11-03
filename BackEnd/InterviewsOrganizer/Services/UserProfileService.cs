using InterviewsOrganizer.Models.DTOs;
using InterviewsOrganizer.Models.Entities;
using InterviewsOrganizer.Repositories.Interfaces;
using InterviewsOrganizer.Services.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace InterviewsOrganizer.Services
{
    public class UserProfileService : IUserProfileService
    {
        private readonly IUserProfileRepository _userProfileRepository;
        private readonly UserManager<AppUser> _userManager;
        private readonly ILogger<UserProfileService> _logger;

        public UserProfileService(
            IUserProfileRepository userProfileRepository,
            UserManager<AppUser> userManager,
            ILogger<UserProfileService> logger)
        {
            _userProfileRepository = userProfileRepository;
            _userManager = userManager;
            _logger = logger;
        }

        /// <summary>
        /// Retrieves the profile (FirstName/LastName) for the given user, combined with their
        /// Email from Identity. Returns null if no profile has been created yet.
        /// </summary>
        public async Task<UserProfileDto?> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default)
        {
            var profile = await _userProfileRepository.GetByUserIdAsync(userId, cancellationToken);
            if (profile is null) return null;

            var user = await _userManager.FindByIdAsync(userId);
            return new UserProfileDto(profile.FirstName, profile.LastName, user?.Email, profile.UpdatedAt);
        }
        
        /// <summary>
        /// Creates the user's profile if it doesn't exist yet, or updates FirstName/LastName
        /// if it does. CreatedAt/UpdatedAt are populated automatically by AppDbContext.
        /// </summary>
        public async Task<UserProfileDto> CreateOrUpdateAsync(string userId, UpdateUserProfileDto dto, CancellationToken cancellationToken = default)
        {
            if (dto is null) throw new ArgumentNullException(nameof(dto));

            var profile = await _userProfileRepository.CreateOrUpdateAsync(userId, dto.FirstName, dto.LastName, cancellationToken);
            await _userProfileRepository.Save(cancellationToken);

            var user = await _userManager.FindByIdAsync(userId);
            _logger.LogInformation("UserProfile for user {UserId} created/updated", userId);
            return new UserProfileDto(profile.FirstName, profile.LastName, user?.Email, profile.UpdatedAt);
        }

        /// <summary>
        /// GDPR Article 17 (Right to be Forgotten) cleanup: anonymizes the user's PII
        /// (FirstName/LastName) and then permanently removes the UserProfile row.
        /// Safe to call even if no profile exists (no-op).
        /// </summary>
        public async Task DeleteUserDataGdprAsync(string userId, CancellationToken cancellationToken = default)
        {
            // Anonymize first so the PII overwrite is captured by auditing/change-tracking,
            // then hard-delete the row to satisfy GDPR Article 17 (Right to be Forgotten).
            await _userProfileRepository.AnonymizeAsync(userId, cancellationToken);
            await _userProfileRepository.Save(cancellationToken);

            await _userProfileRepository.DeleteByUserIdAsync(userId, cancellationToken);
            _logger.LogInformation("GDPR cleanup completed for UserProfile of user {UserId}", userId);
        }
    }
}
