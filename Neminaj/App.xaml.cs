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

		MainPage = new AppShell();
	}
}
