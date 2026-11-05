namespace InterviewsOrganizer.Models.DTOs
{
    public class RegisterDto
    {
        public required string Email { get; set; }
        public required string Password { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }

        /// <summary>
        /// Cloudflare Turnstile response token produced by the client-side widget. Verified
        /// server-side via <c>CloudflareTurnstile:VerifyUrl</c> before any registration occurs.
        /// </summary>
        public required string TurnstileToken { get; set; }
    }
}
