using CommunityToolkit.Maui;
using Camera.MAUI;
using Microsoft.Extensions.Logging;

using Neminaj.Interfaces;
using Neminaj.Repositories;
using Neminaj.Services;
using Neminaj.Views;
using Neminaj.ViewsModels;
using System.Reflection;
using Syncfusion.Maui.Core.Hosting;
using BarcodeScanner.Mobile;

using ZXing.Net.Maui.Controls;

#if ANDROID || IOS
    using Maui.GoogleMaps.Hosting;
#endif

namespace Neminaj;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMauiMaps()
#if ANDROID
            .UseGoogleMaps()
#elif IOS
            .UseGoogleMaps("AIzaSyDTndQJ2Z_W4QZnHUPvgkX2AC1LWfch5Ok")
#endif
            .UseBarcodeReader()
            .ConfigureSyncfusionCore()
            .UseMauiCommunityToolkit()
            .UseMauiCameraView()
            .ConfigureMauiHandlers( handlers =>
            {
                handlers.AddBarcodeScannerHandler();
            })
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

#if DEBUG
		builder.Logging.AddDebug();
#endif

        builder.Services.AddSingleton<SQLConnection>(s => ActivatorUtilities.CreateInstance<SQLConnection>(s));

        //if (!File.Exists(ItemRepository._dbPath))
        //{
            // TODO only do this when app first runs - this will happen all the time which is currently needed
   //         var assembly = IntrospectionExtensions.GetTypeInfo(typeof(App)).Assembly;

			//using (Stream stream = assembly.GetManifestResourceStream("Neminaj.Resources.Database.neminaj.db3"))
			//{
			//	using (MemoryStream memoryStream = new MemoryStream())
			//	{
			//		stream.CopyTo(memoryStream);
   //                 File.WriteAllBytes(SQLConnection.m_DBPath, memoryStream.ToArray());
			//	}
			//}
        //}

        // Https client
        builder.Services.AddSingleton<IPlatformHttpMessageHandler>(sp =>
        {
//#if ANDROID
//            return new Neminaj.Services.HttpClientService();
//#elif IOS
//            return new Neminaj.Services.HttpClientService(); // obidve maju tie iste classy zrejme zbytocne to tu nechat takto
//#endif
            return new Neminaj.Services.HttpClientService();
        });

        builder.Services.AddHttpClient(Constants.AppConstants.HttpsClientName, httpClient =>
        {
            var baseUrl = DeviceInfo.Platform == DevicePlatform.Android
                ? "https://10.0.2.2:7279"
                : "https://localhost:7279";

            baseUrl = "https://inflastoreapi.azurewebsites.net/";

            httpClient.BaseAddress = new Uri(baseUrl);
        }).ConfigureHttpMessageHandlerBuilder(configBuilder =>
        {
            var platformHttMessageHandler = configBuilder.Services.GetRequiredService<IPlatformHttpMessageHandler>();
            configBuilder.PrimaryHandler = platformHttMessageHandler.GetPlatformSpecificHttpMessageHandler();
        });

		// Services
		builder.Services.AddSingleton<ISettingsService, SettingsService>();
        builder.Services.AddSingleton<IGeolocation>(Geolocation.Default);
		builder.Services.AddSingleton<IMap>(Map.Default);
        builder.Services.AddSingleton<ILoginService, LoginService>();
        builder.Services.AddSingleton<IRegisterService, RegisterService>();
        builder.Services.AddSingleton<IForgotPasswordService, ForgotPasswordService>();

        // Repositories ( DB queries)
        builder.Services.AddSingleton<ItemRepository>();
		builder.Services.AddSingleton<ItemPriceRepository>();
		builder.Services.AddSingleton<CompanyRepository>();
        builder.Services.AddSingleton<UnitRepository>();
        builder.Services.AddSingleton<SavedCartRepository>();
        builder.Services.AddSingleton<NavigationShopRepository>();
        builder.Services.AddSingleton<SavedCardRepository>();
        builder.Services.AddSingleton<ClubCardRepository>();
        builder.Services.AddSingleton<CategoryRepository>();

        // Views
        builder.Services.AddSingleton<LoginView>();
        builder.Services.AddSingleton<LogOutView>();
        builder.Services.AddSingleton<PriceComparerView>();
        builder.Services.AddSingleton<CartListView>();
        builder.Services.AddSingleton<NavigationView>();
        builder.Services.AddSingleton<CardsView>();
		builder.Services.AddSingleton<SettingsView>();
        builder.Services.AddSingleton<ChooseCardView>();
        builder.Services.AddSingleton<AddCardView>();

        // Views models
        builder.Services.AddSingleton<LogOutViewModel>();
        builder.Services.AddSingleton<CartViewModel>();
        builder.Services.AddSingleton<SavedCartDetailViewModel>();
        builder.Services.AddSingleton<NavigationShopViewModel>();
        builder.Services.AddSingleton<SavedCardDetailViewModel>();
        builder.Services.AddSingleton<NotKnownCardViewModel>();
        builder.Services.AddSingleton<PriceComparerDetailViewModel>();
        builder.Services.AddSingleton<ItemPickerViewModel>();

        // Transients views
        builder.Services.AddTransient<RegisterView>();
        builder.Services.AddTransient<ForgotPasswordView>();
        builder.Services.AddTransient<CartView>();
        builder.Services.AddTransient<NavigationView>();
        builder.Services.AddTransient<PriceComparerDetailView>();
        builder.Services.AddTransient<NotKnownCardView>();
        builder.Services.AddTransient<SavedCartDetailView>(); // CarT
        builder.Services.AddTransient<SavedCardDetailView>(); // CarD

        // test
        builder.Services.AddSingleton<ItemPicker>();
        builder.Services.AddSingleton<CategoryPickerView>();

        return builder.Build();
	}
}
