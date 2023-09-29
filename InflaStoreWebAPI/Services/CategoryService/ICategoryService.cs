using SharedTypesLibrary.Models.API.DatabaseModels;

namespace InflaStoreWebAPI.Services.CategoryService;

public interface ICategoryService
{
    public Task<List<CategoryDTO>> GetAllCategoriesAsync();

    public Task<List<CategoryDTO>> GetSpecificCategoriesAsync(List<int> listIds);
}
