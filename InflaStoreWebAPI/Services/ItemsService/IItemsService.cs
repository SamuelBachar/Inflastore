namespace InflaStoreWebAPI.Services.ItemsService
{
    public interface IItemsService
    {
        public Task<List<Item>> GetAllItemsAsync();

        public Task<List<Item>> GetSpecificItemsAsync(List<int> listIds);
    }
}
