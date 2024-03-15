using CommunityToolkit.Maui.Views;
using Neminaj.ContentViews;
using Neminaj.Events;
using Neminaj.Models;
using Neminaj.Repositories;
using Neminaj.ViewsModels;
using System.Collections.ObjectModel;
using static Xamarin.Google.Crypto.Tink.Shaded.Protobuf.Internal;

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
        listItemChoosen.ItemsSource = CartViewModel.GetItemChoosens().OrderBy(item => item.Company_Id);
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
}