using CommunityToolkit.Mvvm.ComponentModel;
using Neminaj.ContentViews;
using Neminaj.Repositories;
using Neminaj.ViewsModels;
using SharedTypesLibrary.DTOs.API;
using System.Collections.ObjectModel;

namespace Neminaj.Views;

public partial class SubCategoryView : ContentPage
{
    public ObservableCollection<CategoryDTO> _categories { get; set; } = new();

    List<CategoryDTO> _listAllCategories { get; set; } = new();

    CategoryRepository _categoryRepo;
    ItemRepository _itemRepo { get; set; } = null;

    UnitRepository _unitRepo { get; set; } = null;

    SavedCartRepository _savedCartRepo { get; set; } = null;

    CompanyRepository _companyRepo { get; set; } = null;

    ItemPriceRepository _itemPriceRepo { get; set; } = null;

    bool AppDataLoaded { get; set; } = false;

    CategoryDTO _choosenCategory { get; set; }


    PopUpActivityIndicator _popUpIndic = new PopUpActivityIndicator("Načítavam polôžky, ceny a obchody ...");

    SubCategoryViewModel _subCategoryViewModel { get; set; } = null;

    public SubCategoryView(SubCategoryViewModel subCategoryViewModel)
    {
        InitializeComponent();

        _subCategoryViewModel = subCategoryViewModel;
        this.BindingContext = _subCategoryViewModel;

        this.Appearing += (s, e) => { this.Content = _popUpIndic; };
    }

    protected override async void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);

        if (!AppDataLoaded)
        {
            _itemRepo = _subCategoryViewModel.itemRepo;
            _unitRepo = _subCategoryViewModel.unitRepo;
            _savedCartRepo = _subCategoryViewModel.cartRepo;
            _categoryRepo = _subCategoryViewModel.categoryRepo;
            _companyRepo = _subCategoryViewModel.companyRepo;
            _itemPriceRepo = _subCategoryViewModel.itemPriceRepo;
            _choosenCategory = _subCategoryViewModel.choosenCategory;

            AppDataLoaded = true;
            _popUpIndic.SetLblPopUp("Načítavam dáta");
        }

        if (_categories.Count == 0)
        {
            _listAllCategories = await _categoryRepo.GetAllCategories();

            if (this.SubCategoriesCollectionView.SelectedItems.Count != 0)
                this.SubCategoriesCollectionView.SelectedItems.Clear();
        }

        //CartCounterControlView.Init(_savedCartRepo, ItemPicker.ObservableItemsChoosed);

        _categories = new ObservableCollection<CategoryDTO>(_listAllCategories.Where(cat => cat.ParentId == _choosenCategory.Id).ToList());
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