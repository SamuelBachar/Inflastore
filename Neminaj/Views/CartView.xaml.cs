using CommunityToolkit.Maui.Views;
using Neminaj.ContentViews;
using Neminaj.Events;
using Neminaj.Models;
using Neminaj.Repositories;
using Neminaj.ViewsModels;
using System.Collections.ObjectModel;
using SharedTypesLibrary.DTOs.API;
using SharedTypesLibrary.Models.API.DatabaseModels;

namespace Neminaj.Views;

public partial class CartView : ContentPage
{

    // Event for count changed
    public delegate void ItemCountOf_Changed_Handler(object sender, ItemCountOf_Changed_EventArgs e);
    public static event ItemCountOf_Changed_Handler OnItemCountOf_Changed;

    // Events
    public delegate void CartView_ItemRemovedFromList(object sender, EventArgs e);
    public static event CartView_ItemRemovedFromList On_CartView_ItemRemovedFromList;

    private CartViewModel CartViewModel { get; set; } = null;

    SemaphoreSlim semaphoreDeleteItem = new SemaphoreSlim(1, 1);
    int _lastDeletedItemId { get; set; } = -1;

    ItemPicker _itemPicker { get; set; } = null;

    List<int> _listIdsSavedAndChoosedCompanies = null;

    List<ItemPickerModel> _listItemPickerModel = new List<ItemPickerModel>();

    ItemPickerViewModel _itemPickerViewModel { get; set; } = null;

    bool WasLoaded { get; set; } = false;

    bool ShowCartNormal { get; set; } = true;

    ItemRepository _itemRepo = null;

    UnitRepository _unitRepo = null;

    SavedCartRepository _cartRepo = null;

    CompanyRepository _compRepo = null;

    ItemPriceRepository _itemPriceRepo = null;

    private List<Unit> _listUnits { get; set; } = null;

    private List<CompanyDTO> _listCompany { get; set; } = null;

    private List<ItemChoosen> _listItemChoosenAfterLoadingCart { get; set; } = new List<ItemChoosen>();

    public class SavedCartItemLoadedModel : Item
    {
        int CntOfItems { get; set; }
        int Unit_Id { get; set; }
        int Category_Id { get; set; }

        public List<bool> ListVisibleComp { get; set; } = new List<bool>() { false, false, false, false, false, false };
        public List<PricesPerCompany> ListPricesPerCompanies { get; set; } = new List<PricesPerCompany>();
    }

    public CartView(CartViewModel cartViewModel, ItemPicker itemPicker, ItemPickerViewModel itemPickerViewModel)
    {
        InitializeComponent();
        BindingContext = cartViewModel;
        CartViewModel = cartViewModel;
        _itemPicker = itemPicker;
        _itemPickerViewModel = itemPickerViewModel;

        //listItemChoosen.ItemsSource = CartViewModel.GetItemChoosens();
        BtnCompare.Clicked += async (s, e) => { await ButtonCompare_Clicked(s, e); };
        BtnSaveCart.Clicked += async (s, e) => { await BtnSaveCart_Clicked(s, e); };
        _itemPicker.OnObservableItemsChoosed_Swaped += OnObservableItemsChoosed_Swaped; // ak sa nacita zoznam treba v kosiku zobrazit vyber z ulozenych poloziek
    }

    // When saved cart is choosen from list of saved carts
    private async void OnObservableItemsChoosed_Swaped(object sender, EventArgs args)
    {
        this.CartToChoose.IsVisible = true;
        this.CartNormal.IsVisible = false;

        ShowCartNormal = false;
        _listItemChoosenAfterLoadingCart.Clear();

        await Build_CartToChoose();
    }

    private async Task Build_CartToChoose()
    {
        List<ItemChoosen> listChoosenItemsFromCart = _itemPicker.GetChoosenItems().ToList();
        List<int> listChoosenItemsFromCartIds = listChoosenItemsFromCart.Select(itemChoosenFromCart => itemChoosenFromCart.Id).ToList();

        await Task.Run(async () =>
        {
            _listIdsSavedAndChoosedCompanies = SettingsView.GetCheckedAndSavedCompaniesFromSettings(_listCompany);

            List<Item> listItemsAll = await _itemRepo.GetAllItemsAsync();
            List<Item> listItemsFromSavedCart = listItemsAll.Where(item => listChoosenItemsFromCartIds.Contains(item.Id)).ToList();

            List<ItemPrice> listItemPrices = await _itemPriceRepo.GetAllItemPricesAsync();
            listItemPrices = await _itemPriceRepo.GetPriceItemsFilteredAsync(_listIdsSavedAndChoosedCompanies, listItemsFromSavedCart.Select(item => item.Id).ToList());

            _listItemPickerModel.Clear();

            foreach (Item item in listItemsFromSavedCart) // todo zriesit aby tam islo final name z item choosen a taktiez count
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
    }

    protected async override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);

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

        if (ShowCartNormal)
        {
            this.listItemChoosen.ItemsSource = CartViewModel.GetItemChoosens().OrderBy(item => item.Company_Id);
            this.CartNormal.IsVisible = true;
            this.CartToChoose.IsVisible = false;
        }
        else
        {
            this.CartNormal.IsVisible = false;
            this.CartToChoose.IsVisible = true;
        }
    }

    private async void Delete_Clicked(object sender, EventArgs e)
    {
        try
        {
            await semaphoreDeleteItem.WaitAsync();

            ItemChoosen itemChoosen = (ItemChoosen)((Microsoft.Maui.Controls.ImageButton)(sender)).CommandParameter;

            if (_lastDeletedItemId != itemChoosen.IdInList)
            {
                _lastDeletedItemId = itemChoosen.IdInList;
                CartViewModel.DeleteChoosenItem(itemChoosen);

                // Make sure someone is listening to event
                if (On_CartView_ItemRemovedFromList != null)
                {
                    On_CartView_ItemRemovedFromList(this, new EventArgs());
                }

                listItemChoosen.ItemsSource = CartViewModel.GetItemChoosens();
            }
        }
        finally
        {
            semaphoreDeleteItem.Release();
        }
    }

    private void EntryCntOfItem_TextChanged(object sender, TextChangedEventArgs e)
    {
        try
        {
            int idInList = 0;

            if (int.TryParse(((Entry)sender).ClassId, out idInList))
            {
                ItemChoosen itemChoosen = CartViewModel.GetItemChoosens().Where(item => item.IdInList == idInList).First();
                CartViewModel.ChangeFinalNameOfItem(itemChoosen);

                // Make sure someone is listening to event
                if (OnItemCountOf_Changed != null)
                {
                    int cntOfItem = itemChoosen.CntOfItems;
                    string finalName = itemChoosen.FinalName;
                    ItemCountOf_Changed_EventArgs args = new ItemCountOf_Changed_EventArgs(idInList, cntOfItem, finalName);
                    OnItemCountOf_Changed(this, args);
                }

                // Calculate Price and PriceDiscount

                itemChoosen.PriceCartCalc = (float.Parse(itemChoosen.PriceCartOrig) * itemChoosen.CntOfItems).ToString("0.00");
                itemChoosen.PriceDiscountCalc = (float.Parse(itemChoosen.PriceDiscountOrig) * itemChoosen.CntOfItems).ToString("0.00");

                // Find Text Labels and set new text
                Grid temp = ((Grid)((Entry)sender).Parent).Children.Where(child => child is Grid).Cast<Grid>().First();
                ((Label)temp.Children[1]).Text = $"Cena: {itemChoosen.PriceCartCalc}";
                ((Label)temp.Children[2]).Text = $"Cena so zľavou: {itemChoosen.PriceDiscountCalc}";
            }
        }
        finally
        {
            //semaphoreDeleteItem.Release();
        }
    }

    private async Task ButtonCompare_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//PriceComparerView");
    }
    private async Task BtnSaveCart_Clicked(object sender, EventArgs e)
    {
        if (Connectivity.NetworkAccess == NetworkAccess.Internet)
        {
            await Shell.Current.GoToAsync(nameof(CartViewSaveCart),
            new Dictionary<string, object>
            {
                ["ObservableItemsChoosed"] = this.CartViewModel.ObservableItemsChoosed,
                [nameof(SavedCartRepository)] = this.CartViewModel.SavedCartRepository
            });
        }
        else
        {
            await DisplayAlert("Chyba", "Zariadenie nemá pripojenie k internetu\r\nPre lokálne uloženie nákupného zoznamu je potrebné pripojenie", "Zavrieť");
        }
    }

    private void CheckBox_CheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        if (e.Value)
        {
            CheckBox chk = ((CheckBox)sender); // todo custom checkbox s vlastnym bindingom lebo neviem vybrat 

            // https://stackoverflow.com/questions/61975318/adding-command-to-checkbox-in-listview-xamarin
            int company_Id = int.Parse(chk.ClassId);
            ItemPickerModel itemModel = (ItemPickerModel)chk.BindingContext;
            PricesPerCompany pricesPerCompany = itemModel.ListPricesPerCompanies.Where(ppc => ppc.Company_Id == company_Id).First();
            int idInList = _listItemChoosenAfterLoadingCart.Count == 0 ? 0 : _listItemChoosenAfterLoadingCart.Last().IdInList + 1;

            _listItemChoosenAfterLoadingCart.Add
                (
                    new ItemChoosen
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
                    }
                );
        }
    }

    // kliknutie na potvrdenie vyberu cien z ulozeneho zoznamu
    private async void Button_Clicked(object sender, EventArgs e)
    {
        await Task.Run(() =>
        {
            CartViewModel.GetItemChoosens().Clear();

            foreach (ItemChoosen item in _listItemChoosenAfterLoadingCart)
            {
                CartViewModel.GetItemChoosens().Add(
                new ItemChoosen()
                {
                    Id = item.Id,
                    IdInList = item.IdInList,
                    Name = item.Name,
                    CntOfItems = item.CntOfItems,
                    UnitTag = item.UnitTag,
                    ItemImgUrl = item.ItemImgUrl,
                    Company_Id = item.Company_Id,
                    CompanyImgUrl = item.CompanyImgUrl,
                    PriceCartOrig = item.PriceCartOrig,
                    PriceDiscountOrig = item.PriceDiscountOrig,
                    PriceCartCalc = item.PriceCartCalc,
                    PriceDiscountCalc = item.PriceDiscountCalc,
                });
            }
        });

        this.listItemChoosen.ItemsSource = CartViewModel.GetItemChoosens();

        ShowCartNormal = true;
        this.CartNormal.IsVisible = true;
        this.CartToChoose.IsVisible = false;
    }

    private void SearchBar_TextChanged(object sender, TextChangedEventArgs e)
    {

    }
}