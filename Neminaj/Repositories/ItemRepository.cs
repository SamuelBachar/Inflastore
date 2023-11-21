using System.Globalization;
using System.Text;
using System.Text.Json;
using Neminaj.Constants;
using Item = SharedTypesLibrary.Models.API.DatabaseModels.Item;

namespace Neminaj.Repositories;

public class ItemRepository
{
    List<Item> FilteredItems = new List<Item>();

    private readonly IHttpClientFactory _httpClientFactory;
    private readonly HttpClient _httpClient;

    private List<Item> _listItem { get; set; } = null;

    public ItemRepository(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
        _httpClient = _httpClientFactory.CreateClient(AppConstants.HttpsClientName);
    }

    public async Task<List<Item>> GetAllItemsAsync()
    {
        if (_listItem != null)
        {
            return _listItem;
        }
        else
        {
            try
            {
                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {App.UserSessionInfo.JWT}");
                var response = await _httpClient.GetAsync("api/Items/GetAllItems");

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();

                    if (!string.IsNullOrEmpty(content))
                    {
                        _listItem = JsonSerializer.Deserialize<List<Item>>(content, new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        });

                        return _listItem;
                    }
                }
                else
                {
                    // todo display allertt
                }

            }
            catch (Exception ex)
            {
                SQLConnection.StatusMessage = $"Chyba pri načítaní položiek zo servera. {ex.Message}";
            }
        }

        return new List<Item>();
    }

    public async Task<List<Item>> GetSpecificItemsAsync(List<int> listItemIds)
    {
        if (_listItem != null)
        {
            return _listItem.Where(item => listItemIds.Contains(item.Id)).ToList();
        }
        else
        {
            try
            {
                string strListIds = string.Join(",", listItemIds.Select(x => x.ToString()).ToArray());

                var response = await _httpClient.GetAsync($"api/Items/GetSpecificItems?strListIds={strListIds}");

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();

                    return JsonSerializer.Deserialize<List<Item>>(content, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                }

            }
            catch (Exception ex)
            {
                SQLConnection.StatusMessage = $"Chyba pri načítaní položiek zo servera. {ex.Message}";
            }
        }

        return new List<Item>();
    }

    public async Task<List<Item>> SearchItems(string filterText)
    {
        List<Item> listItem = null;

        try
        {
            var response = await _httpClient.GetAsync("api/Items/GetAllItems");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();

                if (!string.IsNullOrEmpty(content))
                {
                    listItem = JsonSerializer.Deserialize<List<Item>>(content, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                }
            }

        }
        catch (Exception ex)
        {
            SQLConnection.StatusMessage = $"Chyba pri načítaní položiek zo servera. {ex.Message}";
        }

        listItem.ForEach(async (item) =>
        {
            string strWithoutDiac = await removeDiacritics(item.Name);

            if (strWithoutDiac.StartsWith(filterText, StringComparison.OrdinalIgnoreCase))
                FilteredItems.Add(item);
        });

        return FilteredItems;
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

    public void ClearFilteredList()
    {
        FilteredItems.Clear();
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
}
