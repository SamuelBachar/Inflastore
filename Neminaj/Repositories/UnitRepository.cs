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

public class UnitRepository : ParentRepository<Unit>
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly HttpClient _httpClient;

    private List<Unit> _listUnits { get; set; } = null;

    public UnitRepository(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
        _httpClient = _httpClientFactory.CreateClient(AppConstants.HttpsClientName);
    }

    public async Task<List<Unit>> GetAllUnitsAsync()
    {
        if (_listUnits != null && !base.GetUpdatedNeeded())
        {
            return _listUnits;
        }
        else
        {

            try
            {
                var response = await _httpClient.GetAsync("api/Units/GetAllUnits");

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();

                    if (!string.IsNullOrEmpty(content))
                    {
                        _listUnits = JsonSerializer.Deserialize<List<Unit>>(content, new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        });

                        return _listUnits;
                    }
                }

            }
            catch (Exception ex)
            {
                SQLConnection.StatusMessage = $"Chyba pri načítaní položiek zo servera. {ex.Message}";
            }
        }

        return new List<Unit>();
    }

    public async Task<List<Unit>> GetSpecificUnits(List<int> listUnitIds)
    {
        if (_listUnits != null && !base.GetUpdatedNeeded())
        {
            return _listUnits.Where(unit => listUnitIds.Contains(unit.Id)).ToList();
        }
        else
        {
            try
            {
                string strListIds = string.Join(",", listUnitIds.Select(x => x.ToString()).ToArray());

                var response = await _httpClient.GetAsync($"api/Units/GetSpecificUnits?strListIds={strListIds}");

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();

                    return JsonSerializer.Deserialize<List<Unit>>(content, new JsonSerializerOptions
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

        return new List<Unit>();
    }
}
