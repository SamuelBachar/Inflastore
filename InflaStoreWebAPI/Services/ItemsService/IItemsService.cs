namespace InflaStoreWebAPI.Services.ItemsService
{
    public interface IItemsService
    {
        public Task<List<Item>> GetAllItemsAsync();
    }
}
