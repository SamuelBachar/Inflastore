using Neminaj.Models;
using Neminaj.Repositories;

namespace Neminaj;

public partial class App : Application
{
	public static UserInfo UserInfo;
	public App()
	{
		InitializeComponent();

		MainPage = new AppShell();
	}
}
