using InterviewsOrganizer.Models.DTOs;
using InterviewsOrganizer.Models.Entities;
using InterviewsOrganizer.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace InterviewsOrganizer.Controllers
{
    [ApiController]
    [Route("api/positions/{positionId}/interviews")]
    public class InterviewController : ControllerBase
    {
        private readonly IInterviewService _service;

        public InterviewController(IInterviewService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(Guid positionId)
        {
            var data = await _service.GetAllAsync(positionId);
            return Ok(data);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var interview = await _service.GetByIdAsync(id);
            if (interview is null) return NotFound();
            return Ok(interview);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Guid positionId, CreateInterviewDto dto)
        {
            var interview = new Interview
            {
                Date = dto.Date,
                Feeling = dto.Feeling,
                Interviewer = dto.Interviewer,
                Notes = dto.Notes
            };
            await _service.CreateAsync(interview, positionId);
            return CreatedAtAction(nameof(GetById), new { positionId, id = interview.Id }, interview);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _service.DeleteAsync(id);
            if (!result) return NotFound();
            return NoContent();
        }
    }
}
