using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Neminaj.Constants;
using Neminaj.Models;
using SharedTypesLibrary.DTOs.API;
using SharedTypesLibrary.Models.API.DatabaseModels;

namespace Neminaj.Repositories;

public class CompanyRepository
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly HttpClient _httpClient;
    private List<CompanyDTO> _listCompanies { get; set; } = null;

    public CompanyRepository(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
        _httpClient = _httpClientFactory.CreateClient(AppConstants.HttpsClientName);
    }

    public async Task<List<CompanyDTO>> GetAllCompaniesAsync()
    {
        if (_listCompanies != null)
        {
            return _listCompanies;
        }
        else
        {
            try
            {
                var response = await _httpClient.GetAsync("api/Company/GetAllCompanies");

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();

                    if (!string.IsNullOrEmpty(content))
                    {
                        _listCompanies = JsonSerializer.Deserialize<List<CompanyDTO>>(content, new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        });

                        return _listCompanies;
                    }
                }

            }
            catch (Exception ex)
            {
                SQLConnection.StatusMessage = $"Chyba pri načítaní položiek zo servera. {ex.Message}";
            }
        }

        return new List<CompanyDTO>();
    }

    public async Task<List<CompanyDTO>> GetSpecificCompaniesAsync(List<int> listCompaniesIds)
    {
        if (_listCompanies != null)
        {
            return _listCompanies.Where(item => listCompaniesIds.Contains(item.Id)).ToList();
        }
        else
        {
            try
            {
                string strListIds = string.Join(",", listCompaniesIds.Select(x => x.ToString()).ToArray());

                var response = await _httpClient.GetAsync($"api/Company/GetSpecificCompanies?strListIds={strListIds}");

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();

                    return JsonSerializer.Deserialize<List<CompanyDTO>>(content, new JsonSerializerOptions
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

        return new List<CompanyDTO>();
    }
}
