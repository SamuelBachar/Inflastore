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

    private CartViewModel CartViewModel { get; set; } = null;

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

    private void Delete_Clicked(object sender, EventArgs e)
    {
        int idInList = int.Parse(((Microsoft.Maui.Controls.ImageButton)(sender)).ClassId);
        CartViewModel.DeleteChoosenItem(idInList);
        listItemChoosen.ItemsSource = CartViewModel.GetItemChoosens();
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
        await this.ShowPopupAsync(new CartViewSaveCartPopUp(CartViewModel.GetItemChoosens().ToList(), CartViewModel, this));
    }
}