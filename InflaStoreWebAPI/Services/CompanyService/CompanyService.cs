namespace InflaStoreWebAPI.Services.CompanyService;
public class CompanyService : ICompanyService
{
    private readonly DataContext _context;

    public CompanyService(DataContext context)
    {
        _context = context;
    }

    public async Task<List<Company>> GetAllCompaniesAsync()
    {
        return await _context.Companies.ToListAsync();
    }

    public async Task<List<Company>> GetSpecificCompaniesAsync(List<int> listIds)
    {
        return await _context.Companies.Where(company => listIds.Contains(company.Id)).ToListAsync();
    }
}