//using BarcodeScanner.Mobile;
using Neminaj.Models;
using Neminaj.ViewsModels;

namespace Neminaj.Views;


public class TempCardData
{
    public ZXing.Net.Maui.BarcodeFormat Format { get; set; }
    
    public string CardCode { get; set; }

    public byte[] Image { get; set; }

    public bool IsKnownCard { get; set; } = false;

    public string CardName { get; set; }
}

public partial class AddCardView : ContentPage
{
    // Events
    public delegate void AddCardView_CardAdded(object sender, EventArgs e);
    public static event AddCardView_CardAdded On_AddCardView_CardAdded;

    TempCardData TempCardData { get; set; } = new TempCardData();
    SavedCardDetailViewModel SavedCardViewModel { get; set; } = null;

    ResultNotKnownCard ResultNotKnownCard { get; set; } = new ResultNotKnownCard();

    bool IsInitialized = false;

    public AddCardView(SavedCardDetailViewModel savedCardViewModel)
    {
        this.Appearing += async (s, e) => { await NavigationView_Appearing(s, e); };

        BindingContext = savedCardViewModel;
        SavedCardViewModel = savedCardViewModel;

        NotKnownCardView.On_NotKnownCardView_BtnAddCard_Clicked += On_NotKnownCardView_BtnAddCard_Clicked;

        BarcodeScanner.Mobile.Methods.SetSupportBarcodeFormat(BarcodeScanner.Mobile.BarcodeFormats.All);
    }

    private async Task NavigationView_Appearing(object sender, EventArgs e)
    {
        PermissionStatus status = PermissionStatus.Unknown;

        status = await Permissions.CheckStatusAsync<Permissions.Camera>();

        if (status != PermissionStatus.Granted)
        {
            if (status == PermissionStatus.Denied && DeviceInfo.Platform == DevicePlatform.iOS)
            {
                await DisplayAlert("Kamera povolenie",
                    "V minulosti bolo zamietnuté aplikácii používať kameru.\r\n" +
                    "Prosím povoľte v nastaveniach telefónu kameru pre Inflastore"
                    , "Ok");
                return;
            }

            // Android - hlaska sa zobrazi ak uzivatel v minulosti nepovolil navigaciu
            if (Permissions.ShouldShowRationale<Permissions.Camera>())
            {
                await DisplayAlert("Kamera povolenie",
                                   "Inflastore vie nájsť obchody vo vašej blízkosti. Prosím povoľte v nasledujúcej výzve prístup ku kamere"
                                  , "Ok");
            }

            status = await Permissions.RequestAsync<Permissions.Camera>();

            if (status != PermissionStatus.Granted)
                return;
            else
            {
                if (!IsInitialized)
                {
                    InitializeComponent();

                    this.btnAddCard.IsVisible = false;
                    this.lblCode.Text = string.Empty;
                    cameraView.Options = new ZXing.Net.Maui.BarcodeReaderOptions
                    {
                        TryInverted = true,
                        AutoRotate = true
                    };

                    IsInitialized = true;
                }
            }
        }
        else
        {
            if (!IsInitialized)
            {
                InitializeComponent();

                this.btnAddCard.IsVisible = false;
                this.lblCode.Text = string.Empty;
                cameraView.Options = new ZXing.Net.Maui.BarcodeReaderOptions
                {
                    TryInverted = true,
                    AutoRotate = true
                };

                IsInitialized = true;
            }
        }
    }

    private async void btnAddCard_Clicked(object sender, EventArgs e)
    {
        SavedCard savedCard = new SavedCard();
        savedCard.CardFormat = (int)TempCardData.Format;
        savedCard.CardCode = TempCardData.CardCode;
        savedCard.IsKnownCard = TempCardData.IsKnownCard;

        if (!TempCardData.IsKnownCard)
        {
            await Shell.Current.GoToAsync(nameof(NotKnownCardView),
            new Dictionary<string, object>
            {
                [nameof(ResultNotKnownCard)] = ResultNotKnownCard,
            });
        }
        else
        {
            savedCard.Image = TempCardData.Image;
            savedCard.CardName = TempCardData.CardName;
            await InsertNewCard(savedCard);
        }
    }

    private async void On_NotKnownCardView_BtnAddCard_Clicked(object sender, EventArgs e)
    {
        SavedCard savedCard = new SavedCard();
        savedCard.CardFormat = (int)TempCardData.Format;
        savedCard.CardCode = TempCardData.CardCode;

        savedCard.CardName = ResultNotKnownCard.CardName;
        savedCard.IsKnownCard = false;
        savedCard.UknownCardColorDB = ResultNotKnownCard.NotKnownCardColor;

        await InsertNewCard(savedCard);
    }

    private async Task InsertNewCard(SavedCard savedCard)
    {
        if (!await SavedCardViewModel.InsertNewCard(savedCard))
        {
            await DisplayAlert("Chyba ukladania karty", "Pri ukladaní karty nastala chyba: " + SQLConnection.StatusMessage, "Zavrieť");
        }
        else
        {
            // Make sure someone is listening to event
            if (On_AddCardView_CardAdded != null)
            {
                On_AddCardView_CardAdded(this, new EventArgs()); // toto bolo vela krat ked view bolo transient
            }

            await Task.Delay(500);
            await DisplayAlert("", "Karta uložená", "Zavrieť");
        }

        TempCardData.IsKnownCard = false;
        TempCardData.CardCode = string.Empty;
        TempCardData.CardName = string.Empty;
        TempCardData.Format = 0x00U;
        TempCardData.Image = null;

        btnAddCard.IsVisible = false;
        lblCode.Text = string.Empty;
        //this.CardImage.Source = null;


        await Shell.Current.GoToAsync("..");
    }

    private void cameraView_BarcodesDetected(object sender, ZXing.Net.Maui.BarcodeDetectionEventArgs e)
    {
        Dispatcher.Dispatch(() =>
        { 
            string text = $"{e.Results[0].Value}: {e.Results[0].Format}";
            lblCode.Text = $"Skenovanie úspešné:\r\n{text}";

            TempCardData.Format = e.Results[0].Format;
            TempCardData.CardCode = e.Results[0].Value;
            TempCardData.Image = SavedCardViewModel.CardData.CardImage;
            TempCardData.CardName = SavedCardViewModel.CardData.Name;
            TempCardData.IsKnownCard = SavedCardViewModel.CardData.IsKnownCard;

            this.btnAddCard.IsVisible = true;
        });
    }

    //private void cameraView_OnDetected(object sender, BarcodeScanner.Mobile.OnDetectedEventArg e)
    //{
    //    List<BarcodeResult> obj = e.BarcodeResults;

    //    string result = string.Empty;

    //    for (int i =0; i < obj.Count; i++)
    //    {
    //        result += $"Type : {obj[i].BarcodeType}, Value: {obj[i].DisplayValue}{Environment.NewLine}";
    //    }

    //    Dispatcher.Dispatch(async () =>
    //    {
    //        await DisplayAlert("Result", result, "OK");
    //        cameraView.IsScanning = true;
    //    });

    //    string text = $"{obj[0].BarcodeFormat}: {obj[0].DisplayValue}";
    //    lblCode.Text = $"Skenovanie úspešné:\r\n{text}";

    //    //TempCardData.Format = obj[0].BarcodeFormat;
    //    TempCardData.CardCode = obj[0].DisplayValue;
    //    TempCardData.Image = SavedCardViewModel.CardData.CardImage;
    //    TempCardData.CardName = SavedCardViewModel.CardData.Name;
    //    TempCardData.IsKnownCard = SavedCardViewModel.CardData.IsKnownCard;

    //    this.btnAddCard.IsVisible = true;
    //}
}