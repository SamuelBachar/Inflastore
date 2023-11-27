using CommunityToolkit.Maui.Views;
using Neminaj.ContentViews;
using Neminaj.Interfaces;
using Neminaj.Models;
using Neminaj.ViewsModels;
using Newtonsoft.Json;
using SharedTypesLibrary.DTOs.API;
using static System.Net.Mime.MediaTypeNames;

namespace Neminaj.Views;

public partial class LoginView : ContentPage
{
    // ActivityIndicator ActivityIndicator { get; set; } = null;
    // VerticalStackLayout vertActivityIndicatorStackLayout { get; set; } = null;
    // Label text { get; set; } = null;

    readonly ILoginService _loginService = null;

    PopUpActivityIndicator _popUpIndic = new PopUpActivityIndicator("Prihlasujem ...");

    public LoginView(ILoginService loginService)
    {
        InitializeComponent();
        this.Appearing += LoginView_Appearing;

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

    private void LoginView_Appearing(object sender, EventArgs e)
    {
        NavigationPage.SetHasNavigationBar(this, false);
        NavigationPage.SetBackButtonTitle(this, null);
        NavigationPage.SetHasBackButton(this, false);
        Shell.SetTabBarIsVisible(this, false);
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
            //ActivityIndicatorPopUp popUpIndic = new ActivityIndicatorPopUp("Prihlasujem ...");
            //this.ShowPopup(popUpIndic);
            //popUpIndic.TurnOnActivityIndicator();

            this.Content = this._popUpIndic;

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

                await Shell.Current.GoToAsync($"//{nameof(CategoryPickerView)}");
                this.Content = this.MainControlWrapper;
                //popUpIndic.TurnOffActivityIndicator();
                //popUpIndic.Close();
            }
            else
            {
                //popUpIndic.TurnOffActivityIndicator();
                //popUpIndic.Close();
                this.Content = this.MainControlWrapper;
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

