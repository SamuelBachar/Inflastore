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

public class NavigationShopRepository : ParentRepository<NavigationShopData>
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly HttpClient _httpClient;

    private List<NavigationShopData> _listNavigShopData { get; set; } = null;

    public NavigationShopRepository(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
        _httpClient = _httpClientFactory.CreateClient(AppConstants.HttpsClientName);
    }

    public async Task<List<NavigationShopData>> GetAllNavigShopDataAsync()
    {
        if (_listNavigShopData != null && !base.GetUpdatedNeeded())
        {
            return _listNavigShopData;
        }
        else
        {
            try
            {
                var response = await _httpClient.GetAsync("api/NavigationShopData/GetAllNavigationShopData");

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();

                    if (!string.IsNullOrEmpty(content))
                    {
                        _listNavigShopData = JsonSerializer.Deserialize<List<NavigationShopData>>(content, new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        });

                        return _listNavigShopData;
                    }
                }

            }
            catch (Exception ex)
            {
                SQLConnection.StatusMessage = $"Chyba pri načítaní položiek zo servera. {ex.Message}";
            }
        }

        return new List<NavigationShopData>();

    }

    public async Task<List<NavigationShopData>> GetSpecificCompaniesNavigShopsDataAsync(List<int> listCompaniesIds)
    {
        if (_listNavigShopData != null && !base.GetUpdatedNeeded())
        {
            return _listNavigShopData.Where(navigShopData => listCompaniesIds.Contains(navigShopData.Company_Id)).ToList();
        }
        else
        {
            try
            {
                string strListIds = string.Join(",", listCompaniesIds.Select(x => x.ToString()).ToArray());

                var response = await _httpClient.GetAsync($"api/NavigationShopData/GetSpecificCompaniesNavigationShopData?strListIds={strListIds}");

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();

                    return JsonSerializer.Deserialize<List<NavigationShopData>>(content, new JsonSerializerOptions
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

        return new List<NavigationShopData>();
    }

}