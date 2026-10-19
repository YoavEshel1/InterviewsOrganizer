using Microsoft.AspNetCore.Identity;

namespace InterviewsOrganizer.Services.Interfaces
{
    public interface IUserCleanupService
    {
        /// <summary>
        /// Deletes a user along with all of their associated Interviews, Positions, and Companies.
        /// </summary>
        Task<IdentityResult> DeleteUserAndDataAsync(string userId, CancellationToken cancellationToken = default);
    }
}
