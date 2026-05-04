using InterviewsOrganizer.Models.DTOs;
using InterviewsOrganizer.Models.Entities;
using InterviewsOrganizer.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace InterviewsOrganizer.Controllers
{
    [ApiController]
    [Route("api/companies/{companyId}/positions")]
    public class PositionController : ControllerBase
    {
        private readonly IPositionService _service;

        public PositionController(IPositionService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(Guid companyId)
        {
            var data = await _service.GetAllAsync(companyId);
            return Ok(data);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var position = await _service.GetByIdAsync(id);
            if (position is null) return NotFound();
            return Ok(position);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Guid companyId, CreatePositionDto dto)
        {
            var position = new Position
            {
                Title = dto.Title,
                ApplyDate = dto.ApplyDate,
                Status = dto.Status,
                Notes = dto.Notes
            };
            await _service.CreateAsync(position, companyId);
            return CreatedAtAction(nameof(GetById), new { companyId, id = position.Id }, position);
        }
    }
}