using InterviewsOrganizer.Services.Interfaces;
using System.Security.Claims;

namespace InterviewsOrganizer.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<CurrentUserService> _logger;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor, ILogger<CurrentUserService> logger)
        {
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        private ClaimsPrincipal? User => _httpContextAccessor.HttpContext?.User;

        public string? UserId
        {
            get
            {
                var userId = User?.FindFirstValue(ClaimTypes.NameIdentifier);
                if (userId is null && _httpContextAccessor.HttpContext is not null)
                {
                    _logger.LogWarning("UserId requested but no NameIdentifier claim was found on the current HttpContext.User");
                }
                return userId;
            }
        }

        public bool IsImpersonated => User?.FindFirstValue("IsImpersonated") == "true";

        public string? OriginalAdminId => User?.FindFirstValue("OriginalAdminId");
    }
}
