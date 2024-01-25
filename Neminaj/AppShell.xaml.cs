using Neminaj.Models;
using Neminaj.Views;

namespace Neminaj;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();

		Routing.RegisterRoute(nameof(CartView), typeof(CartView));
        Routing.RegisterRoute(nameof(SavedCartDetailView), typeof(SavedCartDetailView));
        Routing.RegisterRoute(nameof(NavigationView), typeof(NavigationView));
        Routing.RegisterRoute(nameof(AddCardView), typeof(AddCardView));
        Routing.RegisterRoute(nameof(RegisterView), typeof(RegisterView));
        Routing.RegisterRoute(nameof(ForgotPasswordView), typeof(ForgotPasswordView));
        Routing.RegisterRoute(nameof(SavedCardDetailView), typeof(SavedCardDetailView));
        Routing.RegisterRoute(nameof(NotKnownCardView), typeof(NotKnownCardView));
        Routing.RegisterRoute(nameof(PriceComparerDetailView), typeof(PriceComparerDetailView));
        Routing.RegisterRoute(nameof(ChooseCardView), typeof(ChooseCardView));
        Routing.RegisterRoute(nameof(ItemPicker), typeof(ItemPicker));
        Routing.RegisterRoute(nameof(LogOutView), typeof(LogOutView));
        Routing.RegisterRoute(nameof(CartViewSaveCart), typeof(CartViewSaveCart));
    }

    // Code for closing of opened nested pages after clicking tap bar item
    protected override void OnNavigating(ShellNavigatingEventArgs args)
    {
        base.OnNavigating(args);

        if (args.Source == ShellNavigationSource.ShellSectionChanged)
        {
            var navigation = Shell.Current.Navigation;
            var pages = navigation.NavigationStack;
            for (var i = pages.Count - 1; i >= 1; i--)
            {
                navigation.RemovePage(pages[i]);
            }
        }
    }
}
