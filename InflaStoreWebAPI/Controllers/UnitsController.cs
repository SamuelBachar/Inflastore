using InflaStoreWebAPI.Services.ItemsService;
using InflaStoreWebAPI.Services.UnitsService;
using Microsoft.AspNetCore.Mvc;

namespace InflaStoreWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UnitsController : ControllerBase
    {
        private readonly IUnitsService _unitsService;

        public UnitsController(IUnitsService unitsService)
        {
            _unitsService = unitsService;
        }

        [HttpGet("GetAllUnits")]
        public async Task<List<Unit>> GetAllUnitsAsync()
        {
            return await _unitsService.GetAllUnitsAsync();
        }

        [HttpGet("GetSpecificUnits")]
        public async Task<List<Unit>> GetSpecificUnitsAsync(string strListIds)
        {
            var separated = strListIds.Split(new char[] { ',' });
            List<int> listIds = separated.Select(s => int.Parse(s)).ToList();

            return await _unitsService.GetSpecificUnitsAsync(listIds);
        }
    }
}
