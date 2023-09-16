using SharedTypesLibrary.Models.API.DatabaseModels;

namespace InflaStoreWebAPI.Services.ClubCardService;
public class ClubCardService : IClubCardService
{
    private readonly DataContext _context;

    public ClubCardService(DataContext context)
    {
        _context = context;
    }

    public async Task<List<ClubCardDTO>> GetAllClubCardAsync()
    {
        List<ClubCardDTO> listClubCardDTOs = new List<ClubCardDTO>();
        List<ClubCard> listClubCard = await _context.ClubCards.ToListAsync();

        foreach (ClubCard clubCard in listClubCard)
        {
            string imgPath = Path.Combine(Directory.GetCurrentDirectory(), $"wwwroot\\StaticFile\\Companies\\ClubCard\\{clubCard.Path}");

            listClubCardDTOs.Add( new ClubCardDTO
            {
                Company_Id = clubCard.Company_Id,
                Name = clubCard.Name,
                Url = @$"https://inflastoreapi.azurewebsites.net/StaticFile/Companies/ClubCard/{clubCard.Path}",
                Image = await System.IO.File.ReadAllBytesAsync(imgPath)
            });
        }

        return listClubCardDTOs;
    }

    public async Task<List<ClubCardDTO>> GetSpecificClubCardAsync(List<int> listIds)
    {
        List<ClubCardDTO> listClubCardDTOs = new List<ClubCardDTO>();
        List<ClubCard> listClubCard = await _context.ClubCards.Where(company => listIds.Contains(company.Id)).ToListAsync();

        foreach (ClubCard clubCard in listClubCard)
        {
            string imgPath = Path.Combine(Directory.GetCurrentDirectory(), $"wwwroot\\StaticFile\\Companies\\ClubCard\\{clubCard.Path}");

            listClubCardDTOs.Add(new ClubCardDTO
            {
                Company_Id = clubCard.Company_Id,
                Name = clubCard.Name,
                Url = @$"https://inflastoreapi.azurewebsites.net/StaticFile/Companies/ClubCard/{clubCard.Path}",
                Image = await System.IO.File.ReadAllBytesAsync(imgPath)
            });
        }

        return listClubCardDTOs;
    }
}