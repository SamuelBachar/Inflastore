using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Maps;
using Neminaj.Models;
using Neminaj.Repositories;
using Neminaj.ViewsModels;
using System.Diagnostics;

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

        CreateActivityIndicator();
        TurnOnActivityIndicator();
    }

    protected override async void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);

        await FindClosestShops();
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
        Content = mappy;
    }

    private async Task FindClosestShops()
    {
        List<NavigationShopData> listNavShopData = await this.NavigationShopViewModel.GetChoosenCompaniesNavigShopsDataAsync();
        List<Company> listCompanies = await this.NavigationShopViewModel.GetChoosenCompaniesData();

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
                    mappy.Pins.Add(
                        new Pin()
                        {
                            Label = shop.CompanyName,
                            Address = shop.FullAddress,
                            Type = PinType.Place,
                            Location = new Location(shop.Latitude, shop.Longtitude)
                        });
                }

                if (listNearestShops.Count > 1)
                {
                    FoundedShop nearest = listNearestShops[0];
                    FoundedShop farrest = listNearestShops[0];

                    foreach (FoundedShop foundShop in listNearestShops)
                    {
                        if (nearest.Latitude < foundShop.Latitude)
                            nearest = foundShop;

                        if (farrest.Latitude > foundShop.Latitude)
                            farrest = foundShop;
                    }

                    FoundedShop middleFictive = new FoundedShop
                    {
                        Latitude = ((nearest.Latitude + farrest.Latitude) / 2),
                        Longtitude = ((nearest.Longtitude + farrest.Longtitude) / 2)
                    };

                    mappy.MoveToRegion(new MapSpan(new Location(middleFictive.Latitude, middleFictive.Longtitude), 0.075, 0.075));
                }
                else
                {
                    mappy.MoveToRegion(new MapSpan(new Location(listNearestShops[0].Latitude, listNearestShops[0].Longtitude), 0.05, 0.05));
                }
            }
            else
            {
             
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Unable to query location: {ex.Message}");
            await DisplayAlert("Navigácia chyba", "Pri vyhľadávaní lokácií nastala chyba: " + SQLConnection.StatusMessage, "Zavrieť");
        }

        TurnOffActivityIndicator();
    }
}