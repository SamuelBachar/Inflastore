using Neminaj.Repositories;
using SharedTypesLibrary.DTOs.API;
using SharedTypesLibrary.Models.API.DatabaseModels;
using System.Collections.ObjectModel;

namespace Neminaj.Views;

public partial class CategoryPickerView : ContentPage
{
	private readonly CategoryRepository _categoryRepository;
	public ObservableCollection<CategoryDTO> _categories { get; set; } = new ();

	ItemRepository _itemRepo { get; set; } = null;

    UnitRepository _unitRepo { get; set; } = null;

    SavedCartRepository _savedCartRepo { get; set; } = null;


    public CategoryPickerView(ItemRepository itemRepo, UnitRepository unitRepo, SavedCartRepository cartRepo, CategoryRepository categoryRepository)
	{
		InitializeComponent();

		_categoryRepository = categoryRepository;
		this.BindingContext = this;

        _itemRepo = itemRepo;
        _unitRepo = unitRepo;
        _savedCartRepo = cartRepo;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        _categories.Clear();

		foreach (CategoryDTO category in await _categoryRepository.GetAllCategories())
		{
			_categories.Add(category);
		}

        if (this.CategoriesCollectionView.SelectedItems.Count != 0)
            this.CategoriesCollectionView.SelectedItems.Clear();

        // todo: tu som skoncil https://youtu.be/FP_ZvUGcumg?t=692
    }

    private async void CategoriesCollectionView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
		if (e.CurrentSelection?[0] is CategoryDTO category)
		{
			await Shell.Current.GoToAsync(nameof(ItemPicker),
            new Dictionary<string, object>
            {
                ["Category"] = category,
				["ItemRepo"] = _itemRepo,
                ["UnitRepo"] = _unitRepo,
                ["CartRepo"] = _savedCartRepo
            });
        }
    }
}