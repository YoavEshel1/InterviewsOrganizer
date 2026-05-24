using InterviewsOrganizer.Models.DTOs;
using InterviewsOrganizer.Models.Entities;
using InterviewsOrganizer.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Timeouts;
using Microsoft.AspNetCore.Mvc;

namespace InterviewsOrganizer.Controllers
{
    [ApiController]
    [Route("api/companies")]
    public class CompanyController : ControllerBase
    {
        private readonly ICompanyService _service;

        public CompanyController(ICompanyService service)
        {
            _service = service;
        }

        [HttpGet]
        [Authorize]
        [RequestTimeout(120000)] // 2 minutes in milliseconds
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
           
            var data = await _service.GetAllAsync(cancellationToken);
            return Ok(data);
        }

        [HttpGet("{id}")]
        [Authorize]
        [RequestTimeout(120000)] // 2 minutes in milliseconds
        public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
        {
            var company = await _service.GetByIdAsync(id, cancellationToken);
            if (company is null) return NotFound();
            return Ok(company);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create(CreateCompanyDto dto)
        {
            var company = new Company
            {
                Name = dto.Name,
                CompanyInfo = dto.CompanyInfo
            };
            await _service.CreateAsync(company);
            return CreatedAtAction(nameof(GetById), new { id = company.Id }, company);
        }
    }
}