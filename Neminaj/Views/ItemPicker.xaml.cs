//using Microsoft.UI.Xaml.Controls;
using Microsoft.Maui.Controls;
using Neminaj.Events;
using Neminaj.Interfaces;
using Neminaj.Models;
using Neminaj.Repositories;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Globalization;
using System.Text;
using static System.Net.Mime.MediaTypeNames;

namespace Neminaj.Views;

public partial class ItemPicker : ContentPage
{
    public delegate void ObservableItemsChoosed_Swaped(object sender, EventArgs e);
    public event ObservableItemsChoosed_Swaped OnObservableItemsChoosed_Swaped;

    public ObservableCollection<ItemChoosen> ObservableItemsChoosed { get; set; } = new ObservableCollection<ItemChoosen>();

    private List<Unit> ListUnits { get; set; } = null;

    ItemRepository ItemRepo = null;

    UnitRepository UnitRepo = null;

    SavedCartRepository CartRepo = null;

    public ItemPicker(ItemRepository itemRepo, UnitRepository unitRepo, SavedCartRepository cartRepo)
	{
		InitializeComponent();
		ItemRepo = itemRepo;
        UnitRepo = unitRepo;
        CartRepo = cartRepo;

        ObservableItemsChoosed.CollectionChanged += ItemsChoosedCollection_Changed;
            
        this.NavigationBarMainPage.GetBtnCart().Clicked += async (s, e) => { await BtnCart_Clicked(s, e); };
        this.Loaded += async (s, e) => { await GetItemsObservableCollection(); };
        this.Loaded += async (s, e) => { await GetUnits(); };
    }

    private void ItemsChoosedCollection_Changed(object sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.Action == NotifyCollectionChangedAction.Add)
        {
            ObservableItemsChoosed[e.NewStartingIndex].FinalName = $"{ObservableItemsChoosed[e.NewStartingIndex].CntOfItems}x {ObservableItemsChoosed[e.NewStartingIndex].Name}";
        }
    }

    private async Task GetItemsObservableCollection()
	{
		List<Item> listItems = await ItemRepo.GetAllItemsAsync();

        this.BindingContext = this;
		this.listItem.ItemsSource = listItems.OrderBy(n => n.Name);
    }

    private async Task GetUnits()
    {
        ListUnits = await UnitRepo.GetAllUnitsAsync();
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
        NavigationBarMainPage.SetShoppingCartCounter(ObservableItemsChoosed.Count);
    }

    private void listItem_SelectionChanged(object sender, SelectionChangedEventArgs e)
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

        NavigationBarMainPage.IncreaseShoppingCartCounter();
    }

    private async Task BtnCart_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(CartView),
            new Dictionary<string, object>
            {
                ["ObservableItemsChoosed"] = ObservableItemsChoosed,
                ["NavigationBarMainPage"] = NavigationBarMainPage,
                ["SavedCartRepository"] = CartRepo
            });
    }
}