using CommunityToolkit.Mvvm.Input;
using Neminaj.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neminaj.ViewsModels;

public partial class LogOutViewModel : BaseViewModel
{
    public async void SignOut()
    {
        if (Preferences.ContainsKey(nameof(App.UserInfo)))
        {
            Preferences.Remove(nameof(App.UserInfo));
        }

        await Shell.Current.GoToAsync($"//{nameof(LoginView)}");
    }
}