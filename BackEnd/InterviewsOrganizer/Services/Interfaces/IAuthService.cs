using InterviewsOrganizer.Models.DTOs;

namespace InterviewsOrganizer.Services.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponseDto> RegisterAsync(RegisterDto dto, CancellationToken ct = default);

        /// <summary>
        /// Validates credentials and, on success, issues a new access/refresh token pair and
        /// persists the refresh token against the user.
        /// </summary>
        Task<LoginResultDto> LoginAsync(LoginDto dto, CancellationToken ct = default);

        /// <summary>
        /// Exchanges a still-valid refresh token for a new access/refresh token pair.
        /// </summary>
        Task<LoginResultDto> RefreshAsync(string? refreshToken, CancellationToken ct = default);

        /// <summary>
        /// Invalidates the given refresh token (if it belongs to a user) so it can no longer be used.
        /// </summary>
        Task LogoutAsync(string? refreshToken, CancellationToken ct = default);

        /// <summary>
        /// Initiates a password reset for the given email. Always returns a successful result
        /// unless the Turnstile token itself is invalid, to prevent user enumeration.
        /// </summary>
        Task<PasswordResetResultDto> ForgotPasswordAsync(ForgotPasswordDto dto, CancellationToken ct = default);

        /// <summary>
        /// Completes a password reset using a token previously issued by <see cref="ForgotPasswordAsync"/>.
        /// </summary>
        Task<PasswordResetResultDto> ResetPasswordAsync(ResetPasswordDto dto, CancellationToken ct = default);
    }
}
