using Android.Widget;
using Neminaj.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neminaj.Repositories;

public class SavedCartRepository
{
    public SavedCartRepository()
    {
            
    }

    public async Task<List<SavedCart>> GetAllSavedCartsAsync()
    {
        try
        {
            await SQLConnection.InitAsync();
            return await SQLConnection.m_ConnectionAsync.Table<SavedCart>().ToListAsync();
        }
        catch (Exception ex)
        {
            SQLConnection.StatusMessage = $"Failed to retrieve data. {ex.Message}";
        }

        return new List<SavedCart>();
    }

    public async Task<bool> InsertNewCart(SavedCart savedCart)
    {
        try
        {
            await SQLConnection.InitAsync();
            return await SQLConnection.m_ConnectionAsync.InsertAsync(savedCart) > 0;
        }
        catch (Exception ex)
        {
            SQLConnection.StatusMessage = $"Failed to insert data into table: SavedCart. {ex.Message}";
        }

        return false;
    }

    public async Task<List<SavedCartItem>> GetAllSavedCartItemsAsync(int filter)
    {
        try
        {
            await SQLConnection.InitAsync();
            return await SQLConnection.m_ConnectionAsync.Table<SavedCartItem>().Where(cart => cart.SavedCart_Id == filter).ToListAsync();
        }
        catch (Exception ex)
        {
            SQLConnection.StatusMessage = $"Failed to retrieve data. {ex.Message}";
        }

        return new List<SavedCartItem>();
    }

    public async Task<bool> InsertNewCartItems(List<SavedCartItem> listSavedCartItems)
    {
        try
        {
            await SQLConnection.InitAsync();
            return await SQLConnection.m_ConnectionAsync.InsertAllAsync(listSavedCartItems) > 0;
        }
        catch (Exception ex)
        {
            SQLConnection.StatusMessage = $"Failed to insert data into table: SavedCartItems. {ex.Message}";
        }

        return false;
    }

    public async Task<bool> DeleteSavedCart(List<SavedCartItem> listSavedCartItems, SavedCart savedCart)
    {
        try
        {
            await SQLConnection.InitAsync();

            foreach (SavedCartItem item in listSavedCartItems)
                await SQLConnection.m_ConnectionAsync.DeleteAsync(item);

            return await SQLConnection.m_ConnectionAsync.DeleteAsync(savedCart) > 0;
        }
        catch (Exception ex)
        {
            SQLConnection.StatusMessage = $"Failed to insert data into table: SavedCartItems. {ex.Message}";
        }

        return false;
    }

    public async Task<int> GetLastCartIndex()
    {
        try
        {
            await SQLConnection.InitAsync();

            return await SQLConnection.m_ConnectionAsync.Table<SavedCartItem>().CountAsync();
        }
        catch (Exception ex)
        {
            SQLConnection.StatusMessage = $"Failed to load last index table : SavedCartItems. {ex.Message}";
        }

        return -1;
    }
    
}


