using SharedTypesLibrary.Models.API.DatabaseModels;

namespace InflaStoreWebAPI.Services.CompanyService;

public interface ICompanyService
{
    public Task<List<CompanyDTO>> GetAllCompaniesAsync();

    public Task<List<CompanyDTO>> GetSpecificCompaniesAsync(List<int> listIds);
}
