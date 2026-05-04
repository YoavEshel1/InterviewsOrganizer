namespace InterviewsOrganizer.Models.Entities
{
    public class Company
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public string? CompanyInfo { get; set; }
        public List<Position>? Positions { get; set; }
    }
}
