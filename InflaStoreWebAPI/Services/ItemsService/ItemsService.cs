using AutoMapper;
using Microsoft.AspNetCore.OutputCaching;

namespace InflaStoreWebAPI.Services.ItemsService
{
    public class ItemsService : IItemsService
    {
        private readonly DataContext _context;

        private List<Item> _listItems = new List<Item>();

        public ItemsService(DataContext context)
        {
            _context = context;
        }

        public async Task<List<Item>> GetAllItemsAsync()
        {
            return await _context.Items.ToListAsync();
        }

        public async Task<List<Item>> GetSpecificItemsAsync(List<int> listIds)
        {
            return await _context.Items.Where(item => listIds.Contains(item.Id)).ToListAsync();
        }
    }
}
