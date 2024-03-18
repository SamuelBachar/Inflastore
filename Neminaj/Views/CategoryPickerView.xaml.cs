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

    CompanyRepository _companyRepo { get; set; } = null;

    ItemPriceRepository _itemPriceRepo { get; set; } = null;

    bool AppDataLoaded { get; set; } = false;

    PopUpActivityIndicator _popUpIndic = new PopUpActivityIndicator("Načítavam polôžky, ceny a obchody ...");

    public CategoryPickerView(ItemRepository itemRepo, UnitRepository unitRepo, SavedCartRepository cartRepo, CategoryRepository categoryRepository, CompanyRepository companyRepository, ItemPriceRepository itemPriceRepository)
    {
        InitializeComponent();

        _categoryRepository = categoryRepository;
        this.BindingContext = this;

        _itemRepo = itemRepo;
        _unitRepo = unitRepo;
        _savedCartRepo = cartRepo;
        _companyRepo = companyRepository;
        _itemPriceRepo = itemPriceRepository;

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

        if (!AppDataLoaded)
        {
            await _itemRepo.GetAllItemsAsync();
            await _itemPriceRepo.GetAllItemPricesAsync();
            await _unitRepo.GetAllUnitsAsync();
            await _companyRepo.GetAllCompaniesAsync();

            AppDataLoaded = true;
            _popUpIndic.SetLblPopUp("Načítavam dáta");
        }


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
                ["CartRepo"] = _savedCartRepo,
                ["CompanyRepo"] = _companyRepo,
                ["ItemPriceRepo"] = _itemPriceRepo
            });
        }
    }
}