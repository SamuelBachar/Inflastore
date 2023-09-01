namespace InflaStoreWebAPI.Services.ItemPriceService
{
    public interface IItemPriceService
    {
        public Task<List<ItemPrice>> GetAllItemsPricesAsync();

        public Task<List<ItemPrice>> GetSpecificItemsPricesAsync(List<int> listCompaniesIds, List<int> listItemsIds);
    }
}
