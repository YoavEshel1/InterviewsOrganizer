using InterviewsOrganizer.Models.DTOs;
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

        public async Task<UpdateCompanyDto> UpdateAsync(UpdateCompanyDto dto, CancellationToken cancellationToken = default)
        {
            if (dto is null) throw new ArgumentNullException(nameof(dto));

            var updatedCompany = await _companyRepository.Update(dto.Id, dto.Name, dto.CompanyInfo, cancellationToken);
            await _companyRepository.Save(cancellationToken);
            return new UpdateCompanyDto(updatedCompany.Id, updatedCompany.Name, updatedCompany.CompanyInfo);
        }

        public async Task DeleteByUserIdAsync(string userId, CancellationToken cancellationToken = default) =>
            await _companyRepository.DeleteByUserIdAsync(userId, cancellationToken);

        public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
        {
            if (id == Guid.Empty) throw new ArgumentException("Id cannot be empty.", nameof(id));
            await _companyRepository.DeleteAsync(id, cancellationToken);
            await _companyRepository.Save(cancellationToken);
        }
    }
}