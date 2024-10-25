﻿using CommunityToolkit.Maui;
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
using CommunityToolkit.Maui.Maps;
using Neminaj.ContentViews;


namespace Neminaj;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMauiMaps()
            .UseMauiCommunityToolkitMaps("FEq6oJaEfpIe3r3vDRPJ~T0WPrk9hOMYR0PV4kh3Stw~AhdnGP1LYApqIX77mp-7TPk4pJ38hClOgAC4Nax4MfMDZrcr97HjIJLbm4E_aMbg")
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
        builder.Services.AddSingleton<IConnectivity>(Connectivity.Current);

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
        builder.Services.AddSingleton<FirstStartUpView>();
        builder.Services.AddSingleton<LoginView>();
        builder.Services.AddSingleton<LogOutView>();
        builder.Services.AddSingleton<PriceComparerView>();
        builder.Services.AddSingleton<CartListView>();
        builder.Services.AddSingleton<NavigationView>();
        builder.Services.AddSingleton<CardsView>();
		builder.Services.AddSingleton<SettingsView>();
        builder.Services.AddSingleton<ChooseCardView>();
        builder.Services.AddSingleton<AddCardView>();
        builder.Services.AddSingleton<ItemPicker>();
        builder.Services.AddSingleton<CategoryPickerView>();

        // Views models
        builder.Services.AddSingleton<LogOutViewModel>();
        builder.Services.AddSingleton<SavedCartDetailViewModel>();
        builder.Services.AddSingleton<NavigationShopViewModel>();
        builder.Services.AddSingleton<SavedCardDetailViewModel>();
        builder.Services.AddSingleton<NotKnownCardViewModel>();
        builder.Services.AddSingleton<PriceComparerDetailViewModel>();
        builder.Services.AddSingleton<ItemPickerViewModel>();
        builder.Services.AddSingleton<CartViewModel>();

        // Transients views
        builder.Services.AddTransient<RegisterView>();
        builder.Services.AddTransient<ForgotPasswordView>();
        builder.Services.AddTransient<NavigationView>();
        builder.Services.AddTransient<PriceComparerDetailView>();
        builder.Services.AddTransient<NotKnownCardView>();
        builder.Services.AddTransient<SavedCartDetailView>(); // Car-T-
        builder.Services.AddTransient<SavedCardDetailView>(); // Car-D-
        builder.Services.AddTransient<CartViewSaveCart>();

        // Scoped views
        builder.Services.AddScoped<SubCategoryView>();
        builder.Services.AddScoped<SubCategoryViewModel>();


        // Test
        builder.Services.AddSingleton<CartView>();

        return builder.Build();
	}
}
