using Neminaj.ViewsModels;

namespace Neminaj.Views;

public partial class LogOutView : ContentPage
{
    LogOutViewModel _logOutViewModel { get; set; } = null;
    public LogOutView(LogOutViewModel logOutViewModel)
	{
		InitializeComponent();
        NavigationPage.SetHasNavigationBar(this, false);
        Shell.SetTabBarIsVisible(this, false);
        this.BindingContext = logOutViewModel;
        _logOutViewModel = logOutViewModel;
        NavigationPage.SetBackButtonTitle(this, null);
        NavigationPage.SetHasBackButton(this, false);
    }

    protected async override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
        await _logOutViewModel.SignOut();
    }
}