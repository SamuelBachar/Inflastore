using SharedTypesLibrary.Models.API.DatabaseModels;

namespace InflaStoreWebAPI.Services.CompanyService;

public interface ICompanyService
{
    public Task<List<Company>> GetAllCompaniesAsync();

    public Task<List<Company>> GetSpecificCompaniesAsync(List<int> listIds);
}
