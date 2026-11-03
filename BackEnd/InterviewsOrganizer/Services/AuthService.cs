using InterviewsOrganizer.Data;
using InterviewsOrganizer.Models.DTOs;
using InterviewsOrganizer.Models.Entities;
using InterviewsOrganizer.Services.Interfaces;
using InterviewsOrganizer.Shared;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
namespace InterviewsOrganizer.Services
{
    public class AuthService: IAuthService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly AppDbContext _context;
        private readonly IEmailEncryptionService _emailEncryptionService;
        private readonly IUserProfileService _userProfileService;
        private readonly ITokenService _tokenService;

        public AuthService(
            UserManager<AppUser> userManager,
            AppDbContext context,
            IEmailEncryptionService emailEncryptionService,
            IUserProfileService userProfileService,
            ITokenService tokenService)
        {
            _userManager = userManager;
            _context = context;
            _emailEncryptionService = emailEncryptionService;
            _userProfileService = userProfileService;
            _tokenService = tokenService;
        }

        public async Task<AuthResponseDto> RegisterAsync(RegisterDto dto, CancellationToken ct = default)
        {
            // check if email is already registered
            var emailHash = _emailEncryptionService.ComputeEmailHash(dto.Email);
            var existingUser = await _context.Users.AnyAsync(u => u.EmailHash == emailHash, ct);
            if (existingUser)
            {
                return AuthResponseDto.Fail("Email is already registered.");
            }

            // 2. Begin DB Transaction
            await using var transaction = await _context.Database.BeginTransactionAsync(ct);

            try
            {
                var user = new AppUser
                {
                    UserName = $"user_{Guid.NewGuid()}",
                    Email = dto.Email
                };

                // Create Identity user
                var result = await _userManager.CreateAsync(user, dto.Password);
                if (!result.Succeeded)
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    return AuthResponseDto.Fail(errors);
                }

                // Create the UserProfile through IUserProfileService
                var updateProfileDto = new UpdateUserProfileDto(dto.FirstName, dto.LastName);
                var profileDto = await _userProfileService.CreateOrUpdateAsync(user.Id, updateProfileDto, ct);

                // Commit the transaction
                await transaction.CommitAsync(ct);

                return AuthResponseDto.Ok(profileDto);
            }
            catch
            {
                await transaction.RollbackAsync(ct);
                throw; // Pass to Global Exception Handler
            }
        }

        public async Task<LoginResultDto> LoginAsync(LoginDto dto, CancellationToken ct = default)
        {
            var emailHash = _emailEncryptionService.ComputeEmailHash(dto.Email);
            var user = await _context.Users.FirstOrDefaultAsync(u => u.EmailHash == emailHash, ct);
            if (user is null || !await _userManager.CheckPasswordAsync(user, dto.Password))
                return LoginResultDto.Fail("Invalid credentials.");

            var tokens = await IssueTokensAsync(user, ct);
            return LoginResultDto.Ok(tokens);
        }

        public async Task<LoginResultDto> RefreshAsync(string? refreshToken, CancellationToken ct = default)
        {
            if (string.IsNullOrEmpty(refreshToken))
                return LoginResultDto.Fail("Missing refresh token.");

            var user = await _context.Users.FirstOrDefaultAsync(u => u.RefreshToken == refreshToken, ct);
            if (user is null || user.RefreshTokenExpiry < DateTime.UtcNow)
                return LoginResultDto.Fail("Invalid or expired refresh token.");

            var tokens = await IssueTokensAsync(user, ct);
            return LoginResultDto.Ok(tokens);
        }

        public async Task LogoutAsync(string? refreshToken, CancellationToken ct = default)
        {
            if (string.IsNullOrEmpty(refreshToken)) return;

            var user = await _context.Users.FirstOrDefaultAsync(u => u.RefreshToken == refreshToken, ct);
            if (user is null) return;

            user.RefreshToken = null;
            user.RefreshTokenExpiry = null;
            await _userManager.UpdateAsync(user);
        }

        private async Task<TokenResultDto> IssueTokensAsync(AppUser user, CancellationToken ct)
        {
            var roles = await _userManager.GetRolesAsync(user);
            var accessToken = _tokenService.GenerateAccessToken(user, roles);
            var refreshToken = GenerateRefreshToken();

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);
            await _userManager.UpdateAsync(user);

            return new TokenResultDto(accessToken, refreshToken);
        }

        private static string GenerateRefreshToken() =>
            Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));


    }
}
