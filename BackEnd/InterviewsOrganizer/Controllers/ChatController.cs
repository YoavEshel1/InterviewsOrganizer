using InterviewsOrganizer.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;




namespace InterviewsOrganizer.Controllers
{
    [ApiController]
    [Route("api/chat")]
    [AllowAnonymous] //TODO: Remove this line when authentication is implemented
    public class ChatController : ControllerBase
    {

        private readonly IChatService _service;
        public ChatController(IChatService service)
        {
            _service = service;
        }
        
        [HttpPost]
        public async Task<string> Chat(string userInput)
        {
            return await _service.Chat(userInput);
        }
    }
}
