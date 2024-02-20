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
            List<Item> listItems = await _context.Items.ToListAsync();

            foreach (Item item in listItems)
            {
                item.ImageUrl = @$"https://inflastoreapi.azurewebsites.net/StaticFile/Items/{item.Category_Id}/{item.Path}";
            }

            return listItems;
        }

        public async Task<List<Item>> GetSpecificItemsAsync(List<int> listIds)
        {
            List<Item> listItem = await _context.Items.Where(item => listIds.Contains(item.Id)).ToListAsync();

            listItem.ForEach(item => item.ImageUrl = @$"https://inflastoreapi.azurewebsites.net/StaticFile/Items/{item.Category_Id}/{item.Path}");
            return listItem;
        }
    }
}
