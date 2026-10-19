using InterviewsOrganizer.Services.Interfaces;
using System.Security.Claims;

namespace InterviewsOrganizer.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        private ClaimsPrincipal? User => _httpContextAccessor.HttpContext?.User;

        public string? UserId => User?.FindFirstValue(ClaimTypes.NameIdentifier);

        public bool IsImpersonated => User?.FindFirstValue("IsImpersonated") == "true";

        public string? OriginalAdminId => User?.FindFirstValue("OriginalAdminId");
    }
}
