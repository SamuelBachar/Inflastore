﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Neminaj.Models;
using SQLite;
using Item = Neminaj.Models.Item;

namespace Neminaj.Repositories;

public class ItemRepository
{
    List<Item> FilteredItems = new List<Item>();
    public static string _dbPath { get; } = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "neminaj.db3");

    public ItemRepository()
    {

    }

    public async Task AddNewItemAsync(string name)
    {
        ArgumentNullException.ThrowIfNull(name, nameof(name));

        int result = 0;

        try
        {
            await SQLConnection.InitAsync();
            result = await SQLConnection.m_ConnectionAsync.InsertAsync(new Item { Name = name });

            SQLConnection.StatusMessage = string.Format("{0} record(s) added (Name: {1})", result, name);
        }
        catch (Exception ex)
        {
            SQLConnection.StatusMessage = string.Format("Failed to add {0}. Error: {1})", name, ex.Message);
        }
    }

    public async Task<List<Item>> GetAllItemsAsync()
    {
        try
        {
            await SQLConnection.InitAsync();
            return await SQLConnection.m_ConnectionAsync.Table<Item>().ToListAsync();
        }
        catch (Exception ex)
        {
            SQLConnection.StatusMessage = $"Failed to retrieve data. {ex.Message}";
        }

        return new List<Item>();
    }

    public async Task<List<Item>> GetSpecificItemsAsync(List<int> listItemIds)
    {
        try
        {
            await SQLConnection.InitAsync();
            return await SQLConnection.m_ConnectionAsync.Table<Item>().Where(item => listItemIds.Contains(item.Id)).ToListAsync();
        }
        catch (Exception ex)
        {
            SQLConnection.StatusMessage = $"Failed to retrieve data. {ex.Message}";
        }

        return new List<Item>();
    }

    public async Task<string> removeDiacritics(string text)
    {
        string result = string.Empty;

        await Task.Run(() =>
        {
            string formD = text.Normalize(NormalizationForm.FormD);
            StringBuilder sb = new StringBuilder();

            foreach (char ch in formD)
            {
                UnicodeCategory uc = CharUnicodeInfo.GetUnicodeCategory(ch);
                if (uc != UnicodeCategory.NonSpacingMark)
                {
                    sb.Append(ch);
                }
            }

            result = sb.ToString().Normalize(NormalizationForm.FormC);
        });

        return result;
    }

    public async Task<List<Item>> SearchItems(string filterText)
    {
        var listItem = await SQLConnection.m_ConnectionAsync.Table<Item>().Where(item => !string.IsNullOrEmpty(item.Name)).ToListAsync();

        listItem.ForEach(async (item) =>
        {
            string strWithoutDiac = await removeDiacritics(item.Name);

            if (strWithoutDiac.StartsWith(filterText, StringComparison.OrdinalIgnoreCase))
                FilteredItems.Add(item);
        });

        return FilteredItems;
    }

    public void ClearFilteredList()
    {
        FilteredItems.Clear();
    }
}