using SharedTypesLibrary.Models.API.DatabaseModels;

namespace InflaStoreWebAPI.Services.NavigationShopDatasService;

public interface INavigationShopDatas
{
    public Task<List<NavigationShopData>> GetAllNavigationShopDataAsync();

    public Task<List<NavigationShopData>> GetSpecificCompaniesNavigationShopDataAsync(List<int> listIds);
}
