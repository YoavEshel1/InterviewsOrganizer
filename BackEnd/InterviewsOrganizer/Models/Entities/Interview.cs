using InterviewsOrganizer.Models.Enums;

namespace InterviewsOrganizer.Models.Entities
{
    public class Interview
    {
        public Guid Id { get; set; }       
        public DateTime Date { get; set; }
        public InterviewFeeling Feeling { get; set; }           
        public Guid PositionId { get; set; }
        public string ? Interviewer { get; set; }
        public string? Notes { get; set; }
    }
}
