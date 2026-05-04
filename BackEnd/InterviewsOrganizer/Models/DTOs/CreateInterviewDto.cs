using InterviewsOrganizer.Models.Enums;

namespace InterviewsOrganizer.Models.DTOs
{
    public class CreateInterviewDto
    {
        public DateTime Date { get; set; }
        public InterviewFeeling Feeling { get; set; }
        public string? Interviewer { get; set; }
        public string? Notes { get; set; }
    }
}
