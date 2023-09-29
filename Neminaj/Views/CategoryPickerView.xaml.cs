using Neminaj.Repositories;
using SharedTypesLibrary.DTOs.API;
using SharedTypesLibrary.Models.API.DatabaseModels;
using System.Collections.ObjectModel;

namespace Neminaj.Views;

public partial class CategoryPickerView : ContentPage
{
	private readonly CategoryRepository _categoryRepository;
	public ObservableCollection<CategoryDTO> _categories { get; set; } = null;
	
	public CategoryPickerView(CategoryRepository categoryRepository)
	{
		InitializeComponent();

		_categoryRepository = categoryRepository;
		this.BindingContext = this;
	}

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        _categories.Clear();

		foreach (CategoryDTO category in await _categoryRepository.GetAllCategories())
		{
            _categories.Add(category);
		}

        // todo: tu som skoncil https://youtu.be/FP_ZvUGcumg?t=692
    }

}