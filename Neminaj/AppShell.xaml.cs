using Neminaj.Models;
using Neminaj.Views;

namespace Neminaj;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();

		Routing.RegisterRoute(nameof(CartView), typeof(CartView));
        Routing.RegisterRoute(nameof(SavedCartDetail), typeof(SavedCartDetail));
        Routing.RegisterRoute(nameof(NavigationView), typeof(NavigationView));
        Routing.RegisterRoute(nameof(AddCardView), typeof(AddCardView));
    }
}
