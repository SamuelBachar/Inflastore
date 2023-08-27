using InflaStoreWebAPI.Services.ItemsService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace InflaStoreWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ItemsController : ControllerBase
    {
        private readonly IItemsService _itemsService;

        public ItemsController(IItemsService itemsService)
        {
            _itemsService = itemsService;
        }

        [HttpPost("GetAllItems")]
        private async Task<List<Item>> GetAllItemsAsync()
        {
            return await _itemsService.GetAllItemsAsync();
        }
    }
}
