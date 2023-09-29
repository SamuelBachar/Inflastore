using InflaStoreWebAPI.Services.ClubCardService;
using InflaStoreWebAPI.Services.CompanyService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace InflaStoreWebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ClubCardController : ControllerBase
{
    private readonly IClubCardService _clubCardService;

    public ClubCardController(IClubCardService clubCardService)
    {
        _clubCardService = clubCardService;
    }

    [HttpGet("GetAllClubCards")]
    public async Task<List<ClubCardDTO>> GetAllClubCardAsync()
    {
        return await _clubCardService.GetAllClubCardAsync();
    }

    [HttpGet("GetSpecificClubCard")]
    public async Task<List<ClubCardDTO>> GetSpecificClubCardAsync(string strListIds)
    {
        var separated = strListIds.Split(new char[] { ',' });
        List<int> listIds = separated.Select(s => int.Parse(s)).ToList();

        return await _clubCardService.GetSpecificClubCardAsync(listIds);
    }
}
