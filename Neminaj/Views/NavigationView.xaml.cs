#if (ANDROID || IOS || WINDOWS) && !MACCATALYST
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Maps;
#endif
using Neminaj.Models;
using Neminaj.Repositories;
using Neminaj.ViewsModels;
using SharedTypesLibrary.DTOs.API;
using SharedTypesLibrary.Models.API.DatabaseModels;
using System.Diagnostics;
using Windows.Devices.Geolocation;


#if (ANDROID || IOS || WINDOWS) && !MACCATALYST
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

    bool IsInitialized = false;

    Location _lastClickedLocation { get; set; } = null;

    string _lastClickedAddress { get; set; } = null;

    List<Pin> _listPins { get; set; } = new List<Pin>();

    public NavigationView(NavigationShopViewModel navigShopViewModel)
    {
        BindingContext = navigShopViewModel;
        this.NavigationShopViewModel = navigShopViewModel;
        this.Geolocation = NavigationShopViewModel.GeoLocation;
        this.Appearing += async (s, e) => { await NavigationView_Appearing(s, e); };
    }

    private async Task NavigationView_Appearing(object sender, EventArgs e)
    {
        if (DeviceInfo.Idiom == DeviceIdiom.Desktop)
        {
#if WINDOWS
            GeolocationAccessStatus accessStatus = await Geolocator.RequestAccessAsync();

            if (accessStatus == GeolocationAccessStatus.Allowed)
            {
                InitializeComponent();
                IsInitialized = true;
                await FindClosestShops();
            }
            else if (accessStatus == GeolocationAccessStatus.Denied ||
                     accessStatus == GeolocationAccessStatus.Unspecified)
            {
                await DisplayAlert("GPS povolenie",
                        "Aplikácia nemá povolenie pre používanie GPS.\r\n" +
                        "Prosím povoľte v nastaveniach počítača Navigáciu / GPS pre Inflastore \r\n" +
                        "Návod pre povolenie Navigácie / GPS nájdete tu: "
                        , "Ok");
                return;
            }
#endif
        }
        else
        {
            PermissionStatus status = PermissionStatus.Unknown;

            status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();

            if (status != PermissionStatus.Granted)
            {
                if (status == PermissionStatus.Denied && DeviceInfo.Platform == DevicePlatform.iOS)
                {
                    await DisplayAlert("GPS povolenie",
                        "V minulosti bolo zamietnuté aplikácii používať navigáciu.\r\n" +
                        "Prosím povoľte v nastaveniach telefónu Navigáciu / GPS pre Inflastore"
                        , "Ok");
                    return;
                }

                // Android - hlaska sa zobrazi ak uzivatel v minulosti nepovolil navigaciu
                if (Permissions.ShouldShowRationale<Permissions.LocationWhenInUse>())
                {
                    await DisplayAlert("GPS povolenie",
                                       "Inflastore vie nájsť obchody vo vašej blízkosti. Prosím povoľte v nasledujúcej výzve prístup k Navigácii / GPS"
                                      , "Ok");
                }

                status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();

                if (status != PermissionStatus.Granted)
                    return;
                else
                {
                    if (!IsInitialized)
                    {
                        InitializeComponent();
                        IsInitialized = true;
                        await FindClosestShops();
                    }
                }
            }
            else
            {
                if (!IsInitialized)
                {
                    InitializeComponent();
                    IsInitialized = true;
                    await FindClosestShops();
                }
            }
        }
    }

#if (ANDROID || IOS || WINDOWS) && !MACCATALYST
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
        Content = this.MainControl;
        //Content = googleMaps;
    }

    private async Task FindClosestShops()
    {
        List<NavigationShopData> listNavShopData = await this.NavigationShopViewModel.GetChoosenCompaniesNavigShopsDataAsync();
        List<CompanyDTO> listCompanies = await this.NavigationShopViewModel.GetChoosenCompaniesData();

        double distanceWithin = await this.NavigationShopViewModel.SettingsService.Get<double>("slider", 10.0d);

        List<FoundedShop> listNearestShops = new List<FoundedShop>();

        try
        {
            var location = await this.Geolocation.GetLastKnownLocationAsync();
            if (location == null)
            {
                location = await this.Geolocation.GetLocationAsync(new GeolocationRequest
                {
                    DesiredAccuracy = GeolocationAccuracy.Medium,
                    Timeout = TimeSpan.FromSeconds(30)
                });
            }

            // Find closest  shops within range
            foreach (NavigationShopData navShopData in listNavShopData)
            {
                double tempDistance = location.CalculateDistance(new Location(navShopData.Latitude, navShopData.Longtitude), DistanceUnits.Kilometers);

                if (tempDistance <= distanceWithin)
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

                foreach (FoundedShop shop in listNearestShops)
                {
                    Pin pin = new Pin()
                    {
                        Label = shop.CompanyName,
                        Address = shop.FullAddress,
                        Type = PinType.Place,
                        Location = new Location(shop.Latitude, shop.Longtitude)
                    };
                    pin.MarkerClicked += Pin_MarkerClicked;
                    pin.InfoWindowClicked += Pin_InfoWindowClicked;

                    _listPins.Add(pin);
#if ANDOROID || IOS || WINDOWS
                    mappy.Pins.Add(pin);
#endif
                }

                if (listNearestShops.Count > 1) // atleast 2 Shops navigation
                {
                    FoundedShop nearestShop = listNearestShops.OrderBy(shop => shop.Distance).First();
#if ANDOROID || IOS || WINDOWS
                    mappy.MoveToRegion(new MapSpan(new Location(nearestShop.Latitude, nearestShop.Longtitude), 0.075, 0.075));
#else
                    await this.NavigationShopViewModel.OpenMapWithChoosenCompanies(nearestShop);
#endif
                }
                else // Single shop navigation
                {
#if ANDOROID || IOS || WINDOWS
                    mappy.MoveToRegion(new MapSpan(new Location(listNearestShops[0].Latitude, listNearestShops[0].Longtitude), 0.05, 0.05));
#else
                    await this.NavigationShopViewModel.OpenMapWithChoosenCompanies(listNearestShops[0]);
#endif
                }
            }
            else
            {
                await DisplayAlert("Nenajdené obchody",
                    $"Vo Vami nastavenej vzdialenosti vyhľadávania: {distanceWithin} km, sa nenašli žiadne obchody.\r\n" +
                     "Nastavenie vzdialenosti vyhľadávania nájdete v nastaveniach aplikácii",
                     "Zavrieť");
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Unable to query location: {ex.Message}");
            await DisplayAlert("Navigácia chyba", $"Pri vyhľadávaní lokácií nastala chyba: {SQLConnection.StatusMessage} - {ex.Message}" , "Zavrieť");
        }
    }

    private void Pin_InfoWindowClicked(object sender, PinClickedEventArgs e)
    {
        _lastClickedLocation = ((Pin)sender).Location;
        _lastClickedAddress = $"{((Pin)sender).Label} - {((Pin)sender).Address}";
        this.btnNavigate.IsEnabled = true;
    }


    private void Pin_MarkerClicked(object sender, PinClickedEventArgs e)
    {
        _lastClickedLocation = ((Pin)sender).Location;
        _lastClickedAddress = $"{((Pin)sender).Label} - {((Pin)sender).Address}";
        this.btnNavigate.IsEnabled = true;
    }

    private async void btnNavigate_Clicked(object sender, EventArgs e)
    {
        await Map.Default.OpenAsync(_lastClickedLocation.Latitude, _lastClickedLocation.Longitude, new MapLaunchOptions { Name = _lastClickedAddress });
    }
#endif
        }