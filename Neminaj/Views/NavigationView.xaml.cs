#if (ANDROID || IOS) && !MACCATALYST
using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Maps;
#endif
using Neminaj.Models;
using Neminaj.Repositories;
using Neminaj.ViewsModels;
using SharedTypesLibrary.DTOs.API;
using SharedTypesLibrary.Models.API.DatabaseModels;
using System.Diagnostics;

#if (ANDROID || IOS) && !MACCATALYST
using Map = Microsoft.Maui.ApplicationModel.Map;
#endif

namespace Neminaj.Views;

public class FoundedShop
{
    public string CompanyName { get; set; }
    public string FullAddress { get; set; }

    public float Latitude { get; set; }

    public float Longtitude { get; set; }

    public double Distance { get; set; }
}

public partial class NavigationView : ContentPage
{
    IGeolocation Geolocation { get; set; }

    ActivityIndicator ActivityIndicator { get; set; } = null;

    VerticalStackLayout VertStackLayout { get; set; } = null;

    Label lblLoadingData { get; set; } = null;

    NavigationShopViewModel NavigationShopViewModel { get; set; } = null;

    public NavigationView(NavigationShopViewModel navigShopViewModel)
	{
		InitializeComponent();

        BindingContext = navigShopViewModel;
        this.NavigationShopViewModel = navigShopViewModel;
        this.Geolocation = NavigationShopViewModel.GeoLocation;

        //CreateActivityIndicator();
        //TurnOnActivityIndicator();
    }

#if (ANDROID || IOS) && !MACCATALYST
    protected override async void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);

        await FindClosestShops();

        //await NavigateToBuilding25();
    }

    public async Task NavigateToBuilding25()
    {
        var location = new Location(47.645160, -122.1306032);
        var options = new MapLaunchOptions { Name = "Microsoft Building 25" };

        try
        {
            await Map.Default.OpenAsync(location, options);
        }
        catch (Exception ex)
        {
            // No map application available to open
        }
    }

    public void CreateActivityIndicator()
    {
        ActivityIndicator = new ActivityIndicator
        {
            IsRunning = false,
            Color = Colors.Orange
        };
    }

    public void TurnOnActivityIndicator()
    {
        if (!ActivityIndicator.IsRunning)
        {
            if (ActivityIndicator.Parent != null)
                VertStackLayout.Remove(ActivityIndicator);

            ActivityIndicator.IsRunning = true;
            ActivityIndicator.HorizontalOptions = LayoutOptions.Center;
            ActivityIndicator.VerticalOptions = LayoutOptions.Center;
            ActivityIndicator.HeightRequest = 200;
            ActivityIndicator.WidthRequest = 200;

            VertStackLayout = new VerticalStackLayout();
            lblLoadingData = new Label
            {
                Text = "Načítavam dáta",
                FontSize = 32,
                FontAttributes = FontAttributes.Bold,
                TextColor = Colors.DarkGrey,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.End,
            };

            VertStackLayout.HorizontalOptions = LayoutOptions.Center;
            VertStackLayout.VerticalOptions = LayoutOptions.Center;
            VertStackLayout.Add(ActivityIndicator);
            VertStackLayout.Add(lblLoadingData);

            Content = VertStackLayout;
        }
    }

    public void TurnOffActivityIndicator()
    {
        ActivityIndicator.IsRunning = false;
        //Content = mappy;
        Content = googleMaps;
    }

    private async Task FindClosestShops()
    {
        List<NavigationShopData> listNavShopData = await this.NavigationShopViewModel.GetChoosenCompaniesNavigShopsDataAsync();
        List<CompanyDTO> listCompanies = await this.NavigationShopViewModel.GetChoosenCompaniesData();

        double distanceWithin = await this.NavigationShopViewModel.SettingsService.Get<double>("slider", 10.0d);

        List<FoundedShop> listNearestShops = new List<FoundedShop>();

        try
        {
            //var location = await this.Geolocation.GetLastKnownLocationAsync();

            //if (location == null)
            //{
                var location = await this.Geolocation.GetLocationAsync( new GeolocationRequest 
                {
                    DesiredAccuracy = GeolocationAccuracy.Medium,
                    Timeout = TimeSpan.FromSeconds(30)
                }); 
            //}

            // Find closest  shops within range
            foreach (NavigationShopData navShopData in listNavShopData)
            {
                double tempDistance = location.CalculateDistance(new Location(navShopData.Latitude, navShopData.Longtitude), DistanceUnits.Kilometers);

                if ( tempDistance <= distanceWithin)
                {
                    listNearestShops.Add(new FoundedShop
                    {
                        CompanyName = listCompanies.First(comp => comp.Id == navShopData.Company_Id).Name,
                        FullAddress = navShopData.FullAddress,
                        Latitude = navShopData.Latitude,
                        Longtitude = navShopData.Longtitude,
                        Distance = tempDistance
                    });
                }
            }

            if (listNearestShops.Count > 0)
            {
                listNearestShops.OrderBy(adr => adr.Distance);

                //await this.NavigationShopViewModel.OpenMapWithChoosenCompanies(listNearestShops);

                foreach (FoundedShop shop in listNearestShops)
                {
                    googleMaps.Pins.Add(
                        new Maui.GoogleMaps.Pin
                        {
                            Label = shop.CompanyName,
                            Address = shop.FullAddress,
                            Type = Maui.GoogleMaps.PinType.Place,
                            Position = new Maui.GoogleMaps.Position(shop.Latitude, shop.Longtitude)

                        });
                    //mappy.Pins.Add(
                    //    new Pin()
                    //    {
                    //        Label = shop.CompanyName,
                    //        Address = shop.FullAddress,
                    //        Type = PinType.Place,
                    //        Location = new Location(shop.Latitude, shop.Longtitude)
                    //    });
                }

                if (listNearestShops.Count > 1) // atleast 2 Shops navigation
                {

                    FoundedShop nearestShop = listNearestShops.OrderBy(shop => shop.Distance).First();

                    //mappy.MoveToRegion(new MapSpan(new Location(nearestShop.Latitude, nearestShop.Longtitude), 0.075, 0.075));
                    googleMaps.MoveToRegion(new Maui.GoogleMaps.MapSpan(new Maui.GoogleMaps.Position(nearestShop.Latitude, nearestShop.Longtitude), nearestShop.Latitude, nearestShop.Longtitude));
                }
                else // Single shop navigation
                {
                    //mappy.MoveToRegion(new MapSpan(new Location(listNearestShops[0].Latitude, listNearestShops[0].Longtitude), 0.05, 0.05));
                    googleMaps.MoveToRegion(new Maui.GoogleMaps.MapSpan(new Maui.GoogleMaps.Position(listNearestShops[0].Latitude, listNearestShops[0].Longtitude), listNearestShops[0].Latitude, listNearestShops[0].Longtitude));
                }
            }
            else
            {
                await DisplayAlert("Nenajdené obchody",
                    $"Vo Vami nastavenej vzdialenosti vyhľadávania: {distanceWithin} km, sa nenašli žiadne obchody.\r\n" +
                     "Nastavenie vzdialenosti vyhľadávania nájdete v nastaveniach aplikácii",
                     "Zavrieť") ;
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Unable to query location: {ex.Message}");
            await DisplayAlert("Navigácia chyba", "Pri vyhľadávaní lokácií nastala chyba: " + SQLConnection.StatusMessage, "Zavrieť");
        }

        TurnOffActivityIndicator();
    }
#endif
}