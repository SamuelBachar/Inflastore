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
using SharedTypesLibrary.DTOs.API;

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

            if (Preferences.ContainsKey(nameof(App.UserLoginInfo)))
            {
                Preferences.Remove(nameof(App.UserLoginInfo));
            }

            string userDetails = JsonConvert.SerializeObject(userInfo);
            Preferences.Set(nameof(App.UserLoginInfo), userDetails);
            App.UserLoginInfo = userInfo.Item1;

            await Shell.Current.GoToAsync($"//{nameof(ItemPicker)}");
        }
    }
}
