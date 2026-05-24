namespace InterviewsOrganizer.Models.DTOs
{
    public class CreateUserDto
    {
        public required string Email { get; set; }
        public required string Password { get; set; }
        public required string Role { get; set; } // "Admin" or "User"
    }
}