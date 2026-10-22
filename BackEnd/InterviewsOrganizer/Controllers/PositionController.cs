using InterviewsOrganizer.Models.DTOs;
using InterviewsOrganizer.Models.Entities;
using InterviewsOrganizer.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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
        [Authorize]
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
        [Authorize]
        public async Task<IActionResult> Create(Guid companyId, CreatePositionDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var position = new Position
            {
                Title = dto.Title,
                ApplyDate = dto.ApplyDate,
                Status = dto.Status,
                Notes = dto.Notes,
                CompanyId = companyId,
                UserId = userId
            };
            await _service.CreateAsync(position, companyId);
            return CreatedAtAction(nameof(GetById), new { companyId, id = position.Id }, position);
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> Update(Guid companyId, Guid id, UpdatePositionDto dto, CancellationToken cancellationToken)
        {
            var updatedPosition = await _service.UpdateAsync(id, companyId, dto, cancellationToken);
            return Ok(updatedPosition);
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(Guid companyId, Guid id, CancellationToken cancellationToken)
        {
            await _service.DeleteAsync(id, companyId, cancellationToken);
            return NoContent();
        }
    }
}