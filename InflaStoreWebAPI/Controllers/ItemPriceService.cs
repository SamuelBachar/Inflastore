using InflaStoreWebAPI.Services.ItemPriceService;
using Microsoft.AspNetCore.Mvc;

namespace InflaStoreWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ItemsPricesController : ControllerBase
    {
        private readonly IItemPriceService _itemPriceService;

        public ItemsPricesController(IItemPriceService itemPriceService)
        {
            _itemPriceService = itemPriceService;
        }

        [HttpGet("GetAllItemsPrices")]
        public async Task<List<ItemPrice>> GetAllItemsPricesAsync()
        {
            return await _itemPriceService.GetAllItemsPricesAsync();
        }

        [HttpGet("GetSpecificItemsPrices")]
        public async Task<List<ItemPrice>> GetSpecificItemsPricesAsync(string strListCompaniesIds, string strListItemIds)
        {
            var separated = strListCompaniesIds.Split(new char[] { ',' });
            List<int> listCompaniesIds = separated.Select(s => int.Parse(s)).ToList();

            separated = strListItemIds.Split(new char[] { ',' });
            List<int> listItemIds = separated.Select(s => int.Parse(s)).ToList();

            return await _itemPriceService.GetSpecificItemsPricesAsync(listCompaniesIds, listItemIds);
        }
    }
}
