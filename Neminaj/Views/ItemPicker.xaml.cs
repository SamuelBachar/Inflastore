using CommunityToolkit.Maui.Views;
using Neminaj.ContentViews;
using Neminaj.Models;
using Neminaj.Repositories;
using Neminaj.ViewsModels;
using SharedTypesLibrary.Models.API.DatabaseModels;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace Neminaj.Views;

public partial class ItemPicker : ContentPage
{
    enum AnimationType
    {
        Out,
        In,
        Pulse
    }

    public delegate void ObservableItemsChoosed_Swaped(object sender, EventArgs e);
    public event ObservableItemsChoosed_Swaped OnObservableItemsChoosed_Swaped;

    public static ObservableCollection<ItemChoosen> ObservableItemsChoosed { get; set; } = new ObservableCollection<ItemChoosen>();
    private static int CountValue { get; set; } = 0;

    private List<Unit> ListUnits { get; set; } = null;

    ItemRepository ItemRepo = null;

    UnitRepository UnitRepo = null;

    SavedCartRepository CartRepo = null;

    ItemPickerViewModel _itemPickerViewModel { get; set; } = null;


    private bool Rotate = true;
    public ItemPicker(ItemPickerViewModel itemPickerViewModel)
	{
		InitializeComponent();
        _itemPickerViewModel = itemPickerViewModel;
        this.BindingContext = _itemPickerViewModel;

        CartView.On_CartView_ItemRemovedFromList += async (s, e) => { await DecreaseShoppingCartCounter(); };
        ObservableItemsChoosed.CollectionChanged += ItemsChoosedCollection_Changed;
            
        this.Loaded += async (s, e) => { await GetUnits(); };
        this.Appearing += async (s, e) => { await GetItems(); };
    }

    protected override async void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);

        ItemRepo = _itemPickerViewModel.ItemRepo;
        UnitRepo = _itemPickerViewModel.UnitRepo;
        CartRepo = _itemPickerViewModel.CartRepo;

        ActivityIndicatorPopUp popUpIndic = new ActivityIndicatorPopUp("Načítavam polôžky ...");
        this.ShowPopupAsync(popUpIndic);

        ListUnits = await UnitRepo.GetAllUnitsAsync();

        popUpIndic.TurnOnActivityIndicator();
        List<Item> listItems = await ItemRepo.GetAllItemsAsync();
        listItems = listItems.Where(item => item.Category_Id == _itemPickerViewModel.Category.Id).ToList();

        this.listItem.ItemsSource = listItems.OrderBy(n => n.Name);

        popUpIndic.TurnOffActivityIndicator();
    }

    private void ItemsChoosedCollection_Changed(object sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.Action == NotifyCollectionChangedAction.Add)
        {
            ObservableItemsChoosed[e.NewStartingIndex].FinalName = $"{ObservableItemsChoosed[e.NewStartingIndex].CntOfItems}x {ObservableItemsChoosed[e.NewStartingIndex].Name}";
        }
    }

    private async Task GetUnits()
	{
        //ListUnits = await UnitRepo.GetAllUnitsAsync();
    }

    private async Task GetItems()
    {
    }

    private async void SearchBar_TextChanged(object sender, TextChangedEventArgs e)
    {
        string text = ((SearchBar)sender).Text;

        if (text.Length >= 2)
        {
            List<Item> listItems = await ItemRepo.SearchItems(text);
            listItem.ItemsSource = new ObservableCollection<Item>(listItems);
            ItemRepo.ClearFilteredList();
        }

        if (text.Length == 0)
        {
            ItemRepo.ClearFilteredList();
            List<Item> listItems = await ItemRepo.GetAllItemsAsync();
            listItem.ItemsSource = new ObservableCollection<Item>(listItems);
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
        SetShoppingCartCounter(ObservableItemsChoosed.Count);
    }

    private async void listItem_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        Item item = ((Item)e.CurrentSelection.First());
        int idInList = ObservableItemsChoosed.Count == 0 ? 0 : ObservableItemsChoosed.Last().IdInList + 1;

        ObservableItemsChoosed.Add(new ItemChoosen
        {
            Id = item.Id,
            IdInList = idInList,
            Name = item.Name,
            CntOfItems = 1,
            UnitTag = ListUnits.First(u => u.Id == item.Unit_Id).Tag
        });

        await IncreaseShoppingCartCounter();
    }

    private async void CartCounter_TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(CartView),
            new Dictionary<string, object>
            {
                ["ObservableItemsChoosed"] = ObservableItemsChoosed,
                ["SavedCartRepository"] = CartRepo
            });
    }

    public async Task IncreaseShoppingCartCounter()
    {
        //Grid.SetColumn(this.BtnCart, 1);

        CountValue++;

        if (CountValue >= 100)
        {
            this.lblCartCount.Text = "99+";
        }
        else
        {
            this.lblCartCount.Text = CountValue.ToString();
        }

        await AnimateCartCounter(AnimationType.In);
    }

    public async Task DecreaseShoppingCartCounter()
    {
        CountValue--;

        if (CountValue - 1 < 0) // should not happen
            CountValue = 0;

        if (CountValue - 1 >= 0 || CountValue - 1 <= 99)
        {
            this.lblCartCount.Text = CountValue.ToString();
        }

        await AnimateCartCounter(AnimationType.Out);
    }

    public void SetShoppingCartCounter(int countValue)
    {
        CountValue = countValue;

        if (CountValue >= 100)
        {
            this.lblCartCount.Text = "99+";
        }
        else
        {
            this.lblCartCount.Text = CountValue.ToString();
        }
    }

    private async Task AnimateCartCounter(AnimationType animationType)
    {
        switch (animationType)
        {
            case AnimationType.In:
                //if (Rotate)
                //{
                //    await CartCounter.RotateTo(360, length: 250);
                //    Rotate = false;
                //}
                //else
                //{
                //    await CartCounter.RotateTo(-360, length: 250);
                //    Rotate = true;
                //}

                await Pulse();
                break;
            case AnimationType.Out:
                //await CartCounter.ScaleTo(0);
                break;
            default:
                await Pulse();
                break;
        }

        async Task Pulse()
        {
            await CartCounter.ScaleTo(1, 180);
            await CartCounter.ScaleTo(1.2, 180);
            await CartCounter.ScaleTo(1, 180);
            await CartCounter.ScaleTo(1.2, 180);
            await CartCounter.ScaleTo(1, 180);
        }
    }

}