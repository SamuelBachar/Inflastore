using InflaStoreWebAPI.Services.CategoryService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace InflaStoreWebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CategoryController : ControllerBase
{
    private readonly ICategoryService _categoryService;

    public CategoryController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    [HttpGet("GetAllCategories")]
    public async Task<List<CategoryDTO>> GetAllCategoriesAsync()
    {
        return await _categoryService.GetAllCategoriesAsync();
    }

    [HttpGet("GetSpecificCategories")]
    public async Task<List<CategoryDTO>> GetSpecificCategoriesAsync(string strListIds)
    {
        var separated = strListIds.Split(new char[] { ',' });
        List<int> listIds = separated.Select(s => int.Parse(s)).ToList();

        return await _categoryService.GetSpecificCategoriesAsync(listIds);
    }
}
