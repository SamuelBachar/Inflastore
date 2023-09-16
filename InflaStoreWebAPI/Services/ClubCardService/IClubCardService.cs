using SharedTypesLibrary.Models.API.DatabaseModels;

namespace InflaStoreWebAPI.Services.ClubCardService;

public interface IClubCardService
{
    public Task<List<ClubCardDTO>> GetAllClubCardAsync();

    public Task<List<ClubCardDTO>> GetSpecificClubCardAsync(List<int> listIds);
}
