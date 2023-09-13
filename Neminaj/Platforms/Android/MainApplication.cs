using Android.App;
using Android.Runtime;
using AndroidX.AppCompat.App;

namespace Neminaj;

#if DEBUG
[Application(UsesCleartextTraffic = true)]
#else 
[Application]
#endif

public class MainApplication : MauiApplication
{
	public MainApplication(IntPtr handle, JniHandleOwnership ownership)
		: base(handle, ownership)
	{
		// For light mode only
		AppCompatDelegate.DefaultNightMode = AppCompatDelegate.ModeNightNo;
    }

	protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
}
