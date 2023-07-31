using Neminaj.Interfaces;
using SharedTypesLibrary.DTOs.API;
using SharedTypesLibrary.Models.API;

namespace Neminaj.Views;

public partial class ForgotPasswordView : ContentPage
{
    readonly IForgotPasswordService _forgotPasswordService = null;

    ForgotPasswordRequest ForgotPasswordRequest { get; set; } = null;

    public ForgotPasswordView(IForgotPasswordService forgotPasswordService)
	{
		InitializeComponent();

        _forgotPasswordService = forgotPasswordService;
        ForgotPasswordRequest = new ForgotPasswordRequest();
    }

    private async void BtnForgotPasswordHTTPS_Clicked(object sender, EventArgs e)
    {
        bool badEnty = false;

        if (string.IsNullOrWhiteSpace(EntryEmail.Text))
        {
            EntryEmail.Placeholder = "Zadajte e-mail";
            EntryEmail.PlaceholderColor = Colors.OrangeRed;

            badEnty = true;
        }

        if (string.IsNullOrWhiteSpace(EntryPassword.Text))
        {
            EntryPassword.IsPassword = false;
            EntryPassword.Placeholder = "Zadajte heslo";
            EntryPassword.PlaceholderColor = Colors.OrangeRed;

            badEnty = true;
        }

        if (string.IsNullOrWhiteSpace(EntryPasswordConfirm.Text))
        {
            EntryPasswordConfirm.IsPassword = false;
            EntryPasswordConfirm.Placeholder = "Zadajte potvrdzujúce heslo";
            EntryPasswordConfirm.PlaceholderColor = Colors.OrangeRed;

            badEnty = true;
        }

        if (badEnty)
            return;

        ForgotPasswordRequest.Email = EntryEmail.Text;
        ForgotPasswordRequest.Password = EntryPassword.Text;
        ForgotPasswordRequest.ConfirmPassword = EntryPasswordConfirm.Text;

        (UserForgotPasswordDTO UserForgotPasswordDTO, string Message) response = await _forgotPasswordService.UserForgotPasswordHTTPS(ForgotPasswordRequest);

        if (response.UserForgotPasswordDTO != null)
        {
            await DisplayAlert("Úspešne obnovené heslo", response.Message, "Zavrieť");
        }
        else
        {
            await DisplayAlert("Nastala chyba pri obnovovaní hesla", response.Message, "Zavrieť");
        }
    }

    private void EntryEmail_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (EntryEmail.PlaceholderColor == Colors.OrangeRed)
        {
            EntryEmail.TextColor = Colors.Black;
        }
    }

    private void EntryPassword_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (!EntryPassword.IsPassword && EntryPassword.PlaceholderColor == Colors.OrangeRed)
        {
            EntryPassword.IsPassword = true;
            EntryPassword.TextColor = Colors.Black;
        }
    }

    private void EntryPasswordConfirm_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (!EntryPasswordConfirm.IsPassword && EntryPasswordConfirm.PlaceholderColor == Colors.OrangeRed)
        {
            EntryPasswordConfirm.IsPassword = true;
            EntryPasswordConfirm.TextColor = Colors.Black;
        }
    }
}