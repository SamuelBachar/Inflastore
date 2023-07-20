using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Neminaj.Models;
using Neminaj.Interfaces;
using Neminaj.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using Newtonsoft.Json;
using Neminaj.Views;

namespace Neminaj.ViewsModels;

public partial class LoginViewModel : BaseViewModel
{
    [ObservableProperty]
    private string email;

    [ObservableProperty]
    private string password;

    readonly ILoginService loginService = new LoginService();

    [RelayCommand]
    public async void Login()
    {
        if (!string.IsNullOrWhiteSpace(Email) && !string.IsNullOrWhiteSpace(Password)) 
        {
            var userInfo = await loginService.Login(Email, Password);

            if (Preferences.ContainsKey(nameof(App.UserInfo)))
            {
                Preferences.Remove(nameof(App.UserInfo));
            }

            string userDetails = JsonConvert.SerializeObject(userInfo);
            Preferences.Set(nameof(App.UserInfo), userDetails);
            App.UserInfo = userInfo.Item1;

            await Shell.Current.GoToAsync($"//{nameof(ItemPicker)}");
        }
    }

    [RelayCommand]
    public async void LoginHTTPS()
    {
        if (!string.IsNullOrWhiteSpace(Email) && !string.IsNullOrWhiteSpace(Password))
        {
            var userInfo = await loginService.LoginHTTPS(Email, Password);

            if (Preferences.ContainsKey(nameof(App.UserInfo)))
            {
                Preferences.Remove(nameof(App.UserInfo));
            }

            string userDetails = JsonConvert.SerializeObject(userInfo);
            Preferences.Set(nameof(App.UserInfo), userDetails);
            App.UserInfo = userInfo.Item1;

            await Shell.Current.GoToAsync($"//{nameof(ItemPicker)}");
        }
    }
}
