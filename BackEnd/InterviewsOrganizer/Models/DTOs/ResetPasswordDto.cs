namespace InterviewsOrganizer.Models.DTOs
{
    /// <summary>
    /// Request payload for completing a password reset using the token issued via
    /// <see cref="ForgotPasswordDto"/>.
    /// </summary>
    public class ResetPasswordDto
    {
        public required string Email { get; set; }

        /// <summary>
        /// The ASP.NET Core Identity password reset token previously emailed to the user.
        /// </summary>
        public required string Token { get; set; }

        public required string NewPassword { get; set; }

        /// <summary>
        /// Cloudflare Turnstile response token produced by the client-side widget. Verified
        /// server-side via <c>CloudflareTurnstile:VerifyUrl</c> before any work occurs.
        /// </summary>
        public required string TurnstileToken { get; set; }
    }
}
