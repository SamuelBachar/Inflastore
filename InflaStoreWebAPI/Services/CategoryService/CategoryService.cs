using SharedTypesLibrary.Models.API.DatabaseModels;

namespace InflaStoreWebAPI.Services.CategoryService;
public class CategoryService : ICategoryService
{
    private readonly DataContext _context;

    public CategoryService(DataContext context)
    {
        _context = context;
    }

    public async Task<List<CategoryDTO>> GetAllCategoriesAsync()
    {
        List<CategoryDTO> listCategoriesDTOs = new List<CategoryDTO>();
        List<Category> listCategories = await _context.Categories.ToListAsync();

        foreach (Category category in listCategories)
        {
            listCategoriesDTOs.Add( new CategoryDTO
            {
                Id = category.Id,
                Name = category.Name,
                ImageUrl = @$"https://inflastoreapi.azurewebsites.net/StaticFile/Categories/{category.Path}",
            });
        }

        return listCategoriesDTOs;
    }

    public async Task<List<CategoryDTO>> GetSpecificCategoriesAsync(List<int> listIds)
    {
        List<CategoryDTO> listCategoriesDTOs = new List<CategoryDTO>();
        List<Category> listClubCard = await _context.Categories.Where(category => listIds.Contains(category.Id)).ToListAsync();

        foreach (Category category in listClubCard)
        {
            listCategoriesDTOs.Add(new CategoryDTO
            {
                Id = category.Id,
                Name = category.Name,
                ImageUrl = @$"https://inflastoreapi.azurewebsites.net/StaticFile/Categories/{category.Path}",
            });
        }

        return listCategoriesDTOs;
    }
}