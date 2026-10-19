using InterviewsOrganizer.Models.Entities;

namespace InterviewsOrganizer.Services.Interfaces
{
    public interface ITokenService
    {
        string GenerateAccessToken(AppUser user, IList<string> roles);
        string GenerateImpersonationToken(AppUser targetUser, string originalAdminId);
    }
}
