using Neminaj.ContentViews;
using Neminaj.Repositories;
using SharedTypesLibrary.DTOs.API;
using System.Collections.ObjectModel;

namespace Neminaj.Views;

public partial class SubCategoryView : ContentPage
{
    public ObservableCollection<CategoryDTO> _categories { get; set; } = new();

    List<CategoryDTO> _listAllCategories { get; set; } = new();

    private readonly CategoryRepository _categoryRepo;
    ItemRepository _itemRepo { get; set; } = null;

    UnitRepository _unitRepo { get; set; } = null;

    SavedCartRepository _savedCartRepo { get; set; } = null;

    CompanyRepository _companyRepo { get; set; } = null;

    ItemPriceRepository _itemPriceRepo { get; set; } = null;

    bool AppDataLoaded { get; set; } = false;

    CategoryDTO _choosenCategory { get; set; }


    PopUpActivityIndicator _popUpIndic = new PopUpActivityIndicator("Načítavam polôžky, ceny a obchody ...");

    public SubCategoryView(ItemRepository itemRepo, UnitRepository unitRepo, SavedCartRepository cartRepo, CategoryRepository categoryRepository,
                           CompanyRepository companyRepository, ItemPriceRepository itemPriceRepository, CategoryDTO choosenCategory)
    {
        InitializeComponent();

        _categoryRepo = categoryRepository;
        this.BindingContext = this;

        _itemRepo = itemRepo;
        _unitRepo = unitRepo;
        _savedCartRepo = cartRepo;
        _companyRepo = companyRepository;
        _itemPriceRepo = itemPriceRepository;
        _choosenCategory = choosenCategory;

        this.Appearing += (s, e) => { this.Content = _popUpIndic; };
    }

    protected override async void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);

        if (_categories.Count == 0)
        {
            _listAllCategories = await _categoryRepo.GetAllCategories();

            if (this.SubCategoriesCollectionView.SelectedItems.Count != 0)
                this.SubCategoriesCollectionView.SelectedItems.Clear();
        }

        //CartCounterControlView.Init(_savedCartRepo, ItemPicker.ObservableItemsChoosed);

        if (!AppDataLoaded)
        {
            AppDataLoaded = true;
            _popUpIndic.SetLblPopUp("Načítavam dáta");
        }

        _categories = new ObservableCollection<CategoryDTO>(_listAllCategories.Where(cat => cat.ParentId == _choosenCategory).ToList());
        this.SubCategoriesCollectionView.ItemsSource = _categories;

        this.Content = this.MainControlWrapper;
    }

    private async void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
        if (e.Parameter is CategoryDTO category)
        {

            if (category.ParentId != null && _listAllCategories.All(cat => cat.ParentId != category.Id))
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
            else
            {
                await Shell.Current.GoToAsync(nameof(SubCategoryView),
                new Dictionary<string, object>
                {
                    ["ItemRepo"] = _itemRepo,
                    ["UnitRepo"] = _unitRepo,
                    ["CartRepo"] = _savedCartRepo,
                    ["CategoryRepo"] = _categoryRepo,
                    ["CompanyRepo"] = _companyRepo,
                    ["ItemPriceRepo"] = _itemPriceRepo,
                    ["ChoosenCategory"] = category
                });
            }
        }
    }
}