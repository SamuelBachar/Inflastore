using Neminaj.ContentViews;
using Neminaj.Repositories;
using SharedTypesLibrary.DTOs.API;
using SharedTypesLibrary.Models.API.DatabaseModels;
using System.Collections.ObjectModel;

namespace Neminaj.Views;

public partial class CategoryPickerView : ContentPage
{
    private readonly CategoryRepository _categoryRepository;
    public ObservableCollection<CategoryDTO> _categories { get; set; } = new();

    ItemRepository _itemRepo { get; set; } = null;

    UnitRepository _unitRepo { get; set; } = null;

    SavedCartRepository _savedCartRepo { get; set; } = null;

    PopUpActivityIndicator _popUpIndic = new PopUpActivityIndicator("Načítavam polôžky ...");

    public CategoryPickerView(ItemRepository itemRepo, UnitRepository unitRepo, SavedCartRepository cartRepo, CategoryRepository categoryRepository)
    {
        InitializeComponent();

        _categoryRepository = categoryRepository;
        this.BindingContext = this;

        _itemRepo = itemRepo;
        _unitRepo = unitRepo;
        _savedCartRepo = cartRepo;

        this.Appearing += (s, e) => { this.Content = _popUpIndic; };
    }

    protected override async void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);

        if (_categories.Count == 0)
        {
            foreach (CategoryDTO category in await _categoryRepository.GetAllCategories())
            {
                _categories.Add(category);
            }

            if (this.CategoriesCollectionView.SelectedItems.Count != 0)
                this.CategoriesCollectionView.SelectedItems.Clear();
        }

        CartCounterControlView.Init(_savedCartRepo, ItemPicker.ObservableItemsChoosed);

        this.Content = this.MainControlWrapper;
    }

    private async void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
        if (e.Parameter is CategoryDTO category)
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