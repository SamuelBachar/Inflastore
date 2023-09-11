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

        if (Preferences.ContainsKey("RememberLogin"))
        {
            chkRememberLogin.IsChecked = Preferences.Get("RememberLogin", false);

            if (chkRememberLogin.IsChecked)
            {

                string userInfoSerialized = Preferences.Get(nameof(App.UserLoginInfo), "");

                if (userInfoSerialized != "")
                {
                    UserLoginInfo userLoginInfo = JsonConvert.DeserializeObject<UserLoginInfo>(userInfoSerialized);
                    EntryEmail.Text = userLoginInfo.Email;
                    EntryPassword.Text = userLoginInfo.Password;
                }
            }
        }
    }

    private async void BtnLogInHttps_Clicked(object sender, EventArgs e)
    {
        //await Shell.Current.GoToAsync($"//{nameof(ItemPicker)}");
        
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
                UserLoginInfo userLoginInfo = new UserLoginInfo { Email = EntryEmail.Text, Password = EntryPassword.Text };
                UserSessionInfo userSessionInfo = new UserSessionInfo { Email = EntryEmail.Text, JWT = response.UserLoginDTO.JWT };
                
                App.UserLoginInfo = userLoginInfo;
                App.UserSessionInfo = userSessionInfo;


                if (Preferences.ContainsKey(nameof(App.UserLoginInfo)))
                {
                    Preferences.Remove(nameof(App.UserLoginInfo));
                }

                if (Preferences.ContainsKey("RememberLogin"))
                {
                    if (Preferences.Get("RememberLogin", false))
                    {
                        string userInfoSerialized = JsonConvert.SerializeObject(App.UserLoginInfo);
                        Preferences.Set(nameof(App.UserLoginInfo), userInfoSerialized);
                    }
                }

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

    private async void txtForgotPassword_Tapped(object sender, TappedEventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(ForgotPasswordView));
    }

    private async void txtRegister_Tapped(object sender, TappedEventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(RegisterView));
    }
    private void chkRememberLogin_CheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        if (e.Value)
        {
            Preferences.Set("RememberLogin", true);
        }
        else
        {
            Preferences.Set("RememberLogin", false);
        }
    }
}