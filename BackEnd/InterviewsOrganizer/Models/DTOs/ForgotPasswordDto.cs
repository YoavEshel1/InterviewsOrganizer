namespace InterviewsOrganizer.Models.DTOs
{
    /// <summary>
    /// Request payload for initiating a password reset. The response is always
    /// success-shaped (unless the Turnstile token is invalid) to prevent user enumeration.
    /// </summary>
    public class ForgotPasswordDto
    {
        public required string Email { get; set; }

        /// <summary>
        /// Cloudflare Turnstile response token produced by the client-side widget. Verified
        /// server-side via <c>CloudflareTurnstile:VerifyUrl</c> before any work occurs.
        /// </summary>
        public required string TurnstileToken { get; set; }
    }
}
