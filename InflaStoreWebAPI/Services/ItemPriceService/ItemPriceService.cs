using AutoMapper;
using System.Linq;

namespace InflaStoreWebAPI.Services.ItemPriceService
{
    public class ItemPriceService : IItemPriceService
    {
        private readonly DataContext _context;

        public ItemPriceService(DataContext context)
        {
            _context = context;
        }

        public async Task<List<ItemPrice>> GetAllItemsPricesAsync()
        {
            return await _context.ItemsPrices.ToListAsync();
        }

        public async Task<List<ItemPrice>> GetSpecificItemsPricesAsync(List<int> listCompaniesIds, List<int> listItemsIds)
        {
            return await _context.ItemsPrices.Where(itemPrice => listCompaniesIds.Contains(itemPrice.Company_Id) && listItemsIds.Contains(itemPrice.Item_Id)).ToListAsync();
        }
    }
}
