namespace InterviewsOrganizer.Models.DTOs
{
    public class LoginResultDto
    {
        public bool Success { get; set; }
        public string? ErrorMessage { get; set; }
        public TokenResultDto? Tokens { get; set; }

        public static LoginResultDto Ok(TokenResultDto tokens) =>
            new() { Success = true, Tokens = tokens };

        public static LoginResultDto Fail(string errorMessage) =>
            new() { Success = false, ErrorMessage = errorMessage };
    }
}
