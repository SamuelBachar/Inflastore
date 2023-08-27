using AutoMapper;

namespace InflaStoreWebAPI.Services.ItemsService
{
    public class ItemsService : IItemsService
    {
        private readonly DataContext _context;

        public ItemsService(DataContext context)
        {
            _context = context;
        }

        public async Task<List<Item>> GetAllItemsAsync()
        {
            return await _context.Items.ToListAsync();
        }
    }
}
