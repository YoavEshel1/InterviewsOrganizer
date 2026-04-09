using InterviewsOrganizer.Models.Enums;

namespace InterviewsOrganizer.Models.Entities
{
    public class Interview
    {
        public Guid Id { get; set; }
        public DateTime Date { get; set; }
        public InterviewStatus Status { get; set; }
        public string ? Notes { get; set; }
        public string CompanyName { get; set; }
    }
}
