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

    public AddCardView(SavedCardDetailViewModel savedCardViewModel)
    {
        InitializeComponent();

        BindingContext = savedCardViewModel;
        SavedCardViewModel = savedCardViewModel;

        NotKnownCardView.On_NotKnownCardView_BtnAddCard_Clicked += On_NotKnownCardView_BtnAddCard_Clicked;

#if IOS
            BarcodeScanner.Mobile.Methods.SetSupportBarcodeFormat(BarcodeScanner.Mobile.BarcodeFormats.All);
            BarcodeScanner.Mobile.Methods.AskForRequiredPermission();
#endif

#if ANDROID
            BarcodeScanner.Mobile.Methods.SetSupportBarcodeFormat(BarcodeScanner.Mobile.BarcodeFormats.All);
            BarcodeScanner.Mobile.Methods.AskForRequiredPermission();
#endif


        //cameraView.BarCodeOptions = new()
        //{
        //    PossibleFormats =
        //    {
        //        ZXing.BarcodeFormat.QR_CODE,
        //        ZXing.BarcodeFormat.All_1D,
        //        ZXing.BarcodeFormat.MAXICODE,
        //        ZXing.BarcodeFormat.RSS_14
        //    },
        //    AutoRotate = true,
        //    TryHarder = true,
        //    TryInverted = true,
        //    ReadMultipleCodes = false
        //};

        //cameraView.BarCodeDetectionFrameRate = 3;
        ////cameraView.BarCodeDetectionMaxThreads = 5;
        //cameraView.ControlBarcodeResultDuplicate = true;
        //cameraView.BarCodeDetectionEnabled = true;

        //if (cameraView.MaxZoomFactor >= 2.0f)
        //    cameraView.ZoomFactor = 2.0f;

        cameraView.Options = new ZXing.Net.Maui.BarcodeReaderOptions
        {
            TryInverted = true,
            AutoRotate = true
        };

        this.Appearing += AddCardView_Appearing;
    }

    private void AddCardView_Appearing(object sender, EventArgs e)
    {
        this.btnAddCard.IsVisible = false;
        this.lblCode.Text = string.Empty;
    }

    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
        // az tu su nainicializovane hodnoty pre SavedCardViewModel
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