using Neminaj.Interfaces;
using Neminaj.Models;
using Neminaj.ViewsModels;
using Newtonsoft.Json;
using SharedTypesLibrary.DTOs.API;

namespace Neminaj.Views;

public partial class LoginView : ContentPage
{
    readonly ILoginService _loginService = null;

    public LoginView(ILoginService loginService)
	{
		InitializeComponent();

        _loginService = loginService;
    }

    private void BtnLogIn_Clicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(EntryEmail.Text))
        {
            EntryEmail.Placeholder = "Zadajte e-mail";
            EntryEmail.PlaceholderColor = Colors.OrangeRed;
        }

        if (string.IsNullOrWhiteSpace(EntryPassword.Text))
        {
            EntryPassword.IsPassword = false;
            EntryPassword.Placeholder = "Zadajte heslo";
            EntryPassword.PlaceholderColor = Colors.OrangeRed;
        }
    }

    private async void BtnLogInHttps_Clicked(object sender, EventArgs e)
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

        if (badEnty)
            return;

        if (!string.IsNullOrWhiteSpace(EntryEmail.Text) && !string.IsNullOrWhiteSpace(EntryPassword.Text))
        {
            (UserLoginDTO UserLoginDTO, string Message) response = await _loginService.LoginHTTPS(EntryEmail.Text, EntryPassword.Text);

            if (response.UserLoginDTO != null)
            {
                if (Preferences.ContainsKey(nameof(App.UserInfo)))
                {
                    Preferences.Remove(nameof(App.UserInfo));
                }

                UserInfo userInfo = new UserInfo { Email = EntryEmail.Text, Password = EntryPassword.Text, JWT = response.UserLoginDTO.JWT };
                App.UserInfo = userInfo;

                string userInfoSerialized = JsonConvert.SerializeObject(App.UserInfo);

                Preferences.Set(nameof(App.UserInfo), userInfoSerialized);

                await Shell.Current.GoToAsync($"//{nameof(ItemPicker)}");
            }
            else
            {
                await DisplayAlert("Prihlásenie chyba", response.Message, "Zavrieť");
            }
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

    private void EntryEmail_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (EntryEmail.PlaceholderColor == Colors.OrangeRed)
        {
            EntryEmail.TextColor = Colors.Black;
        }
    }

    private void txtForgotPassword_Tapped(object sender, TappedEventArgs e)
    {
        
    }

    private async void txtRegister_Tapped(object sender, TappedEventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(RegisterView));
    }
}