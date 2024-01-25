using CommunityToolkit.Maui.Views;
using Neminaj.ContentViews;
using Neminaj.Events;
using Neminaj.Models;
using Neminaj.Repositories;
using Neminaj.ViewsModels;
using System.Collections.ObjectModel;

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

    public CartView(CartViewModel cartViewModel)
    {
        InitializeComponent();
        BindingContext = cartViewModel;
        CartViewModel = cartViewModel;

        //listItemChoosen.ItemsSource = CartViewModel.GetItemChoosens();
        BtnCompare.Clicked += async (s, e) => { await ButtonCompare_Clicked(s, e); };
        BtnSaveCart.Clicked += async (s, e) => { await BtnSaveCart_Clicked(s, e); };
    }

    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
        listItemChoosen.ItemsSource = CartViewModel.GetItemChoosens();
    }

    private async void Delete_Clicked(object sender, EventArgs e)
    {
        try
        {
            await semaphoreDeleteItem.WaitAsync();

            int idInList = int.Parse(((Microsoft.Maui.Controls.ImageButton)(sender)).ClassId);

            if (_lastDeletedItemId != idInList)
            {
                _lastDeletedItemId = idInList;
                CartViewModel.DeleteChoosenItem(idInList);

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
}