using CommunityToolkit.Maui.Views;
using Neminaj.Models;
using Neminaj.Repositories;
using Neminaj.Views;
using Neminaj.ViewsModels;

namespace Neminaj.ContentViews;

public partial class CartViewSaveCartPopUp : Popup
{
    List<ItemChoosen> ListItems { get; set; }
    CartViewModel CartViewModel { get; set; }

    CartView CartView { get; set; }

    public CartViewSaveCartPopUp(List<ItemChoosen> listItems, CartViewModel cartViewModel, CartView cartView)
    {
        ListItems = listItems;
        CartViewModel = cartViewModel;
        CartView = cartView;

        InitializeComponent();
    }

    private void Cancel_Button_Clicked(object sender, EventArgs e)
    {
        this.Close();
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

            if (!await CartViewModel.InsertNewCart(new SavedCart { Name = CartListName.Text, Note = CartListNote.Text })) // TODO test ... delete ! to test it easily
            {
                this.Close();
                await CartView.DisplayAlert("Chyba ukladania zoznamu", "Pri ukladaní zoznamu nastala chyba: " + SQLConnection.StatusMessage, "Zavrieť");
                success = false;
            }
            else
            {
                List<SavedCart> listSavedCarts = await CartViewModel.GetAllSavedCartsAsync();
                int idLastSavedCart = listSavedCarts.Last().Id;

                List<SavedCartItem> listSavedCartItems = new List<SavedCartItem>();

                foreach (ItemChoosen item in ListItems)
                    listSavedCartItems.Add(new SavedCartItem { Item_Id = item.Id, SavedCart_Id = idLastSavedCart, CntOfItem = item.CntOfItems });

                if (!await CartViewModel.InsertNewCartItems(listSavedCartItems)) // TODO test ... delete ! to test it easily
                {
                    this.Close();
                    await CartView.DisplayAlert("Chyba ukladania položky", "Pri ukladaní jednej z položiek nastala chyba: " + SQLConnection.StatusMessage, "Zavrieť");
                    success = false;
                }
            }

            if (success)
            {
                this.BtnCancel.IsEnabled = false;
                this.BtnSave.IsEnabled = false;

                this.LblListSaved.Text = "Zoznam uložení";
                this.LblListSaved.TextColor = Colors.Green;
                await Task.Delay(1000);
                this.Close();
            }
        }
    }

    private void CartListNote_TextChanged(object sender, TextChangedEventArgs e)
    {

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