using SharedTypesLibrary.Models.API.DatabaseModels;

namespace InflaStoreWebAPI.Services.UnitsService
{
    public interface IUnitsService
    {
        public Task<List<Unit>> GetAllUnitsAsync();

        public Task<List<Unit>> GetSpecificUnitsAsync(List<int> listIds);
    }
}
