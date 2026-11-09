using InterviewsOrganizer.Data;
using InterviewsOrganizer.Models.DTOs;
using InterviewsOrganizer.Models.Entities;
using InterviewsOrganizer.Models.Options;
using InterviewsOrganizer.Services.Interfaces;
using InterviewsOrganizer.Shared;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
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
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailService;
        private readonly AzureCommunicationServicesOptions _acsOptions;
        private readonly ILogger<AuthService> _logger;

        public AuthService(
            UserManager<AppUser> userManager,
            AppDbContext context,
            IEmailEncryptionService emailEncryptionService,
            IUserProfileService userProfileService,
            ITokenService tokenService,
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration,
            IEmailService emailService,
            IOptions<AzureCommunicationServicesOptions> acsOptions,
            ILogger<AuthService> logger)
        {
            _userManager = userManager;
            _context = context;
            _emailEncryptionService = emailEncryptionService;
            _userProfileService = userProfileService;
            _tokenService = tokenService;
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _emailService = emailService;
            _acsOptions = acsOptions.Value;
            _logger = logger;
        }

        public async Task<AuthResponseDto> RegisterAsync(RegisterDto dto, CancellationToken ct = default)
        {
            // Validate the Turnstile token before doing any other work.
            var turnstileValid = await ValidateTurnstileTokenAsync(dto.TurnstileToken, ct);
            if (!turnstileValid)
            {
                return AuthResponseDto.Fail("Captcha verification failed. Please try again.");
            }

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

                // Immediately log the new user in, same as a successful LoginAsync.
                var tokens = await IssueTokensAsync(user, ct);

                // Dispatch the welcome email on a background task so it never delays the
                // HTTP response carrying the auto-login tokens. Registration must succeed
                // regardless of email delivery outcome, so failures are only logged inside
                // AzureEmailService and never bubble up here.
                _ = Task.Run(() => _emailService.SendWelcomeEmailAsync(dto.Email, dto.FirstName, CancellationToken.None));

                return AuthResponseDto.Ok(profileDto, tokens);
            }
            catch
            {
                await transaction.RollbackAsync(ct);
                throw; // Pass to Global Exception Handler
            }
        }

        /// <summary>
        /// Verifies a Cloudflare Turnstile response token against Cloudflare's siteverify endpoint.
        /// Fails closed (returns false) on any error, missing configuration, or cancellation.
        /// </summary>
        private async Task<bool> ValidateTurnstileTokenAsync(string? turnstileToken, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(turnstileToken))
                return false;           

            var secretKey = _configuration["CloudflareTurnstile:SecretKey"];
            if (string.IsNullOrWhiteSpace(secretKey))
            {
                _logger.LogError("CloudflareTurnstile:SecretKey is not configured; failing closed.");
                return false;
            }

            var verifyUrl = _configuration["CloudflareTurnstile:VerifyUrl"]
                ?? "https://challenges.cloudflare.com/turnstile/v0/siteverify";

            try
            {
                // Use the HttpClient defined at Program.cs with the name "Turnstile" to make the request to Cloudflare's Turnstile verification endpoint.
                var client = _httpClientFactory.CreateClient("Turnstile");
                var content = new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    ["secret"] = secretKey,
                    ["response"] = turnstileToken
                });

                using var httpResponse = await client.PostAsync(verifyUrl, content, ct);
                if (!httpResponse.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Turnstile siteverify returned status {StatusCode}.", httpResponse.StatusCode);
                    return false;
                }

                var verifyResult = await httpResponse.Content.ReadFromJsonAsync<TurnstileVerifyResponseDto>(cancellationToken: ct);
                if (verifyResult is null || !verifyResult.Success)
                {
                    _logger.LogWarning("Turnstile verification failed. ErrorCodes: {ErrorCodes}",
                        verifyResult?.ErrorCodes is null ? "none" : string.Join(",", verifyResult.ErrorCodes));
                    return false;
                }

                return true;
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while validating Turnstile token.");
                return false;
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

        public async Task<PasswordResetResultDto> ForgotPasswordAsync(ForgotPasswordDto dto, CancellationToken ct = default)
        {
            // Validate the Turnstile token before doing any other work.
            var turnstileValid = await ValidateTurnstileTokenAsync(dto.TurnstileToken, ct);
            if (!turnstileValid)
            {
                return PasswordResetResultDto.Fail("Captcha verification failed. Please try again.");
            }

            var emailHash = _emailEncryptionService.ComputeEmailHash(dto.Email);
            var user = await _context.Users.FirstOrDefaultAsync(u => u.EmailHash == emailHash, ct);

            if (user is not null)
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var resetUrl = $"{_acsOptions.BaseUrl.TrimEnd('/')}/reset-password?email={Uri.EscapeDataString(dto.Email)}&token={Uri.EscapeDataString(token)}";

                // Dispatch the email on a background task so it never delays the HTTP response.
                // Delivery failures are only logged inside AzureEmailService and never bubble up here.
                _ = Task.Run(() => _emailService.SendPasswordResetEmailAsync(dto.Email, resetUrl, CancellationToken.None));
            }

            // Always report success regardless of whether the email matched a user, to prevent
            // account enumeration.
            return PasswordResetResultDto.Ok();
        }

        public async Task<PasswordResetResultDto> ResetPasswordAsync(ResetPasswordDto dto, CancellationToken ct = default)
        {
            // Validate the Turnstile token before doing any other work.
            var turnstileValid = await ValidateTurnstileTokenAsync(dto.TurnstileToken, ct);
            if (!turnstileValid)
            {
                return PasswordResetResultDto.Fail("Captcha verification failed. Please try again.");
            }

            var emailHash = _emailEncryptionService.ComputeEmailHash(dto.Email);
            var user = await _context.Users.FirstOrDefaultAsync(u => u.EmailHash == emailHash, ct);
            if (user is null)
            {
                return PasswordResetResultDto.Fail("Invalid or expired reset token.");
            }

            var result = await _userManager.ResetPasswordAsync(user, dto.Token, dto.NewPassword);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return PasswordResetResultDto.Fail(errors);
            }

            // Invalidate any existing refresh token as a security hardening measure after a password reset.
            user.RefreshToken = null;
            user.RefreshTokenExpiry = null;
            await _userManager.UpdateAsync(user);

            return PasswordResetResultDto.Ok();
        }
    }
}
