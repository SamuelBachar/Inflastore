using InflaStoreWebAPI.Services.ItemsService;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;

namespace InflaStoreWebAPI.Controllers
{
    //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    [ApiController]
    [OutputCache(Duration = 60 * 60 * 12)]
    public class ItemsController : ControllerBase
    {
        private readonly IItemsService _itemsService;

        public ItemsController(IItemsService itemsService)
        {
            _itemsService = itemsService;
        }

        [HttpGet("GetAllItems")]
        public async Task<List<Item>> GetAllItemsAsync()
        {
            return await _itemsService.GetAllItemsAsync();
        }

        [HttpGet("GetSpecificItems")]
        public async Task<List<Item>> GetSpecificItemsAsync(string strListIds)
        {
            var separated = strListIds.Split(new char[] { ',' });
            List<int> listIds = separated.Select(s => int.Parse(s)).ToList();

            return await _itemsService.GetSpecificItemsAsync(listIds);
        }
    }
}
