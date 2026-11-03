using InterviewsOrganizer.Data;
using InterviewsOrganizer.Models.Entities;
using InterviewsOrganizer.Services.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace InterviewsOrganizer.Services
{
    public class UserCleanupService : IUserCleanupService
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly IInterviewService _interviewService;
        private readonly IPositionService _positionService;
        private readonly ICompanyService _companyService;
        private readonly IUserProfileService _userProfileService;

        public UserCleanupService(
            AppDbContext context,
            UserManager<AppUser> userManager,
            IInterviewService interviewService,
            IPositionService positionService,
            ICompanyService companyService,
            IUserProfileService userProfileService)
        {
            _context = context;
            _userManager = userManager;
            _interviewService = interviewService;
            _positionService = positionService;
            _companyService = companyService;
            _userProfileService = userProfileService;
        }

        public async Task<IdentityResult> DeleteUserAndDataAsync(string userId, CancellationToken cancellationToken = default)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user is null) return IdentityResult.Failed(new IdentityError { Description = "User not found." });

            await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

            // Delete in FK-safe order: Interviews -> Positions -> Companies.
            await _interviewService.DeleteByUserIdAsync(userId, cancellationToken);
            await _positionService.DeleteByUserIdAsync(userId, cancellationToken);
            await _companyService.DeleteByUserIdAsync(userId, cancellationToken);

            // GDPR Article 17: anonymize + remove UserProfile PII explicitly (cascade delete
            // would also remove the row, but this keeps the wipe auditable and explicit).
            await _userProfileService.DeleteUserDataGdprAsync(userId, cancellationToken);

            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                await transaction.RollbackAsync(cancellationToken);
                return result;
            }

            await transaction.CommitAsync(cancellationToken);
            return result;
        }
    }
}
