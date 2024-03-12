using CommunityToolkit.Maui.Views;
using Neminaj.ContentViews;
using Neminaj.Models;
using Neminaj.Repositories;
using Neminaj.ViewsModels;
using SharedTypesLibrary.DTOs.API;
using SharedTypesLibrary.Models.API.DatabaseModels;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace Neminaj.Views;

public class PricesPerCompany
{
    public string CompanyImgUrl { get; set; }

    public string Price { get; set; }

    public string PriceDiscount { get; set; }
}

public class ItemPickerModel : Item
{
    public List<PricesPerCompany> ListPricesPerCompanies { get; set; } = new List<PricesPerCompany>();
}

public partial class ItemPicker : ContentPage
{
    public delegate void ObservableItemsChoosed_Swaped(object sender, EventArgs e);
    public event ObservableItemsChoosed_Swaped OnObservableItemsChoosed_Swaped;

    public static ObservableCollection<ItemChoosen> ObservableItemsChoosed { get; set; } = new ObservableCollection<ItemChoosen>();

    private List<Unit> _listUnits { get; set; } = null;

    private List<CompanyDTO> _listCompany { get; set; } = null;

    ItemRepository _itemRepo = null;

    UnitRepository _unitRepo = null;

    SavedCartRepository _cartRepo = null;

    CompanyRepository _compRepo = null;

    ItemPriceRepository _itemPriceRepo = null;

    List<int> _listIdsSavedAndChoosedCompanies = null;

    List<ItemPickerModel> _listItemPickerModel = new List<ItemPickerModel>();

    ItemPickerViewModel _itemPickerViewModel { get; set; } = null;

    PopUpActivityIndicator _popUpIndic = new PopUpActivityIndicator("Načítavam polôžky ...");

    Grid _gridItemPrice { get; set; } = new Grid();

    bool WasLoaded { get; set; } = false;

    private bool Rotate = true;
    public ItemPicker(ItemPickerViewModel itemPickerViewModel)
    {
        InitializeComponent();
        _itemPickerViewModel = itemPickerViewModel;
        this.BindingContext = _itemPickerViewModel;

        this.Appearing += (s, e) => { this.Content = _popUpIndic; };
        CartView.On_CartView_ItemRemovedFromList += async (s, e) => { await CartCounterControlView.DecreaseShoppingCartCounter(); };
        ObservableItemsChoosed.CollectionChanged += ItemsChoosedCollection_Changed;
    }

    protected override async void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);

        if (Connectivity.NetworkAccess == NetworkAccess.Internet)
        {
            this.Title = _itemPickerViewModel.Category.Name;

            if (!WasLoaded)
            {
                _itemRepo = _itemPickerViewModel.ItemRepo;
                _unitRepo = _itemPickerViewModel.UnitRepo;
                _cartRepo = _itemPickerViewModel.CartRepo;
                _compRepo = _itemPickerViewModel.CompanyRepo;
                _itemPriceRepo = _itemPickerViewModel.ItemPriceRepo;

                _listUnits = await _unitRepo.GetAllUnitsAsync();
                _listCompany = await _compRepo.GetAllCompaniesAsync();

                WasLoaded = true;
            }

            await Task.Run(async () =>
            {
                _listIdsSavedAndChoosedCompanies = SettingsView.GetCheckedAndSavedCompaniesFromSettings(_listCompany);

                List<Item> listItemsPerCategory = await _itemRepo.GetAllItemsAsync();
                listItemsPerCategory = listItemsPerCategory.Where(item => item.Category_Id == _itemPickerViewModel.Category.Id).ToList();

                List<ItemPrice> listItemPrices = await _itemPriceRepo.GetAllItemPricesAsync();
                listItemPrices = await _itemPriceRepo.GetPriceItemsFilteredAsync(_listIdsSavedAndChoosedCompanies, listItemsPerCategory.Select(item => item.Id).ToList());

                _listItemPickerModel.Clear();

                foreach (Item item in listItemsPerCategory)
                {
                    ItemPickerModel itemPickerModel = new ItemPickerModel();

                    itemPickerModel.Id = item.Id;
                    itemPickerModel.Name = item.Name;
                    itemPickerModel.ImageUrl = item.ImageUrl;
                    itemPickerModel.Unit_Id = item.Unit_Id;
                    itemPickerModel.Category_Id = item.Category_Id;

                    List<ItemPrice> temp = listItemPrices.Where(itemPrice => itemPrice.Item_Id == item.Id).ToList();

                    foreach (ItemPrice itemPrice in temp)
                    {
                        PricesPerCompany pricesPerCompany = new PricesPerCompany();
                        pricesPerCompany.Price = itemPrice.Price.ToString("0.00");
                        pricesPerCompany.PriceDiscount = itemPrice.PriceDiscount.ToString("0.00"); // todo otestovat ze ked neni ziadna cena co je za hodnotu
                        pricesPerCompany.CompanyImgUrl = _listCompany.Where(comp => comp.Id == itemPrice.Company_Id).First().Url;

                        itemPickerModel.ListPricesPerCompanies.Add(pricesPerCompany);
                    }

                    _listItemPickerModel.Add(itemPickerModel);
                }

                //CreateGridItemPrice(_listItemPickerModel.OrderBy(n => n.Name).ToList(), _listIdsSavedAndChoosedCompanies.Count);

            });

            this.listItem.ItemsSource = _listItemPickerModel.OrderBy(n => n.Name);

            CartCounterControlView.Init(_cartRepo, ObservableItemsChoosed);
        }
        else
        {
            await this.DisplayAlert("Chyba", "Zariadenie nemá pripojenie k internetu\r\nNie je možné načítať položky", "Zavrieť");
        }

        this.Content = this.MainControlWrapper;
    }

    private void ItemsChoosedCollection_Changed(object sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.Action == NotifyCollectionChangedAction.Add)
        {
            ObservableItemsChoosed[e.NewStartingIndex].FinalName = $"{ObservableItemsChoosed[e.NewStartingIndex].CntOfItems}x {ObservableItemsChoosed[e.NewStartingIndex].Name} {ObservableItemsChoosed[e.NewStartingIndex].UnitTag}";
        }
    }

    private async void SearchBar_TextChanged(object sender, TextChangedEventArgs e)
    {
        string text = ((SearchBar)sender).Text;

        if (text.Length >= 2)
        {
            List<Item> listItems = await _itemRepo.SearchItems(text); // todo tu sa selektuju udaje namiesto tahania od repository
            List<int> listItemsIds = listItems.Select(item => item.Id).ToList();
            List<ItemPickerModel> listItemPickerModel = _listItemPickerModel.Where(itemPicker => listItemsIds.Contains(itemPicker.Id)).ToList();

            listItem.ItemsSource = new ObservableCollection<ItemPickerModel>(listItemPickerModel);
            _itemRepo.ClearFilteredList();
        }

        if (text.Length == 0)
        {
            _itemRepo.ClearFilteredList();
            List<Item> listItems = await _itemRepo.GetAllItemsAsync();
            listItem.ItemsSource = new ObservableCollection<ItemPickerModel>(_listItemPickerModel);
        }
    }

    public ObservableCollection<ItemChoosen> GetChoosenItems()
    {
        return ObservableItemsChoosed;
    }

    public void SetChoosenItems(List<ItemChoosen> listItemChoosen)
    {
        ObservableItemsChoosed.CollectionChanged -= ItemsChoosedCollection_Changed;
        ObservableItemsChoosed.Clear();

        ObservableItemsChoosed = new ObservableCollection<ItemChoosen>(listItemChoosen.Select(i => new ItemChoosen()
        {
            Name = i.Name,
            FinalName = i.FinalName,
            Id = i.Id,
            IdInList = i.IdInList,
            CntOfItems = i.CntOfItems
        }));

        // Make sure someone is listening to event
        if (OnObservableItemsChoosed_Swaped != null)
        {
            OnObservableItemsChoosed_Swaped(this, new EventArgs());
        }

        ObservableItemsChoosed.CollectionChanged += ItemsChoosedCollection_Changed;
        CartCounterControlView.SetShoppingCartCounter(ObservableItemsChoosed.Count);
    }

    private async void listItem_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (Connectivity.NetworkAccess == NetworkAccess.Internet)
        {
            Item item = ((Item)e.CurrentSelection.First());
            int idInList = ObservableItemsChoosed.Count == 0 ? 0 : ObservableItemsChoosed.Last().IdInList + 1;

            ObservableItemsChoosed.Add(new ItemChoosen
            {
                Id = item.Id,
                IdInList = idInList,
                Name = item.Name,
                CntOfItems = 1,
                UnitTag = _listUnits.First(u => u.Id == item.Unit_Id).Tag
            });

            await CartCounterControlView.IncreaseShoppingCartCounter();
        }
        else
        {
            await this.DisplayAlert("Chyba", "Zariadenie nemá pripojenie k internetu\r\nNie je možné pridať položku", "Zavrieť");
        }
    }

    private void CreateGridItemPrice(List<ItemPickerModel> listItemPickerModel, int companyCount)
    {
        // Add Rows ////
        int cntRows = (companyCount / 2) + (companyCount % 2);
        
        for (int i = 0; i < cntRows; i++)
        {
            _gridItemPrice.AddRowDefinition(new RowDefinition(GridLength.Star));
        }
        ///////////////

        // Add Columns ////
        DeviceIdiom currIdiom = DeviceInfo.Current.Idiom;
        double compImgWidth = 0.0d;

        if ((currIdiom == DeviceIdiom.Phone) || (currIdiom == DeviceIdiom.Tablet))
        {
            compImgWidth = 48.0d;
        }
        else
        {
            compImgWidth = 64.0d;
        }

        int cntCols = 0;
        if (companyCount == 1)
        {
            _gridItemPrice.AddColumnDefinition(new ColumnDefinition(new GridLength(compImgWidth)));
            _gridItemPrice.AddColumnDefinition(new ColumnDefinition(GridLength.Star));
            _gridItemPrice.AddColumnDefinition(new ColumnDefinition(GridLength.Star));
            cntCols = 1;
        }
        else
        {
            _gridItemPrice.AddColumnDefinition(new ColumnDefinition(new GridLength(compImgWidth)));
            _gridItemPrice.AddColumnDefinition(new ColumnDefinition(GridLength.Star));
            _gridItemPrice.AddColumnDefinition(new ColumnDefinition(GridLength.Star));

            _gridItemPrice.AddColumnDefinition(new ColumnDefinition(new GridLength(compImgWidth)));
            _gridItemPrice.AddColumnDefinition(new ColumnDefinition(GridLength.Star));
            _gridItemPrice.AddColumnDefinition(new ColumnDefinition(GridLength.Star));
            cntCols = 2;
        }

        //foreach (ListPricesPerCompanies)

        //for (int iRow = 0; iRow < cntRows; iRow++)
        //{
        //    for (int iCol = 0; iCol < cntCols; iCol++)
        //    {
        //        Image image = new Image();
        //        image.SetBinding(Image.SourceProperty, )
        //                Label.Ro

        //    }
        //}
        
    }
}