namespace InterviewsOrganizer.Services.Interfaces
{
    public interface IChatService
    {
        Task<string> Chat(string userInput);
    }
}
