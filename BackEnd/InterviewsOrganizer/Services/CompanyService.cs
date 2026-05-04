using InterviewsOrganizer.Models.Entities;
using InterviewsOrganizer.Repositories.Interfaces;
using InterviewsOrganizer.Services.Interfaces;

namespace InterviewsOrganizer.Services
{
    public class CompanyService : ICompanyService
    {
        private readonly ICompanyRepository _companyRepository;

        public CompanyService(ICompanyRepository companyRepository)
        {
            _companyRepository = companyRepository;
        }

        public async Task<List<Company>> GetAllAsync() =>
            await _companyRepository.GetAll();

        public async Task<Company?> GetByIdAsync(Guid id) =>
            await _companyRepository.GetById(id);

        public async Task CreateAsync(Company company)
        {
            await _companyRepository.Add(company);
            await _companyRepository.Save();
        }
    }
}