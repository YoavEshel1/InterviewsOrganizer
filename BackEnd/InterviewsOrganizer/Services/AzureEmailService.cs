using Azure;
using Azure.Communication.Email;
using InterviewsOrganizer.Models.Options;
using InterviewsOrganizer.Services.Interfaces;
using Microsoft.Extensions.Options;

namespace InterviewsOrganizer.Services
{
    /// <summary>
    /// Sends transactional emails via Azure Communication Services (ACS) Email.
    /// Loads the HTML template once and replaces tokens per send.
    /// </summary>
    public class AzureEmailService : IEmailService
    {
        private readonly EmailClient _emailClient;
        private readonly AzureCommunicationServicesOptions _options;
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<AzureEmailService> _logger;

        private const string WelcomeTemplateRelativePath = "Templates/WelcomeEmail.html";
        private const string PasswordResetTemplateRelativePath = "Templates/PasswordResetEmail.html";

        public AzureEmailService(
            EmailClient emailClient,
            IOptions<AzureCommunicationServicesOptions> options,
            IWebHostEnvironment environment,
            ILogger<AzureEmailService> logger)
        {
            _emailClient = emailClient;
            _options = options.Value;
            _environment = environment;
            _logger = logger;
        }

        public async Task SendWelcomeEmailAsync(string toAddress, string firstName, CancellationToken ct = default)
        {
            try
            {
                var templatePath = Path.Combine(_environment.ContentRootPath, WelcomeTemplateRelativePath);
                var htmlBody = await File.ReadAllTextAsync(templatePath, ct);

                var unsubscribeUrl = $"{_options.BaseUrl.TrimEnd('/')}/unsubscribe?email={Uri.EscapeDataString(toAddress)}";

                htmlBody = htmlBody
                    .Replace("{{FirstName}}", firstName)
                    .Replace("{{UnsubscribeUrl}}", unsubscribeUrl);

                var emailMessage = new EmailMessage(
                    senderAddress: _options.SenderAddress,
                    recipientAddress: toAddress,
                    content: new EmailContent("Welcome to Interviews Organizer")
                    {
                        Html = htmlBody
                    });

                // Explicit No-Reply pattern: replies are never expected, so ReplyTo is left
                // pointing at the same no-reply sender address rather than a monitored mailbox.
                emailMessage.ReplyTo.Add(new EmailAddress(_options.SenderAddress));

                // Fire the send request and wait until Azure accepts/completes the operation,
                // but this call itself runs on a background Task (see AuthService), so it
                // never blocks the original HTTP response to the client.
                await _emailClient.SendAsync(WaitUntil.Completed, emailMessage, ct);

                _logger.LogInformation("Welcome email sent to {Recipient}.", toAddress);
            }
            catch (RequestFailedException ex)
            {
                // Email delivery failures must never break registration; log and move on.
                _logger.LogError(ex, "Failed to send welcome email to {Recipient} via Azure Communication Services.", toAddress);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while sending welcome email to {Recipient}.", toAddress);
            }
        }

        public async Task SendPasswordResetEmailAsync(string toAddress, string resetUrl, CancellationToken ct = default)
        {
            try
            {
                var templatePath = Path.Combine(_environment.ContentRootPath, PasswordResetTemplateRelativePath);
                var htmlBody = await File.ReadAllTextAsync(templatePath, ct);

                htmlBody = htmlBody.Replace("{{ResetUrl}}", resetUrl);

                var emailMessage = new EmailMessage(
                    senderAddress: _options.SenderAddress,
                    recipientAddress: toAddress,
                    content: new EmailContent("Reset your Interviews Organizer password")
                    {
                        Html = htmlBody
                    });

                // Explicit No-Reply pattern: replies are never expected, so ReplyTo is left
                // pointing at the same no-reply sender address rather than a monitored mailbox.
                emailMessage.ReplyTo.Add(new EmailAddress(_options.SenderAddress));

                // Fire the send request and wait until Azure accepts/completes the operation,
                // but this call itself runs on a background Task (see AuthService), so it
                // never blocks the original HTTP response to the client.
                await _emailClient.SendAsync(WaitUntil.Completed, emailMessage, ct);

                _logger.LogInformation("Password reset email sent to {Recipient}.", toAddress);
            }
            catch (RequestFailedException ex)
            {
                // Email delivery failures must never break the forgot-password flow; log and move on.
                _logger.LogError(ex, "Failed to send password reset email to {Recipient} via Azure Communication Services.", toAddress);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while sending password reset email to {Recipient}.", toAddress);
            }
        }
    }
}
