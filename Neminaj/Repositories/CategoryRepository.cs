using Neminaj.Constants;
using SharedTypesLibrary.DTOs.API;
using SharedTypesLibrary.Models.API.DatabaseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Neminaj.Repositories;

public class CategoryRepository
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly HttpClient _httpClient;
    private List<CategoryDTO> _listCategory { get; set; } = null;

    public CategoryRepository(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
        _httpClient = _httpClientFactory.CreateClient(AppConstants.HttpsClientName);
    }

    public async Task<List<CategoryDTO>> GetAllCategories()
    {
        if (_listCategory != null)
        {
            return _listCategory;
        }
        else
        {
            try
            {
                var response = await _httpClient.GetAsync("api/Category/GetAllCategories");

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();

                    if (!string.IsNullOrEmpty(content))
                    {
                        _listCategory = JsonSerializer.Deserialize<List<CategoryDTO>>(content, new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        });

                        return _listCategory;
                    }
                }
                else
                {
                    // TODO a to vsade !!!!!
                }

            }
            catch (Exception ex)
            {
                SQLConnection.StatusMessage = $"Chyba pri načítaní položiek zo servera. {ex.Message}";
            }
        }

        return new List<CategoryDTO>();
    }

    public async Task<List<CategoryDTO>> GetSpecificCategoriesAsync(List<int> listCategoryIds)
    {
        if (_listCategory != null)
        {
            return _listCategory.Where(category => listCategoryIds.Contains(category.Id)).ToList();
        }
        else
        {
            try
            {
                string strListIds = string.Join(",", listCategoryIds.Select(x => x.ToString()).ToArray());

                var response = await _httpClient.GetAsync($"api/Category/GetSpecificCategories?strListIds={strListIds}");

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();

                    return JsonSerializer.Deserialize<List<CategoryDTO>>(content, new JsonSerializerOptions
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

        return new List<CategoryDTO>();
    }
}
