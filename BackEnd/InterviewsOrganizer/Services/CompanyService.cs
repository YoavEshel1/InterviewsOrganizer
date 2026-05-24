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

        public async Task<List<Company>> GetAllAsync(CancellationToken cancellationToken = default) =>
            await _companyRepository.GetAll(cancellationToken); 

        public async Task<Company?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
            await _companyRepository.GetById(id, cancellationToken);  

        public async Task CreateAsync(Company company)
        {
            await _companyRepository.Add(company);
            await _companyRepository.Save();
        }
    }
}