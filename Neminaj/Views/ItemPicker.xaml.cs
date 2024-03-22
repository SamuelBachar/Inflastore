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
    public int Company_Id { get; set; }
    public string CompanyImgUrl { get; set; }

    public string Price { get; set; }

    public string PriceDiscount { get; set; }
}

public class ItemPickerModel : Item
{
    public List<bool> ListVisibleComp { get; set; } = new List<bool>() { false, false, false, false, false, false };
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
                        pricesPerCompany.Company_Id = itemPrice.Company_Id;

                        itemPickerModel.ListPricesPerCompanies.Add(pricesPerCompany);
                    }

                    // Set rows visibility
                    for (int i = 0; (i < _listIdsSavedAndChoosedCompanies.Count); i++)
                    {
                        itemPickerModel.ListVisibleComp[i] = true;
                    }

                    _listItemPickerModel.Add(itemPickerModel);
                }
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

        if (text.Length >= 1)
        {
            List<Item> listItems = await _itemRepo.SearchItems(text); // todo tu sa selektuju udaje namiesto tahania od repository
            List<int> listItemsIds = listItems.Select(item => item.Id).ToList();
            List<ItemPickerModel> listItemPickerModel = _listItemPickerModel.Where(itemPicker => listItemsIds.Contains(itemPicker.Id)).ToList();

            listItem.ItemsSource = new ObservableCollection<ItemPickerModel>(listItemPickerModel.OrderBy(item => item.Name));
            _itemRepo.ClearFilteredList();
        }

        if (text.Length == 0)
        {
            _itemRepo.ClearFilteredList();
            listItem.ItemsSource = new ObservableCollection<ItemPickerModel>(_listItemPickerModel.OrderBy(item => item.Name));
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

    private async void Button_Clicked(object sender, EventArgs e)
    {
        if (Connectivity.NetworkAccess == NetworkAccess.Internet)
        {
            Button clickedButton = ((Button)sender);

            ItemPickerModel itemModel = (ItemPickerModel)clickedButton.CommandParameter;
            int idInList = ObservableItemsChoosed.Count == 0 ? 0 : ObservableItemsChoosed.Last().IdInList + 1;
            PricesPerCompany pricesPerCompany = itemModel.ListPricesPerCompanies.Where(ppc => ppc.Company_Id == int.Parse(clickedButton.ClassId)).First();

            ObservableItemsChoosed.Add(new ItemChoosen
            {
                Id = itemModel.Id,
                IdInList = idInList,
                Name = itemModel.Name,
                CntOfItems = 1,
                UnitTag = _listUnits.First(u => u.Id == itemModel.Unit_Id).Tag,
                ItemImgUrl = itemModel.ImageUrl,
                Company_Id = pricesPerCompany.Company_Id,
                CompanyImgUrl = pricesPerCompany.CompanyImgUrl,
                PriceCartOrig = pricesPerCompany.Price,
                PriceDiscountOrig = pricesPerCompany.PriceDiscount,
                PriceCartCalc = pricesPerCompany.Price,
                PriceDiscountCalc = pricesPerCompany.PriceDiscount,
            });

            await CartCounterControlView.IncreaseShoppingCartCounter();
        }
        else
        {
            await this.DisplayAlert("Chyba", "Zariadenie nemá pripojenie k internetu\r\nNie je možné pridať položku", "Zavrieť");
        }
    }
}