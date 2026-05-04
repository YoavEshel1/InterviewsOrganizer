using InterviewsOrganizer.Models.DTOs;
using InterviewsOrganizer.Models.Entities;
using InterviewsOrganizer.Services.Interfaces;
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
        public async Task<IActionResult> GetAll()
        {
            var data = await _service.GetAllAsync();
            return Ok(data);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var company = await _service.GetByIdAsync(id);
            if (company is null) return NotFound();
            return Ok(company);
        }

        [HttpPost]
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