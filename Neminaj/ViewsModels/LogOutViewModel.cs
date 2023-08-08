﻿using CommunityToolkit.Mvvm.Input;
using Neminaj.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neminaj.ViewsModels;

public partial class LogOutViewModel
{
    public async void SignOut() // TODO: toto prerobit na zaklade rememberlogin v loginview
    {
        if (Preferences.ContainsKey(nameof(App.UserLoginInfo)))
        {
            Preferences.Remove(nameof(App.UserLoginInfo));
        }

        await Shell.Current.GoToAsync($"//{nameof(LoginView)}");
    }
}