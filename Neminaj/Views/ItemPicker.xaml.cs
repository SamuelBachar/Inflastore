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
    public delegate void ObservableItemsChoosed_Swaped(object sender, EventArgs e);
    public event ObservableItemsChoosed_Swaped OnObservableItemsChoosed_Swaped;

    public static ObservableCollection<ItemChoosen> ObservableItemsChoosed { get; set; } = new ObservableCollection<ItemChoosen>();

    private List<Unit> ListUnits { get; set; } = null;

    ItemRepository ItemRepo = null;

    UnitRepository UnitRepo = null;

    SavedCartRepository CartRepo = null;

    ItemPickerViewModel _itemPickerViewModel { get; set; } = null;

    PopUpActivityIndicator _popUpIndic = new PopUpActivityIndicator("Načítavam polôžky ...");

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
            ItemRepo = _itemPickerViewModel.ItemRepo;
            UnitRepo = _itemPickerViewModel.UnitRepo;
            CartRepo = _itemPickerViewModel.CartRepo;

            this.Title = _itemPickerViewModel.Category.Name;

            if (ListUnits == null)
                ListUnits = await UnitRepo.GetAllUnitsAsync();

            List<Item> listItems = await ItemRepo.GetAllItemsAsync();
            listItems = listItems.Where(item => item.Category_Id == _itemPickerViewModel.Category.Id).ToList();

            this.listItem.ItemsSource = listItems.OrderBy(n => n.Name);

            CartCounterControlView.Init(CartRepo, ObservableItemsChoosed);
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
            ObservableItemsChoosed[e.NewStartingIndex].FinalName = $"{ObservableItemsChoosed[e.NewStartingIndex].CntOfItems}x {ObservableItemsChoosed[e.NewStartingIndex].Name}";
        }
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
                UnitTag = ListUnits.First(u => u.Id == item.Unit_Id).Tag
            });

            await CartCounterControlView.IncreaseShoppingCartCounter();
        }
        else
        {
            await this.DisplayAlert("Chyba", "Zariadenie nemá pripojenie k internetu\r\nNie je možné pridať položku", "Zavrieť");
        }
    }
}