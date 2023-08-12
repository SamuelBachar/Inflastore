﻿using CommunityToolkit.Maui;
using Camera.MAUI;
using Microsoft.Extensions.Logging;

using Neminaj.Interfaces;
using Neminaj.Repositories;
using Neminaj.Services;
using Neminaj.Views;
using Neminaj.ViewsModels;
using System.Net;
using System.Reflection;
using Syncfusion.Maui.Core.Hosting;

namespace Neminaj;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureSyncfusionCore()
            .UseMauiCommunityToolkit()
            .UseMauiMaps()
            .UseMauiCameraView()
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
            var assembly = IntrospectionExtensions.GetTypeInfo(typeof(App)).Assembly;

			using (Stream stream = assembly.GetManifestResourceStream("Neminaj.Resources.Database.neminaj.db3"))
			{
				using (MemoryStream memoryStream = new MemoryStream())
				{
					stream.CopyTo(memoryStream);

					File.WriteAllBytes(ItemRepository._dbPath, memoryStream.ToArray());
				}
			}
		//}

		// Services
		builder.Services.AddSingleton<ISettingsService, SettingsService>();
        builder.Services.AddSingleton<IGeolocation>(Geolocation.Default);
		builder.Services.AddSingleton<IMap>(Map.Default);
        builder.Services.AddSingleton<ILoginService, LoginService>();
        builder.Services.AddSingleton<IRegisterService, RegisterService>();
        builder.Services.AddSingleton<IForgotPasswordService, ForgotPasswordService>();

        //builder.Services.AddHttpClient("api", httpsClient => httpsClient.BaseAddress = new Uri("https://localhost:7279/WeatherForecast")); TODO example len mozno nastudovat

        // Repositories ( DB queries)
        builder.Services.AddSingleton<ItemRepository>();
		builder.Services.AddSingleton<ItemPriceRepository>();
		builder.Services.AddSingleton<CompanyRepository>();
        builder.Services.AddSingleton<UnitRepository>();
        builder.Services.AddSingleton<SavedCartRepository>();
        builder.Services.AddSingleton<NavigationShopRepository>();
        builder.Services.AddSingleton<SavedCardRepository>();

        // Views
        builder.Services.AddSingleton<LoginView>();
        builder.Services.AddSingleton<LogOutView>();
        builder.Services.AddSingleton<ItemPicker>();
        builder.Services.AddSingleton<PriceComparerView>();
        builder.Services.AddSingleton<CartListView>();
        builder.Services.AddSingleton<NavigationView>();
        builder.Services.AddSingleton<CardsView>();
		builder.Services.AddSingleton<SettingsView>();

        // Views models
        builder.Services.AddSingleton<LogOutViewModel>();
        builder.Services.AddSingleton<CartViewModel>();
        builder.Services.AddSingleton<SavedCartDetailViewModel>();
        builder.Services.AddSingleton<NavigationShopViewModel>();
        builder.Services.AddSingleton<SavedCardDetailViewModel>();

        // Transients
        builder.Services.AddTransient<RegisterView>();
        builder.Services.AddTransient<ForgotPasswordView>();
        builder.Services.AddTransient<CartView>();
        builder.Services.AddTransient<SavedCartDetailView>();
        builder.Services.AddTransient<NavigationView>();
        builder.Services.AddTransient<AddCardView>();
        builder.Services.AddTransient<SavedCardDetailView>();

        return builder.Build();
	}
}
