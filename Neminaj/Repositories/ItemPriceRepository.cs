using Neminaj.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neminaj.Repositories;

public class ItemPriceRepository
{
    public static string _dbPath { get; } = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "neminaj.db3");

    public ItemPriceRepository()
    {
        
    }

    public async Task<List<ItemPrice>> GetAllItemPricesAsync()
    {
        try
        {
            await SQLConnection.InitAsync();
            return await SQLConnection.m_ConnectionAsync.Table<ItemPrice>().ToListAsync();
        }
        catch (Exception ex)
        {
            SQLConnection.StatusMessage = $"Failed to retrieve data. {ex.Message}";
        }

        return new List<ItemPrice>();
    }

    public async Task<List<ItemPrice>> GetPriceItemsFilteredAsync(string filter)
    {
        try
        {
            await SQLConnection.InitAsync();
            return await SQLConnection.m_ConnectionAsync.QueryAsync<ItemPrice>($"SELECT * FROM ItemPrice WHERE {filter}");
        }
        catch (Exception ex)
        {
            SQLConnection.StatusMessage = $"Failed to retrieve data. {ex.Message}";
        }

        return new List<ItemPrice>();
    }

}
