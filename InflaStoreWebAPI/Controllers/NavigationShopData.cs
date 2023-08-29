using InflaStoreWebAPI.Services.NavigationShopDatasService;
using Microsoft.AspNetCore.Mvc;

namespace InflaStoreWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NavigationShopDataController : ControllerBase
    {
        private readonly INavigationShopDatas _navigationShopDataService;

        public NavigationShopDataController(INavigationShopDatas navigationShopDatasService)
        {
            _navigationShopDataService = navigationShopDatasService;
        }

        [HttpGet("GetAllNavigationShopData")]
        public async Task<List<NavigationShopData>> GetAllNavigationShopDataAsync()
        {
            return await _navigationShopDataService.GetAllNavigationShopDataAsync();
        }

        [HttpGet("GetSpecificCompaniesNavigationShopData")]
        public async Task<List<NavigationShopData>> GetSpecificCompaniesNavigationShopDataAsync(string strListIds)
        {
            var separated = strListIds.Split(new char[] { ',' });
            List<int> listIds = separated.Select(s => int.Parse(s)).ToList();

            return await _navigationShopDataService.GetSpecificCompaniesNavigationShopDataAsync(listIds);
        }
    }
}
