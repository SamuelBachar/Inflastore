using Neminaj.ViewsModels;

namespace Neminaj.Views;

public partial class LogOutView : ContentPage
{
	public LogOutView(LogOutViewModel logOutViewModel)
	{
		InitializeComponent();
        //logOutViewModel.SignOut();
    }
}