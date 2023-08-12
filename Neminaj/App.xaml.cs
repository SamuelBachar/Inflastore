using Neminaj.Models;
using Neminaj.Repositories;

namespace Neminaj;

public partial class App : Application
{
	public static UserLoginInfo UserLoginInfo;

    public static UserSessionInfo UserSessionInfo;
    public App()
	{
        InitializeComponent();
        Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("MjYzODI5N0AzMjMyMmUzMDJlMzBHbkM0WlN3MitzaVIyRk96bWQyandrb3EzZmdJSWYyUlVlQ01RbVlINzNVPQ==");

		MainPage = new AppShell();
	}
}
