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
        Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("Mjc2NjY1MEAzMjMzMmUzMDJlMzBqRmhlcTE4dmlmbEx1M3N6aHZDRlBHSWdhNDJPWDd2ak5PVURUb3pmTkpnPQ==");

        // Important only light mode activate
        Application.Current.UserAppTheme = AppTheme.Light;
        this.RequestedThemeChanged += (s, e) => {
            Application.Current.UserAppTheme = AppTheme.Light; 
        };

        MainPage = new AppShell();
	}
}
