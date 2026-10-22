using InterviewsOrganizer.Models.Entities;
using InterviewsOrganizer.Services.Interfaces;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace InterviewsOrganizer.Services
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<TokenService> _logger;

        public TokenService(IConfiguration configuration, ILogger<TokenService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public string GenerateAccessToken(AppUser user, IList<string> roles)
        {
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.Id),
                new(ClaimTypes.Email, user.Email!)
            };

            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

            _logger.LogInformation("Access token generated for user {UserId}", user.Id);
            return CreateToken(claims, DateTime.UtcNow.AddMinutes(15));
        }

        public string GenerateImpersonationToken(AppUser targetUser, string originalAdminId)
        {
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, targetUser.Id),
                new(ClaimTypes.Email, targetUser.Email!),
                new(ClaimTypes.Role, "User"),
                new("IsImpersonated", "true"),
                new("OriginalAdminId", originalAdminId)
            };

            _logger.LogInformation(
                "Impersonation token generated for target user {TargetUserId} by admin {AdminUserId}",
                targetUser.Id, originalAdminId);
            return CreateToken(claims, DateTime.UtcNow.AddMinutes(60));
        }

        private string CreateToken(IEnumerable<Claim> claims, DateTime expires)
        {
            try
            {
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var token = new JwtSecurityToken(
                    issuer: _configuration["Jwt:Issuer"],
                    audience: _configuration["Jwt:Audience"],
                    claims: claims,
                    expires: expires,
                    signingCredentials: creds
                );

                return new JwtSecurityTokenHandler().WriteToken(token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create JWT token");
                throw;
            }
        }
    }
}
