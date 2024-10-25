﻿using Neminaj.Constants;
using SharedTypesLibrary.DTOs.API;
using System.Globalization;
using System.Text;
using System.Text.Json;

namespace Neminaj.Repositories;

public class CardData
{
    public string Name { get; set; }
    public byte[] CardImage { get; set; }

    public string CardImageUrl { get; set; }

    public bool IsKnownCard { get; set; }
}

public class ClubCardRepository : ParentRepository<CardData>
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly HttpClient _httpClient;
    private List<ClubCardDTO> _listClubCard { get; set; } = null;

    public ClubCardRepository(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
        _httpClient = _httpClientFactory.CreateClient(AppConstants.HttpsClientName);
    }

    public async Task<List<ClubCardDTO>> GetAllClubCards()
    {
        if (_listClubCard != null && !base.GetUpdatedNeeded())
        {
            return _listClubCard;
        }
        else
        {
            try
            {
                var response = await _httpClient.GetAsync("api/ClubCard/GetAllClubCards");

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();

                    if (!string.IsNullOrEmpty(content))
                    {
                        _listClubCard = JsonSerializer.Deserialize<List<ClubCardDTO>>(content, new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        });

                        return _listClubCard;
                    }
                }

            }
            catch (Exception ex)
            {
                SQLConnection.StatusMessage = $"Chyba pri načítaní položiek zo servera. {ex.Message}";
            }
        }

        return new List<ClubCardDTO>();
    }

    public async Task<List<ClubCardDTO>> GetSpecificClubCardAsync(List<int> listCompaniesIds)
    {
        if (_listClubCard != null && !base.GetUpdatedNeeded())
        {
            return _listClubCard.Where(card => listCompaniesIds.Contains(card.Company_Id)).ToList();
        }
        else
        {
            try
            {
                string strListIds = string.Join(",", listCompaniesIds.Select(x => x.ToString()).ToArray());

                var response = await _httpClient.GetAsync($"api/ClubCard/GetSpecificClubCard?strListIds={strListIds}");

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();

                    return JsonSerializer.Deserialize<List<ClubCardDTO>>(content, new JsonSerializerOptions
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

        return new List<ClubCardDTO>();
    }

    public async Task<List<CardData>> SearchItems(List<CardData> listCardDatas, string filterText)
    {
        await Task.Run(() =>
        {
            listCardDatas.ForEach(async (card) =>
            {
                string strWithoutDiac = await base.RemoveDiacritics(card.Name);

                if (strWithoutDiac.StartsWith(filterText, StringComparison.OrdinalIgnoreCase))
                    FilteredItems.Add(card);
            });
        });

        return FilteredItems;
    }
}
