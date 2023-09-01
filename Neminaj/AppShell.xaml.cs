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
    }
}
