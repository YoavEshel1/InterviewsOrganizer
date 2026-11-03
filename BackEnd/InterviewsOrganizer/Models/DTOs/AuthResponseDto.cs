namespace InterviewsOrganizer.Models.DTOs
{
    public class AuthResponseDto
    {
        public bool Success { get; set; }
        public string? ErrorMessage { get; set; }
        public UserProfileDto? Profile { get; set; }

        public static AuthResponseDto Ok(UserProfileDto profile) =>
            new() { Success = true, Profile = profile };

        public static AuthResponseDto Fail(string errorMessage) =>
            new() { Success = false, ErrorMessage = errorMessage };
    }
}
