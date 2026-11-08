namespace InterviewsOrganizer.Services.Interfaces
{
    /// <summary>
    /// Sends transactional emails on behalf of the application. Implementations must be
    /// safe to invoke asynchronously without blocking the caller's HTTP response.
    /// </summary>
    public interface IEmailService
    {
        /// <summary>
        /// Sends the welcome email to a newly registered user.
        /// </summary>
        /// <param name="toAddress">Recipient email address.</param>
        /// <param name="firstName">Recipient first name, used for personalization.</param>
        /// <param name="ct">Cancellation token.</param>
        Task SendWelcomeEmailAsync(string toAddress, string firstName, CancellationToken ct = default);
    }
}
