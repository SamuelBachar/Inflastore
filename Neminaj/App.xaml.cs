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

        // For barcode generating 1D and QR code
        Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("MjY5MjU3MEAzMjMyMmUzMDJlMzBLWVZMazRJamxiemlucSsyOUIwM25HdU52T3k1MW0vQ05HcW1BVGx2c0U4PQ==");

        // Important only light mode activate
        Application.Current.UserAppTheme = AppTheme.Light;
        this.RequestedThemeChanged += (s, e) => {
            Application.Current.UserAppTheme = AppTheme.Light; 
        };

        MainPage = new AppShell();
	}
}
