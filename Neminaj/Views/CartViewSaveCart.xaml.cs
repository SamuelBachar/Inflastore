using Neminaj.Models;
using Neminaj.ViewsModels;

namespace Neminaj.Views;

public partial class CartViewSaveCart : ContentPage
{
    List<ItemChoosen> ListItems { get; set; }
    CartViewModel _cartViewModel { get; set; }

    public CartViewSaveCart(CartViewModel cartViewModel)
    {
        _cartViewModel = cartViewModel;
        InitializeComponent();
    }

    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);

        ListItems = _cartViewModel.GetItemChoosens().ToList();
    }

    private void Cancel_Button_Clicked(object sender, EventArgs e)
    {
        //this.Close();
    }

    private async void SaveCart_Button_Clicked(object sender, EventArgs e)
    {
        if (CartListName.Text == null || CartListName.Text.Length == 0)
        {
            CartListName.PlaceholderColor = Colors.Red;
            CartListName.Placeholder = "Chýba názov";
        }
        else
        {
            bool success = true;

            if (!await _cartViewModel.InsertNewCart(new SavedCart { Name = CartListName.Text, Note = CartListNote.Text })) // TODO test ... delete ! to test it easily
            {
                await this.DisplayAlert("Chyba ukladania zoznamu", "Pri ukladaní zoznamu nastala chyba: " + SQLConnection.StatusMessage, "Zavrieť");
                success = false;
            }
            else
            {
                List<SavedCart> listSavedCarts = await _cartViewModel.GetAllSavedCartsAsync();
                int idLastSavedCart = listSavedCarts.Last().Id;

                List<SavedCartItem> listSavedCartItems = new List<SavedCartItem>();

                foreach (ItemChoosen item in ListItems)
                    listSavedCartItems.Add(new SavedCartItem { Item_Id = item.Id, SavedCart_Id = idLastSavedCart, CntOfItem = item.CntOfItems });

                if (!await _cartViewModel.InsertNewCartItems(listSavedCartItems)) // TODO test ... delete ! to test it easily
                {
                    await this.DisplayAlert("Chyba ukladania položky", "Pri ukladaní jednej z položiek nastala chyba: " + SQLConnection.StatusMessage, "Zavrieť");
                    success = false;
                }
            }

            if (success)
            {
                this.BtnSave.IsEnabled = false;

                this.LblListSaved.Text = "Zoznam uložení";
                this.LblListSaved.TextColor = Colors.Green;
            }
        }
    }

    private void CartListName_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (CartListName.Text.Length == 0)
        {
            CartListName.PlaceholderColor = Colors.Black;
            CartListName.Placeholder = "";
        }
    }
}