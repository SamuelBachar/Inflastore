using Neminaj.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neminaj.Repositories;

public class NavigationShopRepository
{
    public NavigationShopRepository()
    {
            
    }

    public static string _dbPath { get; } = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "neminaj.db3");

    public async Task<List<NavigationShopData>> GetAllNavigShopDataAsync()
    {
        try
        {
            await SQLConnection.InitAsync();
            return await SQLConnection.m_ConnectionAsync.Table<NavigationShopData>().ToListAsync();
        }
        catch (Exception ex)
        {
            SQLConnection.StatusMessage = $"Failed to retrieve data. {ex.Message}";
        }

        return new List<NavigationShopData>();
    }

    public async Task<List<NavigationShopData>> GetSpecificCompaniesNavigShopsDataAsync(List<int> listCompaniesIds)
    {
        try
        {
            await SQLConnection.InitAsync();
            return await SQLConnection.m_ConnectionAsync.Table<NavigationShopData>().Where(navShopData => listCompaniesIds.Contains(navShopData.Company_Id)).ToListAsync();
        }
        catch (Exception ex)
        {
            SQLConnection.StatusMessage = $"Failed to retrieve data. {ex.Message}";
        }

        return new List<NavigationShopData>();
    }

}