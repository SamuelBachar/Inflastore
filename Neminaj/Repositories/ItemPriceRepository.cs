using Neminaj.Constants;
using Neminaj.Models;
using SharedTypesLibrary.Models.API.DatabaseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Neminaj.Repositories;

public class ItemPriceRepository
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly HttpClient _httpClient;
    private List<ItemPrice> _listItemsPrices { get; set; } = null;

    public ItemPriceRepository(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
        _httpClient = _httpClientFactory.CreateClient(AppConstants.HttpsClientName);
    }


    public async Task<List<ItemPrice>> GetAllItemPricesAsync()
    {
        if (_listItemsPrices != null)
        {
            return _listItemsPrices;
        }
        else
        {
            try
            {
                var response = await _httpClient.GetAsync("api/ItemsPrices/GetAllItemsPrices");

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();

                    if (!string.IsNullOrEmpty(content))
                    {
                        _listItemsPrices = JsonSerializer.Deserialize<List<ItemPrice>>(content, new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        });

                        return _listItemsPrices;
                    }
                }

            }
            catch (Exception ex)
            {
                SQLConnection.StatusMessage = $"Chyba pri načítaní položiek zo servera. {ex.Message}";
            }
        }

        return new List<ItemPrice>();
    }

    public async Task<List<ItemPrice>> GetPriceItemsFilteredAsync(List<int> listCompIds, List<int> listItemIds)
    {
        if (_listItemsPrices != null)
        {
            return _listItemsPrices.Where(item => listItemIds.Contains(item.Item_Id) && listCompIds.Contains(item.Company_Id)).ToList();
        }
        else
        {
            try
            {
                var httpClient = _httpClientFactory.CreateClient(AppConstants.HttpsClientName);

                var response = await _httpClient.GetAsync(string.Format("api/ItemsPrices/GetSpecificItemsPrices?strListCompaniesIds={0}&strListItemIds={1}",
                                                          string.Join(",", listCompIds),
                                                          string.Join(",", listItemIds)));

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();

                    return JsonSerializer.Deserialize<List<ItemPrice>>(content, new JsonSerializerOptions
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

        return new List<ItemPrice>();
    }

}
