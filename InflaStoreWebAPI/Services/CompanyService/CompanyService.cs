using System.Web;
using static System.Net.WebRequestMethods;

namespace InflaStoreWebAPI.Services.CompanyService;
public class CompanyService : ICompanyService
{
    private readonly DataContext _context;

    public CompanyService(DataContext context)
    {
        _context = context;
    }

    public async Task<List<CompanyDTO>> GetAllCompaniesAsync()
    {
        List<CompanyDTO> listCompanyDTOs = new List<CompanyDTO>();
        List<Company> listCompany = await _context.Companies.ToListAsync();

        foreach (Company company in listCompany)
        {
            string imgPath = Path.Combine(Directory.GetCurrentDirectory(), $"wwwroot\\StaticFile\\Companies\\Logo\\{company.Path}");

            listCompanyDTOs.Add( new CompanyDTO
            {
                Id = company.Id,
                Name = company.Name,
                Url = @$"https://inflastoreapi.azurewebsites.net/StaticFile/Companies/Logo/{company.Path}",
                Image = await System.IO.File.ReadAllBytesAsync(imgPath)
            });
        }

        return listCompanyDTOs;
    }

    public async Task<List<CompanyDTO>> GetSpecificCompaniesAsync(List<int> listIds)
    {
        List<CompanyDTO> listCompanyDTOs = new List<CompanyDTO>();
        List<Company> listCompany = await _context.Companies.Where(company => listIds.Contains(company.Id)).ToListAsync();

        foreach (Company company in listCompany)
        {
            string imgPath = Path.Combine(Directory.GetCurrentDirectory(), $"wwwroot\\StaticFile\\Companies\\Logo\\{company.Path}");

            listCompanyDTOs.Add(new CompanyDTO
            {
                Id = company.Id,
                Name = company.Name,
                Url = @$"https://inflastoreapi.azurewebsites.net/StaticFile/Companies/Logo/{company.Path}",
                Image = await System.IO.File.ReadAllBytesAsync(imgPath)
            });
        }

        return listCompanyDTOs;
    }
}