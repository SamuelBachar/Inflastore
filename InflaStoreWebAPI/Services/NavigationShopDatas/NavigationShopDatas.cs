namespace InflaStoreWebAPI.Services.NavigationShopDatasService;

public class NavigationShopDatas : INavigationShopDatas
{
    private readonly DataContext _context;

    public NavigationShopDatas(DataContext context)
    {
        _context = context;
    }

    public async Task<List<NavigationShopData>> GetAllNavigationShopDataAsync()
    {
        return await _context.NavigationShopDatas.ToListAsync();
    }

    public async Task<List<NavigationShopData>> GetSpecificCompaniesNavigationShopDataAsync(List<int> listIds)
    {
        return await _context.NavigationShopDatas.Where(navShopData => listIds.Contains(navShopData.Company_Id)).ToListAsync();
    }
}