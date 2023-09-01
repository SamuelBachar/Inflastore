using InflaStoreWebAPI.Services.CompanyService;
using Microsoft.AspNetCore.Mvc;

namespace InflaStoreWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompanyController : ControllerBase
    {
        private readonly ICompanyService _companyService;

        public CompanyController(ICompanyService unitsService)
        {
            _companyService = unitsService;
        }

        [HttpGet("GetAllCompanies")]
        public async Task<List<Company>> GetAllCompaniesAsync()
        {
            return await _companyService.GetAllCompaniesAsync();
        }

        [HttpGet("GetSpecificCompanies")]
        public async Task<List<Company>> GetSpecificCompaniesAsync(string strListIds)
        {
            var separated = strListIds.Split(new char[] { ',' });
            List<int> listIds = separated.Select(s => int.Parse(s)).ToList();

            return await _companyService.GetSpecificCompaniesAsync(listIds);
        }
    }
}
