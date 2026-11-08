namespace InterviewsOrganizer.Models.DTOs
{
    /// <summary>
    /// Result of a forgot-password or reset-password operation.
    /// </summary>
    public class PasswordResetResultDto
    {
        public bool Success { get; set; }
        public string? ErrorMessage { get; set; }

        public static PasswordResetResultDto Ok() =>
            new() { Success = true };

        public static PasswordResetResultDto Fail(string errorMessage) =>
            new() { Success = false, ErrorMessage = errorMessage };
    }
}
